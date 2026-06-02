namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable(ReturnType = typeof(IConnection))]
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 4;
    public const int RETRYINTERVAL_MS = 250;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Retries = RETRIES;
        RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVAL_MS);
    }

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
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

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
            throw new ArgumentException("Number of retries must be cero or greater.")
            .WithData(value);
    }

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsOpen { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Open()
    {
        var num = Retries;
        while (num >= 0)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (IsOpen) return;
            num--;

            try { OnOpen(); return; }
            catch { if (num < 0) throw; }

            if (num >= 0)
            {
                if (RetryInterval.Ticks > 0) Thread.Sleep(RetryInterval);
                else Thread.Yield();
            }
        }
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
        while (num >= 0)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (IsOpen) return;
            num--;

            try { await OnOpenAsync(token).ConfigureAwait(false); return; }
            catch { if (num < 0) throw; }

            if (num >= 0)
            {
                if (RetryInterval.Ticks > 0) await Task.Delay(RetryInterval, token).ConfigureAwait(false);
                else await Task.Yield();
            }
        }
        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        OnClose();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

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
}