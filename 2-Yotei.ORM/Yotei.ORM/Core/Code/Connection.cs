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

    // We need to use two semaphores because tx-open may call cn-open, and we want not to wait
    // on the same semaphore!
    readonly SemaphoreSlim ConnectionSemaphore = new(1, 1);
    readonly SemaphoreSlim TransactionSemaphore = new(1, 1);

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
        try { TransactionSemaphore.Dispose(); } catch { }
        try { ConnectionSemaphore.Dispose(); } catch { }
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
        try { TransactionSemaphore.Dispose(); } catch { }
        try { ConnectionSemaphore.Dispose(); } catch { }
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
            throw new ArgumentException("Number of retries must be cero or greater.")
            .WithData(value);
    }
    = RETRIES;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan RetryInterval
    {
        get;
        set => field = value.Ticks >= 0 ? value :
            throw new ArgumentException("Retry interval must be cero or greater.")
            .WithData(value);
    }
    = TimeSpan.FromMilliseconds(RETRYINTERVAL_MS);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan LockTimeout
    {
        get;
        set => field = value.Ticks is -1 or >= 0 ? value
            : throw new ArgumentException("Invalid lock timeout interval.").WithData(value);
    }
    = TimeSpan.FromSeconds(LOCKTIMEOUT_SECS);

    // ----------------------------------------------------

    static int RandomizeMS(TimeSpan span)
    {
        var rnd = new Random(DateTime.Now.Millisecond);
        var num = rnd.Next(5, 25);
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

            var done = ConnectionSemaphore.Wait(LockTimeout);
            if (done)
            {
                try
                {
                    if (!IsOpen) OnOpen();
                    return;
                }
                catch (Exception ex) { exception = ex; }
                finally { ConnectionSemaphore.Release(); }
            }

            num--; if (num >= 0)
            {
                var ms = RandomizeMS(RetryInterval);
                Thread.Sleep(ms);
            }
        }
        throw exception ?? new TimeoutException(
            "Timeout expired while opening this connection.").WithData(this);
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

            var done = await ConnectionSemaphore.WaitAsync(LockTimeout, token).ConfigureAwait(false);
            if (done)
            {
                try
                {
                    if (!IsOpen) await OnOpenAsync(token).ConfigureAwait(false);
                    return;
                }
                catch (Exception ex) { exception = ex; }
                finally { ConnectionSemaphore.Release(); }
            }

            num--; if (num >= 0)
            {
                var ms = RandomizeMS(RetryInterval);
                await Task.Delay(ms, token).ConfigureAwait(false);
            }
        }
        throw exception ?? new TimeoutException(
            "Timeout expired while opening this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close()
    {
        if (IsDisposed) return;

        var done = ConnectionSemaphore.Wait(LockTimeout);
        if (done)
        {
            try
            {
                if (IsOpen) OnClose();
                return;
            }
            finally { ConnectionSemaphore.Release(); }
        }
        throw new TimeoutException("Timeout expired while closing this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;

        var done = await ConnectionSemaphore.WaitAsync(LockTimeout).ConfigureAwait(false);
        if (done)
        {
            try
            {
                if (IsOpen) await OnCloseAsync().ConfigureAwait(false);
                return;
            }
            finally { ConnectionSemaphore.Release(); }
        }
        throw new TimeoutException("Timeout expired while closing this connection.").WithData(this);
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
    /// <inheritdoc/>
    /// </summary>
    public IRecordsGate Records { get => field ??= CreateRecordsGate(); }

    /// <summary>
    /// Invoked to create the <see cref="IRecordsGate"/> instance associated with this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual IRecordsGate CreateRecordsGate() => new RecordsGate(this);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITransaction? Transaction { get; private set; }
    ITransaction? IConnection.Transaction { get => Transaction; set => Transaction = value; }

    /// <summary>
    /// Invoked to create a new transaction without starting it.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ITransaction StartTransaction()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        var done = TransactionSemaphore.Wait(LockTimeout);
        if (done)
        {
            try
            {
                if (Transaction != null) throw new InvalidOperationException(
                    "This connection already has an active database transaction associated to it.")
                    .WithData(this);

                Transaction = CreateTransaction();
                Transaction.Start();
                return Transaction;
            }
            finally { TransactionSemaphore.Release(); }
        }
        throw new TimeoutException(
            "Timeout expired while starting a database transaction.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual async ValueTask<ITransaction> StartTransactionAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        var done = await TransactionSemaphore.WaitAsync(LockTimeout, token).ConfigureAwait(false);
        if (done)
        {
            try
            {
                if (Transaction != null) throw new InvalidOperationException(
                    "This connection already has an active database transaction associated to it.")
                    .WithData(this);

                Transaction = CreateTransaction();
                await Transaction.StartAsync(token).ConfigureAwait(false);
                return Transaction;
            }
            finally { TransactionSemaphore.Release(); }
        }
        throw new TimeoutException(
            "Timeout expired while starting a database transaction.").WithData(this);
    }
}