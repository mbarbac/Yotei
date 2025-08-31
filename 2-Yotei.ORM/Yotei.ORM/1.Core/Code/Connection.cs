namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IConnection"/>
[Cloneable<IConnection>]
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
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
    {
        Engine = source.Engine;
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        var valid = _Transaction as Transaction;
        if (valid is not null && !valid.IsDisposed)
        {
            valid.HasOpenedConnection = false;
            try { valid.Dispose(); }
            catch { }
        }

        try { if (IsOpen) Close(); } catch { }
        try { Lock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        var valid = _Transaction as Transaction;
        if (valid is not null && !valid.IsDisposed)
        {
            valid.HasOpenedConnection = false;
            try { await valid.DisposeAsync().ConfigureAwait(false); }
            catch { }
        }

        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { await Lock.DisposeAsync().ConfigureAwait(false); } catch { }
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

        var valid = _Transaction as Transaction;
        if (valid is not null && !valid.IsDisposed && valid.IsActive)
        {
            valid.HasOpenedConnection = false;
            valid.Abort();
        }

        OnClose();
    }

    /// <inheritdoc/>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await using var disp = await Lock.LockAsync().ConfigureAwait(false);
        
        var valid = _Transaction as Transaction;
        if (valid is not null && !valid.IsDisposed && valid.IsActive)
        {
            valid.HasOpenedConnection = false;
            await valid.AbortAsync().ConfigureAwait(false);
        }

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
    public ITransaction Transaction
    {
        get
        {
            if (_Transaction is null || _Transaction.IsDisposed)
            {
                _Transaction = CreateTransaction();
                if (IsDisposed || OnDisposing) _Transaction.Dispose();
            }
            return _Transaction;
        }
    }
    ITransaction? _Transaction = null;

    /// <summary>
    /// Invoked to create a new object of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();
}