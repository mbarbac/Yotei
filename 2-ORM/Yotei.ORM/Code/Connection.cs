namespace Yotei.ORM.Code;
/*
// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
public abstract class Connection : BaseDisposable, IConnection
{
    public const int DEFAULT_RETRIES = 3;
    public const int DEFAULT_RETRY_INTERVAL_MS = 200;

    // ----------------------------------------------------

    readonly ThinLocker _Locker = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine)
    {
        Engine = engine.ThrowIfNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Connection(Connection other)
    {
        other = other.ThrowIfNull();

        Engine = other.Engine;
        Retries = other.Retries;
        RetryInterval = other.RetryInterval;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"ORM.Connection[{Engine}]";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (!IsDisposed && disposing)
        {
            try { Close(); }
            catch { }
            _Locker.Dispose();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (!IsDisposed && disposing)
        {
            try { await CloseAsync().ConfigureAwait(false); }
            catch { }
            await _Locker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract Connection Clone();
    IConnection IConnection.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public uint Retries
    {
        get => _Retries;
        set => _Retries = value;
    }
    uint _Retries = DEFAULT_RETRIES;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TimeSpan RetryInterval
    {
        get => _RetryInterval;
        set => _RetryInterval = value;
    }
    TimeSpan _RetryInterval = TimeSpan.FromMilliseconds(DEFAULT_RETRY_INTERVAL_MS);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool IsOpen { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Open()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        using var thin = _Locker.Lock();
        var count = Retries;

        while (true)
        {
            if (IsOpen) return;

            try
            {
                ThrowIfDisposed();
                ThrowOnDisposing();

                OnOpen();
            }
            catch
            {
                if (count == 0) throw;
                else
                {
                    Thread.Sleep(RetryInterval);
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        using var thin = await _Locker.LockAsync(token).ConfigureAwait(false);
        var count = Retries;

        while (true)
        {
            if (IsOpen) return;

            try
            {
                ThrowIfDisposed();
                ThrowOnDisposing();

                await OnOpenAsync(token).ConfigureAwait(false);
            }
            catch
            {
                if (count == 0) throw;
                else
                {
                    await Task.Delay(RetryInterval, token).ConfigureAwait(false);
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract ValueTask OnOpenAsync(CancellationToken token = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Close()
    {
        ThrowIfDisposed();

        using var thin = _Locker.Lock();
        if (IsOpen) OnClose();
    }

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public async ValueTask CloseAsync()
    {
        ThrowIfDisposed();

        using var thin = await _Locker.LockAsync().ConfigureAwait(false);
        if (IsOpen) await OnCloseAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract ValueTask OnCloseAsync();
}*/