#pragma warning disable IDE0290 // Use primary constructor

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
    protected Connection(Connection source) : this(source.Engine)
    {
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { _Transaction?.Dispose(); } catch { }
        try { if (IsOpen) Close(); } catch { }
        AsyncLock.Dispose();
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (_Transaction != null) await _Transaction.DisposeAsync().ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        await AsyncLock.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Connection({Engine})";

    // ----------------------------------------------------

    /// <summary>
    /// The object used to synchronize operations on this instance.
    /// </summary>
    protected AsyncLock AsyncLock = new();

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public int Retries
    {
        get => _Retries;
        set => _Retries = value >= 0
            ? value
            : throw new ArgumentException($"Number of retries '{value}' must be cero or greater.");
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

    /// <inheritdoc/>
    public abstract bool IsOpen { get; }

    /// <inheritdoc/>
    public void Open()
    {
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
                catch
                {
                    if (count <= 0) throw;
                }

                Thread.Sleep(RetryInterval);
                count--;
            }
        }

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// Invoked to open the connection against the underlying database.
    /// </summary>
    protected abstract void OnOpen();

    /// <inheritdoc/>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        var count = Retries; while (count >= 0)
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
                catch
                {
                    if (count <= 0) throw;
                }

                await Task.Delay(RetryInterval, token).ConfigureAwait(false);
                count--;
            }
        }

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <summary>
    /// Invoked to open the connection against the underlying database.
    /// </summary>
    protected abstract ValueTask OnOpenAsync(CancellationToken token);

    /// <inheritdoc/>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        using (var disp = AsyncLock.Lock())
        {
            if (_Transaction != null && _Transaction.IsActive) _Transaction.Abort();
            OnClose();
        }
    }

    /// <summary>
    /// Invoked to close the connection against the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <inheritdoc/>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await using (var disp = await AsyncLock.LockAsync().ConfigureAwait(false))
        {
            if (_Transaction != null && _Transaction.IsActive) await _Transaction.AbortAsync().ConfigureAwait(false);
            await OnCloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Invoked to close the connection against the underlying database.
    /// </summary>
    protected abstract ValueTask OnCloseAsync();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ITransaction Transaction
    {
        // Obtains a valid one, unless we are in a dispose-alike situation...
        get
        {
            if (IsDisposed || OnDisposing) // Dispose-alike situation...
            {
                if (_Transaction == null)
                {
                    _Transaction = CreateTransaction();
                    _Transaction.Dispose();
                }
            }
            else // Standard scenario...
            {
                if (_Transaction == null || _Transaction.IsDisposed)
                    _Transaction = CreateTransaction();
            }
            return _Transaction;
        }

        // We need a protected setter so that derived classes can change it...
        protected set
        {
            value.ThrowWhenDisposed();

            if (this != value.Connection) throw new ArgumentException(
                "The connection of the given transaction is not this instance.")
                .WithData(value)
                .WithData(this);
        }
    }
    ITransaction? _Transaction;

    /// <summary>
    /// Invoked to create a new default transaction associated with this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();
}