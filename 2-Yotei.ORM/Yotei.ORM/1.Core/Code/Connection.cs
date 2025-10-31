namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable<IConnection>]
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 4;
    public const int RETRYINTERVALMS = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Retries = RETRIES;
        RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVALMS);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
    {
        source.ThrowWhenNull();

        Engine = source.Engine;
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { EndTransactions(disposing); } catch { }
        try { if (IsOpen) Close(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { await EndTransactionsAsync(disposing).ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Connection({Engine})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Retries
    {
        get;
        set => field = value >= 0 ? value :
            throw new ArgumentException("Number of retries must be greater than zero.")
            .WithData(value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan RetryInterval
    {
        get;
        set => field = value.Ticks >= 0 ? value :
            throw new ArgumentException("Retry interval must be equal or greater than zero.")
            .WithData(value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool IsOpen { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Open()
    {
        var num = Retries;
        do
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (IsOpen) return;

            try { OnOpen(); return; }
            catch { if (num == 0) throw; }

            if (num > 0)
            {
                if (RetryInterval.Ticks > 0) Thread.Sleep(RetryInterval);
                else Thread.Yield();
            }
            num--;
        }
        while (num >= 0);
        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        var num = Retries;
        do
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (IsOpen) return;

            try { await OnOpenAsync(token).ConfigureAwait(false); return; }
            catch { if (num == 0) throw; }

            if (num > 0)
            {
                if (RetryInterval.Ticks > 0) await Task.Delay(RetryInterval, token).ConfigureAwait(false);
                else await Task.Yield();
            }
            num--;
        }
        while (num >= 0);
        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        EndTransactions(dispose: false);
        OnClose();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await EndTransactionsAsync(dispose: false).ConfigureAwait(false);
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
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnOpenAsync(CancellationToken token);

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnCloseAsync();

    // ----------------------------------------------------

    readonly List<ITransaction> _Transactions = [];

    /// <summary>
    /// Adds the given transaction into the collection of registered ones.
    /// </summary>
    /// <param name="transaction"></param>
    internal void AddTransaction(ITransaction transaction)
    {
        lock (_Transactions)
        {
            if (!ReferenceEquals(this, transaction.Connection))
                throw new ArgumentException(
                    "The connection of the transaction is not this instance.")
                    .WithData(transaction)
                    .WithData(this);

            if (!_Transactions.Contains(transaction)) _Transactions.Add(transaction);
        }
    }

    /// <summary>
    /// Removes the given transaction from the collection of registered ones.
    /// </summary>
    /// <param name="transaction"></param>
    internal bool RemoveTransaction(ITransaction transaction)
    {
        lock (_Transactions) return _Transactions.Remove(transaction);
    }

    /// <summary>
    /// Invoked to terminate the transactions registered in this instance and, optionally,
    /// dispose them.
    /// </summary>
    /// <param name="dispose"></param>
    internal void EndTransactions(bool dispose)
    {
        foreach (var item in _Transactions.ToList())
        {
            if (dispose)
            {
                if (!item.IsDisposed) item.Dispose();
                _Transactions.Remove(item);
            }
            else
            {
                if (item.IsActive) item.Abort();
            }
        }
    }

    /// <summary>
    /// Invoked to terminate the transactions registered in this instance and, optionally,
    /// dispose them.
    /// </summary>
    /// <param name="dispose"></param>
    /// <returns></returns>
    internal async ValueTask EndTransactionsAsync(bool dispose)
    {
        foreach (var item in _Transactions.ToList())
        {
            if (dispose)
            {
                if (!item.IsDisposed) await item.DisposeAsync().ConfigureAwait(false);
                _Transactions.Remove(item);
            }
            else
            {
                if (item.IsActive) await item.AbortAsync().ConfigureAwait(false);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a transaction of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITransaction Transaction
    {
        get
        {
            if (IsDisposed || OnDisposing) // Special case, returning a disposed one...
            {
                field ??= CreateTransaction();
                field.Dispose();
            }
            else // Standard case, returning a valid one...
            {
                if (field is null || field.IsDisposed) field = CreateTransaction();
            }
            return field;
        }
    }
}