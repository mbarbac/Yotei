namespace Yotei.Tools.AsyncLock;

// ========================================================
/// <summary>
/// Provides lock-alike capabilities for synchronous, asynchronous, reentrant and mixed scenarios.
/// The lock methods in this type return a disposable object that, when disposed, either releases
/// the parent lock, or decreases its reentrancy count, as appropriate.
/// </summary>
public sealed partial class AsyncLock : DisposableClass
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AsyncLock() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        Semaphore.Dispose();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
        // We must NOT run this asynchronously (ie: in the thread pool) to prevent a mismatch
        // between when the surrogate is disposed, and when this master is disposed. If that
        // would happen, we cannot guarantee the semaphore is still valid for the surrogate.
        // We also assume the master is not disposed before the surrogate, which sould be the
        // right 'using' usage pattern.

        OnDispose(disposing);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => $"(Thread:{ThreadId}/Async:{AsyncId}, #Count:{Count})";

    // ----------------------------------------------------

    static int EnvironmentId => Environment.CurrentManagedThreadId;
    static readonly AsyncLocal<ulong> AsyncHolder = new() { Value = 0 };
    static ulong LastAsyncHolder = 0;

    readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    /// The last thread under which this lock was captured or its reentrancy count increased,
    /// or cero if not captured.
    /// </summary>
    public int ThreadId { get; private set; }

    /// <summary>
    /// The last async context under which this lock was captured or its reentrancy count
    /// increased, or cero if not captured.
    /// </summary>
    public ulong AsyncId { get; private set; }

    /// <summary>
    /// The number of reentrant locks held by this instance, or cero if not captured.
    /// </summary>
    public int Count { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// Wait until the lock is entered. Returns a disposable instance that indicates if the
    /// lock has been entered or not, and that when disposed either releases the parent lock,
    /// or decreases its reentrancy count, as appropriate.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Surrogate Enter(
        object? context = null) => Enter(Timeout.InfiniteTimeSpan, context);

    /// <summary>
    /// Tries to enter the lock, waiting for the specified amount of time until the lock can be
    /// entered. Specify 'Specify Timeout.Infinite' to wait indefinitely, or 0 to not wait.
    /// Returns a disposable instance that indicates if the lock has been entered or not, and
    /// that when disposed either releases the parent lock, or decreases its reentrancy count,
    /// as appropriate.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Surrogate Enter(TimeSpan timeout, object? context = null)
    {
        var item = new Surrogate(this, context);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.Enter(timeout);
    }

    // ----------------------------------------------------
    
    /// <summary>
    /// Wait until the lock is entered. Returns a disposable instance that indicates if the
    /// lock has been entered or not, and that when disposed either releases the parent lock,
    /// or decreases its reentrancy count, as appropriate.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> EnterAsync(
        CancellationToken token = default)
        => EnterAsync(Timeout.InfiniteTimeSpan, null, token);

    /// <summary>
    /// Tries to enter the lock observing the given cancellation token. Returns a disposable
    /// instance that indicates if the lock has been entered or not, and that when disposed
    /// either releases the parent lock, or decreases its reentrancy count, as appropriate.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> EnterAsync(
        object? context, CancellationToken token = default)
        => EnterAsync(Timeout.InfiniteTimeSpan, context, token);

    /// <summary>
    /// Tries to enter the lock, waiting for the specified amount of time until the lock can be
    /// entered, while observing the given cancellation token. Specify 'Specify Timeout.Infinite'
    /// to wait indefinitely, or 0 to not wait. Returns a disposable instance that indicates if
    /// the lock has been entered or not, and that when disposed either releases the parent lock,
    /// or decreases its reentrancy count, as appropriate.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> EnterAsync(
        TimeSpan timeout, object? context = null, CancellationToken token = default)
    {
        var item = new Surrogate(this, context);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.EnterAsync(timeout, token);
    }
}