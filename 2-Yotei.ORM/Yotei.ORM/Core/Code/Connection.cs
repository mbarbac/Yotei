using System.Runtime.InteropServices.Marshalling;

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

        try
        {
            if (IsTransactionActive && ReferenceEquals(this, _Transaction!.Attached))
                _Transaction.Abort();
        }
        catch { }
        try { if (IsOpen) Close(); } catch { }
        try { _Semaphore.Dispose(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try
        {
            if (IsTransactionActive && ReferenceEquals(this, _Transaction!.Attached))
                await _Transaction.AbortAsync();
        }
        catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { _Semaphore.Dispose(); } catch { }
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
            ThrowOnDisposing();

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
            ThrowOnDisposing();

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

    // ----------------------------------------------------

    readonly SemaphoreSlim _Semaphore = new(1, 1);
    ITransaction? _Transaction = null;

    bool IsTransactionActive => _Transaction != null &&
        !_Transaction.IsDisposed &&
        !_Transaction.OnDisposing;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITransaction? Transaction
    {
        // If the current one is disposed then return null.
        get
        {
            if (IsDisposed || OnDisposing) return null;

            _Semaphore.Wait();
            try
            {
                if (_Transaction != null)
                {
                    if (_Transaction.IsDisposed || _Transaction.OnDisposing)
                    {
                        _Transaction = null;
                        return null;
                    }
                }

                return _Transaction;
            }
            finally { _Semaphore.Release(); }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ITransaction StartTransaction()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        _Semaphore.Wait();
        try
        {
            if (IsTransactionActive) throw new InvalidOperationException(
                "This instance already carries an active transaction.")
                .WithData(this);

            bool opened = IsOpen;
            if (!opened) Open();

            _Transaction = OnStartTransaction();
            _Transaction.Attached = this;
            if (!opened) _Transaction.HasOpenedConnection = true;
            return _Transaction;
        }
        finally { _Semaphore.Release(); }
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

        await _Semaphore.WaitAsync(token).ConfigureAwait(false);
        try
        {
            if (IsTransactionActive) throw new InvalidOperationException(
                "This instance already carries an active transaction.")
                .WithData(this);

            bool opened = IsOpen;
            if (!opened) await OpenAsync(token).ConfigureAwait(false);

            _Transaction = await OnStartTransactionAsync(token).ConfigureAwait(false);
            _Transaction.Attached = this;
            if (!opened) _Transaction.HasOpenedConnection = true;
            return _Transaction;
        }
        finally { _Semaphore.Release(); }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="transaction"></param>
    void IConnection.EndTransaction(ITransaction transaction)
    {
        if (IsDisposed || OnDisposing) return;
        if (!ReferenceEquals(this, transaction.Attached)) return;

        _Semaphore.Wait();
        try
        {
            if (IsOpen && transaction.HasOpenedConnection) Close();

            transaction.HasOpenedConnection = false;
            transaction.Attached = null;
            _Transaction = null;
        }
        finally { _Semaphore.Release(); }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    async ValueTask IConnection.EndTransactionAsync(ITransaction transaction, CancellationToken token)
    {
        if (IsDisposed || OnDisposing) return;
        if (!ReferenceEquals(this, transaction.Attached)) return;

        await _Semaphore.WaitAsync(token).ConfigureAwait(false);
        try
        {
            if (IsOpen && transaction.HasOpenedConnection) await CloseAsync().ConfigureAwait(false);

            transaction.HasOpenedConnection = false;
            transaction.Attached = null;
            _Transaction = null;
        }
        finally { _Semaphore.Release(); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to start and return an underlying transaction.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction OnStartTransaction();

    /// <summary>
    /// Invoked to start and return an underlying transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask<ITransaction> OnStartTransactionAsync(
        CancellationToken token = default);
}