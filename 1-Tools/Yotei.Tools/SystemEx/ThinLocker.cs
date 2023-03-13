namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an object that provides lock capabilities that can be used in synchronous,
/// asynchronous, or mixed scenarios. The 'Lock' methods in this class capture a lock on this
/// class, and return a disposable object that, when disposed, releases that lock.
/// </summary>
public sealed class ThinLocker : BaseDisposable
{
    readonly SemaphoreSlim _Semaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ThinLocker() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {

        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
        throw new NotImplementedException();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Blocks the current thread until this lock can be captured. Returns a disposable object
    /// that, when disposed, releases the lock.
    /// </summary>
    /// <returns></returns>
    public IDisposable Lock()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        _Semaphore.Wait();
        return new ReleaseSurrogate(this);
    }

    /// <summary>
    /// Blocks the current thread until this lock can be captured, waiting at most for the given
    /// time interval. If captured, returns a disposable object that, when disposed, releases the
    /// lock. If the lock cannot be captured, throws an exception.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    public IDisposable Lock(int timeout)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        var taken = _Semaphore.Wait(timeout);
        return taken
            ? new ReleaseSurrogate(this)
            : throw new TimeoutException("Wait time has expired.");
    }

    /// <summary>
    /// Blocks the current thread until this lock can be captured, waiting at most for the given
    /// time interval. If captured, returns a disposable object that, when disposed, releases the
    /// lock. If the lock cannot be captured, throws an exception.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    public IDisposable Lock(TimeSpan timeout)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        var taken = _Semaphore.Wait(timeout);
        return taken
            ? new ReleaseSurrogate(this)
            : throw new TimeoutException("Wait time has expired.");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Blocks the current thread until this lock can be captured, while observing the given
    /// cancellation token. If captured, returns a disposable object that, when disposed,
    /// releases the lock. If the lock cannot be captured, throws an exception.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async ValueTask<IDisposable> LockAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ThrowOnDisposing();

        await _Semaphore.WaitAsync(token).ConfigureAwait(false);
        return new ReleaseSurrogate(this);
    }

    /// <summary>
    /// Blocks the current thread until this lock can be captured, while observing the given
    /// cancellation token, waiting at most for the given time interval. If captured, returns a
    /// disposable object that, when disposed, releases the lock. If the lock cannot be captured,
    /// throws an exception.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public async ValueTask<IDisposable> LockAsync(int timeout, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ThrowOnDisposing();

        var taken = await _Semaphore.WaitAsync(timeout, token).ConfigureAwait(false);
        return taken
            ? new ReleaseSurrogate(this)
            : throw new TimeoutException("Wait time has expired.");
    }

    /// <summary>
    /// Blocks the current thread until this lock can be captured, while observing the given
    /// cancellation token, waiting at most for the given time interval. If captured, returns a
    /// disposable object that, when disposed, releases the lock. If the lock cannot be captured,
    /// throws an exception.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public async ValueTask<IDisposable> LockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ThrowOnDisposing();

        var taken = await _Semaphore.WaitAsync(timeout, token).ConfigureAwait(false);
        return taken
            ? new ReleaseSurrogate(this)
            : throw new TimeoutException("Wait time has expired.");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to release the lock on this instance.
    /// </summary>
    private void Release()
    {
        if (IsDisposed) return;
        if (OnDisposing) return;

        _Semaphore.Release();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The disposable object that, when disposed, releases the lock on its master instance.
    /// </summary>
    struct ReleaseSurrogate : IDisposable
    {
        bool _IsDisposed = false;
        ThinLocker _Master = null!;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        internal ReleaseSurrogate(ThinLocker master) => _Master = master;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override string ToString() => nameof(ReleaseSurrogate);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            if (!_IsDisposed)
            {
                _IsDisposed = true;
                _Master.Release();
            }
        }
    }
}