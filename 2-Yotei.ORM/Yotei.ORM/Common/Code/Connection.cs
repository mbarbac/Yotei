namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable]
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 2;
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
    protected Connection(Connection source) : this(source.Engine)
    {
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

        try { if (IsOpen) Close(); } catch { }
        AsyncLock.Dispose();
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
        await AsyncLock.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Connection({Engine})";

    /// <summary>
    /// The object used to synchronize operations on this instance.
    /// </summary>
    protected AsyncLock AsyncLock { get; } = new();

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
        get => _Retries;
        set => _Retries = value >= 0 ? value : throw new ArgumentException(
            $"Number of retries must be cero or greater: {value}");
    }
    int _Retries = RETRIES;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan RetryInterval
    {
        get => _RetryInterval;
        set => _RetryInterval = value.Ticks >= 0 ? value : throw new ArgumentException(
            $"Retry interval must be cero or greater: {value}");
    }
    TimeSpan _RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVAL);

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
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        var count = Retries; while (count >= 0)
        {
            if (IsOpen) return;

            using (var disp = AsyncLock.Lock())
            {
                try
                {
                    ThrowWhenDisposed();
                    ThrowWhenDisposing();

                    OnOpen();
                    return;
                }
                catch { if (count <= 1) throw; }
            }

            Thread.Sleep(RetryInterval);
            count--;
        }

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        var count = Retries; while (count > 0)
        {
            if (IsOpen) return;

            await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
            {
                try
                {
                    ThrowWhenDisposed();
                    ThrowWhenDisposing();

                    await OnOpenAsync(token).ConfigureAwait(false);
                    return;
                }
                catch { if (count <= 1) throw; }
            }

            await Task.Delay(RetryInterval, token).ConfigureAwait(false);
            count--;
        }

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnOpenAsync(CancellationToken token);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        using var disp = AsyncLock.Lock();
        OnClose();
    }

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await using var disp = await AsyncLock.LockAsync().ConfigureAwait(false);
        await OnCloseAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnCloseAsync();
}