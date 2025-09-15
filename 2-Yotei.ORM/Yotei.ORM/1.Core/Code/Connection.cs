#pragma warning disable IDE0008
#pragma warning disable IDE0019

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IConnection"/>
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 4;
    public const int RETRYINTERVAL = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        ValueConverters = CreateValueConverters();
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
    {
        Engine = source.Engine;
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;

        ValueConverters = CreateValueConverters();
        ValueConverters.Clear();
        ValueConverters.AddRange(source.ValueConverters);
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { EndKnownTransactions(true); } catch { }
        try { if (IsOpen) Close(); } catch { }
        try { Lock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { await EndKnownTransactionsAsync(true).ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { await Lock.DisposeAsync().ConfigureAwait(false); } catch { }
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Connection({Engine})";

    /// <inheritdoc/>
    public abstract IConnection Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public int Retries
    {
        get => _Retries;
        set => _Retries = value > 0
            ? value
            : throw new ArgumentException($"Number of retries '{value}' must be greater than cero.");
    }
    int _Retries = RETRIES;

    /// <inheritdoc/>
    public TimeSpan RetryInterval
    {
        get => _RetryInterval;
        set => _RetryInterval = value.Ticks >= 0
            ? value
            : throw new ArgumentException($"Retry interval '{value}' must be cero or greater.");
    }
    TimeSpan _RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVAL);

    /// <summary>
    /// The object used to synchronize the operations in this instance.
    /// <br/> This object is automatically disposed along with this connection, client code must
    /// not try to manage its life cycle.
    /// </summary>
    public AsyncLock Lock { get; } = new();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract bool IsOpen { get; }

    /// <inheritdoc/>
    public void Open()
    {
        for (int num = Retries; num > 0; num--)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            using (var disp = Lock.Lock())
            {
                if (IsOpen) return;

                try { OnOpen(); return; }
                catch { if (num == 1) throw; }
            }

            Thread.Sleep(RetryInterval);
        }
        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <inheritdoc/>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        for (int num = Retries; num > 0; num--)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            await using (var disp = await Lock.LockAsync(token).ConfigureAwait(false))
            {
                if (IsOpen) return;

                try { await OnOpenAsync(token).ConfigureAwait(false); return; }
                catch { if (num == 1) throw; }
            }

            await Task.Delay(RetryInterval, token).ConfigureAwait(false);
        }
        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        using var disp = Lock.Lock();

        EndKnownTransactions(disposing: false);
        OnClose();
    }

    /// <inheritdoc/>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await using var disp = await Lock.LockAsync().ConfigureAwait(false);

        await EndKnownTransactionsAsync(disposing: false).ConfigureAwait(false);
        await OnCloseAsync().ConfigureAwait(false);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract ValueTask OnOpenAsync(CancellationToken token);

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract ValueTask OnCloseAsync();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IValueConverterList ValueConverters { get; }

    /// <summary>
    /// Invoked to create a new object of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual IValueConverterList CreateValueConverters() => new ValueConverterList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecordsGate Records => _Records ??= CreateRecordsGate();
    IRecordsGate? _Records;

    /// <summary>
    /// Invoked to create a new object of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecordsGate CreateRecordsGate();

    // ----------------------------------------------------

    List<ITransaction> KnownTransactions { get; } = [];

    /// <summary>
    /// Invoked to terminate all know transactions.
    /// </summary>
    void EndKnownTransactions(bool disposing)
    {
        var taken = false;
        try
        {
            Monitor.Enter(KnownTransactions, ref taken);
            if (taken)
            {
                while (KnownTransactions.Count > 0)
                {
                    var transaction = KnownTransactions[0];
                    KnownTransactions.RemoveAt(0);

                    var valid = transaction as Transaction;
                    if (valid != null) valid.HasOpenedConnection = false;

                    if (disposing) transaction.Dispose();
                    else if (transaction.IsActive) transaction.Abort();
                }
                if (!disposing)
                {
                    if (_Transaction != null && !KnownTransactions.Contains(_Transaction))
                        KnownTransactions.Add(_Transaction);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Cannot obtain an exclusive lock on the collection of transactions.")
                    .WithData(this);
            }
        }
        finally { if (taken) Monitor.Exit(KnownTransactions); }
    }

    /// <summary>
    /// Invoked to terminate all know transactions.
    /// </summary>
    async ValueTask EndKnownTransactionsAsync(bool disposing)
    {
        var taken = false;
        try
        {
            Monitor.Enter(KnownTransactions, ref taken);
            if (taken)
            {
                while (KnownTransactions.Count > 0)
                {
                    var transaction = KnownTransactions[0];
                    KnownTransactions.RemoveAt(0);

                    var valid = transaction as Transaction;
                    if (valid != null) valid.HasOpenedConnection = false;

                    if (disposing) await transaction.DisposeAsync().ConfigureAwait(false);
                    else if (transaction.IsActive) await transaction.AbortAsync().ConfigureAwait(false);
                }
                if (!disposing)
                {
                    if (_Transaction != null && !KnownTransactions.Contains(_Transaction))
                        KnownTransactions.Add(_Transaction);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Cannot obtain an exclusive lock on the collection of transactions.")
                    .WithData(this);
            }
        }
        finally { if (taken) Monitor.Exit(KnownTransactions); }
    }

    /// <summary>
    /// Adds the given transaction to the internal collection of known ones.
    /// </summary>
    internal void AddTransaction(ITransaction transaction)
    {
        lock (KnownTransactions)
            if (!KnownTransactions.Contains(transaction)) KnownTransactions.Add(transaction);
    }

    /// <summary>
    /// Removes the given transaction from the internal collection of known ones.
    /// </summary>
    internal bool RemoveTransaction(ITransaction transaction)
    {
        lock (KnownTransactions)
            return KnownTransactions.Remove(transaction);
    }

    /// <inheritdoc/>
    public ITransaction Transaction
    {
        get
        {
            bool disposing = IsDisposed || OnDisposing;

            if (_Transaction == null) // We may need to return a disposed one...
            {
                _Transaction = CreateTransaction();
                if (disposing) _Transaction.Dispose();
            }
            else // We may need to create a not-disposed one...
            {
                if (_Transaction.IsDisposed && !disposing) _Transaction = CreateTransaction();
            }
            return _Transaction;
        }
    }
    ITransaction? _Transaction;

    /// <summary>
    /// Invoked to create a new object of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();
}