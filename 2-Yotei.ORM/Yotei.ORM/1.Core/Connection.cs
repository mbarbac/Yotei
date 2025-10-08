namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
public abstract class Connection : DisposableClass
{
    public const int RETRIES = 4;
    public const int RETRYINTERVALMS = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(Engine engine)
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

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Connection({Engine})";

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { EndTransactions(disposing); } catch { }
        try { if (IsOpen) Close(); } catch { }
        try { Lock.Dispose(); } catch { }
    }
    void EndTransactions(bool disposing)
    {
        if (!disposing) _Transactions.Clear();
        else
        {
            var taken = false;
            try
            {
                Monitor.Enter(_Transactions, ref taken);
                if (taken)
                {
                    foreach (var item in _Transactions.ToList())
                    {
                        if (item.IsDisposed) continue;
                        item.Dispose();
                    }
                }
                else throw new InvalidOperationException(
                    "Cannot obtain an exclusive lock on the collection of transactions.")
                    .WithData(this);
            }
            finally { if (taken) Monitor.Exit(_Transactions); }
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { await EndTransactionsAsync(disposing).ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { await Lock.DisposeAsync().ConfigureAwait(false); } catch { }
    }
    async ValueTask EndTransactionsAsync(bool disposing)
    {
        if (!disposing) _Transactions.Clear();
        else
        {
            var taken = false;
            try
            {
                Monitor.Enter(_Transactions, ref taken);
                if (taken)
                {
                    foreach (var item in _Transactions.ToList())
                    {
                        if (item.IsDisposed) continue;
                        await item.DisposeAsync().ConfigureAwait(false);
                    }
                }
                else throw new InvalidOperationException(
                    "Cannot obtain an exclusive lock on the collection of transactions.")
                    .WithData(this);
            }
            finally { if (taken) Monitor.Exit(_Transactions); }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// The engine descriptor used by this connection.
    /// </summary>
    public Engine Engine { get; }

    /// <summary>
    /// The number of times this connection tries to recover from transient errors before throwing
    /// an exception.
    /// </summary>
    public int Retries
    {
        get;
        set => field = value > 0 ? value :
            throw new ArgumentException("Number of retries must be greater than zero.")
            .WithData(value);
    }

    /// <summary>
    /// The amount of time this connection waits before the next attemp to recover from transient
    /// errors.
    /// </summary>
    public TimeSpan RetryInterval
    {
        get;
        set => field = value.Ticks >= 0 ? value :
            throw new ArgumentException("Retry interval must be equal or greater than zero.")
            .WithData(value);
    }

    /// <summary>
    /// The object used to synchronize operations of this instance.
    /// </summary>
    public AsyncLock Lock { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is opened or not.
    /// </summary>
    public abstract bool IsOpen { get; }

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    public void Open()
    {
        for (int num = Retries; num > 0; num--)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            using (var disp = Lock.Enter())
            {
                if (IsOpen) return;

                try { OnOpen(); return; }
                catch { if (num <= 1) throw; }
            }
        }

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        for (int num = Retries; num > 0; num--)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            await using (var disp = await Lock.EnterAsync(token).ConfigureAwait(false))
            {
                if (IsOpen) return;

                try { await OnOpenAsync(token).ConfigureAwait(false); return; }
                catch { if (num <= 1) throw;  }
            }
        }

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        using var disp = Lock.Enter();

        var taken = false;
        try
        {
            Monitor.Enter(_Transactions, ref taken);
            if (taken)
            {
                foreach (var item in _Transactions)
                    if (item.IsActive) item.Abort();
            }
            else throw new InvalidOperationException(
                "Cannot obtain an exclusive lock on the collection of transactions.")
                .WithData(this);
        }
        finally { if (taken) Monitor.Exit(_Transactions); }

        OnClose();
    }

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    /// <returns></returns>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await using var disp = await Lock.EnterAsync().ConfigureAwait(false);

        var taken = false;
        try
        {
            Monitor.Enter(_Transactions, ref taken);
            if (taken)
            {
                foreach (var item in _Transactions)
                    if (item.IsActive) await item.AbortAsync().ConfigureAwait(false);
            }
            else throw new InvalidOperationException(
                "Cannot obtain an exclusive lock on the collection of transactions.")
                .WithData(this);
        }
        finally { if (taken) Monitor.Exit(_Transactions); }

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

    readonly List<Transaction> _Transactions = [];

    /// <summary>
    /// Invoked to add the given transaction to the collection of known ones.
    /// </summary>
    internal void AddTransaction(Transaction transaction)
    {
        transaction.ThrowWhenNull();
        lock (_Transactions)
            if (!_Transactions.Contains(transaction)) _Transactions.Add(transaction);
    }

    /// <summary>
    /// Invoked to remove the given transaction from the collection of known ones.
    /// </summary>
    internal bool RemoveTransaction(Transaction transaction)
    {
        transaction.ThrowWhenNull();
        lock (_Transactions) return _Transactions.Remove(transaction);
    }

    /// <summary>
    /// Invoked to create a new transaction of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract Transaction CreateTransaction();

    /// <summary>
    /// Returns a transaction associated with this instance.
    /// </summary>
    public Transaction Transaction
    {
        get
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            lock (_Transactions)
            {
                foreach (var transaction in _Transactions)
                    if (!transaction.IsDisposed) return transaction;

                var item = CreateTransaction(); // Adds automatically...
                return item;
            }
        }
    }
}