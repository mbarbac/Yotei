namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IConnection"/>
[Cloneable]
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 4;
    public const int RETRYINTERVAL = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
    {
        // We need not to copy the list of managed transactions because those are strictly
        // related to the instance to which they are associated, and not a new one.

        Engine = source.Engine;
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;
        foreach (var item in source.ToDatabaseConverters) ToDatabaseConverters.Add(item);
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        var items = ManagedTransactions.ToList();
        foreach (var item in items) item.Dispose();

        try { if (IsOpen) Close(); } catch { }
        try { AsyncLock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        var items = ManagedTransactions.ToList();
        foreach (var item in items) await item.DisposeAsync().ConfigureAwait(false);

        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { await AsyncLock.DisposeAsync().ConfigureAwait(false); } catch { }
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Connection({Engine})";

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

    // ----------------------------------------------------

    /// <summary>
    /// The object used to synchronize operations in this instance.
    /// </summary>
    protected AsyncLock AsyncLock { get; } = new();

    /// <inheritdoc/>
    public abstract bool IsOpen { get; }

    /// <inheritdoc/>
    public void Open()
    {
        var count = Retries;
        var first = true;

        do
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (!first) Thread.Sleep(RetryInterval);

            using (var disp = AsyncLock.Lock())
            {
                if (IsOpen) return;

                try
                {
                    OnOpen();
                    return;
                }
                catch
                {
                    if (count <= 1) throw;
                }
            }

            count--;
            first = false;
        }
        while (count > 0);

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <inheritdoc/>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        var count = Retries;
        var first = true;

        do
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (!first) await Task.Delay(RetryInterval, token).ConfigureAwait(false);

            await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
            {
                if (IsOpen) return;

                try
                {
                    await OnOpenAsync(token).ConfigureAwait(false);
                    return;
                }
                catch
                {
                    if (count <= 1) throw;
                }
            }

            count--;
            first = false;
        }
        while (count > 0);

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <inheritdoc/>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        var taken = false;
        try
        {
            Monitor.Enter(ManagedTransactions, ref taken);
            if (taken)
            {
                foreach (var item in ManagedTransactions)
                {
                    var valid = item as Transaction;
                    if (valid is not null && valid.IsActive)
                    {
                        // We are already closing, so we don't need the transaction to re-enter
                        // the close procedure again...
                        valid.HasOpenedConnection = false;
                        valid.Abort();
                    }
                }
            }
        }
        finally
        {
            if (taken) Monitor.Exit(ManagedTransactions);
        }

        using var disp = AsyncLock.Lock();
        OnClose();
    }

    /// <inheritdoc/>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        var taken = false;
        try
        {
            Monitor.Enter(ManagedTransactions, ref taken);
            if (taken)
            {
                foreach (var item in ManagedTransactions)
                {
                    var valid = item as Transaction;
                    if (valid is not null && valid.IsActive)
                    {
                        // We are already closing, so we don't need the transaction to re-enter
                        // the close procedure again...
                        valid.HasOpenedConnection = false;
                        await valid.AbortAsync().ConfigureAwait(false);
                    }
                }
            }
        }
        finally
        {
            if (taken) Monitor.Exit(ManagedTransactions);
        }

        await using var disp = await AsyncLock.LockAsync().ConfigureAwait(false);
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

    readonly List<ITransaction> ManagedTransactions = [];

    /// <summary>
    /// Adds the given transaction to the collection of managed ones maintained by this instance.
    /// </summary>
    /// <param name="transaction"></param>
    internal void AddManagedTransaction(ITransaction transaction)
    {
        transaction.ThrowWhenNull();

        lock (ManagedTransactions)
        {
            if (!ManagedTransactions.Contains(transaction)) ManagedTransactions.Add(transaction);
        }
    }

    /// <summary>
    /// Removes the given transaction from the collection of managed ones maintained by this instance.
    /// </summary>
    /// <param name="transaction"></param>
    internal void RemoveManagedTransaction(ITransaction transaction)
    {
        transaction.ThrowWhenNull();

        lock (ManagedTransactions)
        {
            ManagedTransactions.Remove(transaction);
        }
    }

    /// <inheritdoc/>
    public abstract ITransaction CreateTransaction();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new collection of converters for this instance.
    /// </summary>
    /// <returns></returns>
    protected IValueConverterList CreateDatabaseConverters() => new ValueConverterList();

    /// <inheritdoc/>
    public IValueConverterList ToDatabaseConverters => _ToDatabaseConverters ??= CreateDatabaseConverters();
    IValueConverterList? _ToDatabaseConverters = null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecordsGate Records => _Records ??= CreateRecordsGate();
    IRecordsGate? _Records;

    /// <summary>
    /// Invoked to create an appropriate instance.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecordsGate CreateRecordsGate();
}