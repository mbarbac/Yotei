namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable(ReturnType = typeof(IConnection))]
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 3;
    public const int RETRYINTERVAL_MS = 250;
    public const int LOCKTIMEOUT_SECS = 15;

    // ----------------------------------------------------

    readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Connection(Connection other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Engine = other.Engine;
        Retries = other.Retries;
        RetryInterval = other.RetryInterval;
        LockTimeout = other.LockTimeout;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { Transaction?.Abort(); } catch { }
        try { if (IsOpen) Close(); } catch { }
        try { Semaphore.Dispose(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (Transaction != null) await Transaction.AbortAsync().ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { Semaphore.Dispose(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Connextion({Engine})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// The number of times this instance tries to recover from transient errors.
    /// </summary>
    public int Retries
    {
        get;
        set => field = value >= 0 ? value :
            throw new ArgumentException("Number of retries must be cero or greater.")
            .WithData(value);
    }
    = RETRIES;

    /// <summary>
    /// <inheritdoc/>
    /// errors.
    /// </summary>
    public TimeSpan RetryInterval
    {
        get;
        set => field = (value.Ticks is -1 or >= 0) ? value :
            throw new ArgumentException("Invalid retry interval.")
            .WithData(value);
    }
    = TimeSpan.FromMilliseconds(RETRYINTERVAL_MS);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan LockTimeout
    {
        get;
        set => field = (value.Ticks is -1 or >= 0) ? value :
            throw new ArgumentException("Invalic lock timeout interval.")
            .WithData(value);
    }
    = TimeSpan.FromSeconds(LOCKTIMEOUT_SECS);

    /// <summary>
    /// Invokes to randomize the given wait span.
    /// </summary>
    static int Randomize(TimeSpan span)
    {
        var rnd = new Random(DateTime.Now.Millisecond);
        var num = rnd.Next(5, 50);
        return span.Milliseconds + num;
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
        Exception? exception = null;
        var num = Retries;
        while (num >= 0)
        {
            exception = null;
            ThrowIfDisposed();
            ThrowOnDisposing();

            var done = Semaphore.Wait(LockTimeout);
            if (done)
            {
                try
                {
                    if (!IsOpen) OnOpen();
                    return;
                }
                catch (Exception e) { exception = e; }
                finally { Semaphore.Release(); }
            }

            num--; if (num >= 0)
            {
                var ms = Randomize(RetryInterval);
                Thread.Sleep(ms);
            }
        }

        throw exception ??
            new TimeoutException("Timeout expired while opening this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        Exception? exception = null;
        var num = Retries;
        while (num >= 0)
        {
            exception = null;
            ThrowIfDisposed();
            ThrowOnDisposing();

            var done = await Semaphore.WaitAsync(LockTimeout, token).ConfigureAwait(false);
            if (done)
            {
                try
                {
                    if (!IsOpen) await OnOpenAsync(token).ConfigureAwait(false);
                    return;
                }
                catch (Exception e) { exception = e; }
                finally { Semaphore.Release(); }
            }

            num--; if (num >= 0)
            {
                var ms = Randomize(RetryInterval);
                await Task.Delay(ms, token).ConfigureAwait(false);
            }
        }

        throw exception ??
            new TimeoutException("Timeout expired while opening this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close()
    {
        if (IsDisposed) return;

        var done = Semaphore.Wait(LockTimeout);
        if (done)
        {
            try
            {
                if (Transaction != null)
                {
                    if (Transaction.IsActive) Transaction.Abort();
                    Transaction = null;
                }

                if (IsOpen) OnClose();
                return;
            }
            finally { Semaphore.Release(); }
        }

        throw new TimeoutException("Timeout expired while opening this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;

        var done = await Semaphore.WaitAsync(LockTimeout).ConfigureAwait(false);
        if (done)
        {
            try
            {
                if (Transaction != null)
                {
                    if (Transaction.IsActive) await Transaction.AbortAsync().ConfigureAwait(false);
                    Transaction = null;
                }

                if (IsOpen) await OnCloseAsync().ConfigureAwait(false);
                return;
            }
            finally { Semaphore.Release(); }
        }

        throw new TimeoutException("Timeout expired while opening this connection.").WithData(this);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to open (connect) this instance.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// Invoked to open (connect) this instance.
    /// </summary>
    protected abstract ValueTask OnOpenAsync(CancellationToken token);

    /// <summary>
    /// Invoked to close this instance.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// Invoked to close this instance.
    /// </summary>
    protected abstract ValueTask OnCloseAsync();

    // ----------------------------------------------------

    /// <summary>
    /// The current transaction associated with this instance, or null if any.
    /// </summary>
    public virtual ITransaction? Transaction { get; internal set; }
    ITransaction? IConnection.Transaction
    {
        get => Transaction;
        set => Transaction = value;
    }

    /// <summary>
    /// Invoked to create an instance of the appropriate type.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();

    /// <summary>
    /// Starts a database transaction for this instance, and sets it as the current one.
    /// </summary>
    /// <returns></returns>
    public virtual ITransaction StartTransaction()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        if (Transaction != null) throw new InvalidOperationException(
            "A transaction is already associated with this instance.")
            .WithData(Transaction);

        Transaction = CreateTransaction();
        Transaction.Start();
        return Transaction;
    }

    /// <summary>
    /// Starts a database transaction for this instance, and sets it as the current one.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual async ValueTask<ITransaction> StartTransactionAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        if (Transaction != null) throw new InvalidOperationException(
            "A transaction is already associated with this instance.")
            .WithData(Transaction);

        Transaction = CreateTransaction();
        await Transaction.StartAsync(token).ConfigureAwait(false);
        return Transaction;
    }
}