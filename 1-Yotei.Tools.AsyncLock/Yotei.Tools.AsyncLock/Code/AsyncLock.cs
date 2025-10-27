namespace Yotei.Tools.AsyncLock;

// ========================================================
/// <summary>
/// Represents an object that provides lock-alike capabilities for synchronous, asynchronous,
/// reentrant and mixed scenarios. The lock methods in this type return a disposable object that,
/// when disposed, either releases the parent lock, or decreases its reentrancy count, as
/// appropriate.
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
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        Semaphore.Dispose();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    /// We MUST NOT run this method asynchronously (ie: in the thread pool) to prevent a mismatch
    /// between when the surrogate is disposed, and when this master is disposed. If that would
    /// happen, we cannot guarantee the semaphore is still valid for the surrogate. We also assume
    /// the master is not disposed before the surrogate, which should be the right 'using' usage
    /// pattern.
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
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
    /// Tries to enter this lock. Returns a disposable object that indicates whether the lock has
    /// been entered or not and, if so, when disposed either releases the lock or decreases its
    /// reentrancy count, as appropriate.
    /// <br/> This method accepts an optional 'state' argument that will be captured by the 
    /// returned instance, and that can be used for arbitrary purposes.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public Surrogate Enter(
        object? state = null) => Enter(Timeout.InfiniteTimeSpan, state);

    /// <summary>
    /// Tries to enter this lock waiting for the given amount of time. Returns a disposable object
    /// that indicates whether the lock has been entered or not and, if so, when disposed either
    /// releases the lock or decreases its reentrancy count, as appropriate.
    /// <br/> This method accepts an optional 'state' argument that will be captured by the 
    /// returned instance, and that can be used for arbitrary purposes.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public Surrogate Enter(TimeSpan timeout, object? state = null)
    {
        var item = new Surrogate(this, state);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.Enter(timeout);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to enter this lock, waiting for the given amount of time while observing the given
    /// cancellation token. Returns a disposable object that indicates whether the lock has been
    /// entered or not and, if so, when disposed either releases the lock or decreases its
    /// reentrancy count, as appropriate.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> EnterAsync(
        CancellationToken token = default) => EnterAsync(Timeout.InfiniteTimeSpan, null, token);

    /// <summary>
    /// Tries to enter this lock observing the given cancellation token. Returns a disposable
    /// object that indicates whether the lock has been entered or not and, if so, when disposed
    /// either releases the lock or decreases its reentrancy count, as appropriate.
    /// <br/> This method accepts an optional 'state' argument that will be captured by the 
    /// returned instance, and that can be used for arbitrary purposes.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> EnterAsync(
        object? state, CancellationToken token = default)
        => EnterAsync(Timeout.InfiniteTimeSpan, state, token);

    /// <summary>
    /// Tries to enter this lock waiting for the given amount of time while observing the given
    /// cancellation token. Returns a disposable object that indicates whether the lock has been
    /// entered or not and, if so, when disposed either releases the lock or decreases its
    /// reentrancy count, as appropriate.
    /// <br/> This method accepts an optional 'state' argument that will be captured by the 
    /// returned instance, and that can be used for arbitrary purposes.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="state"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> EnterAsync(
        TimeSpan timeout, object? state = null, CancellationToken token = default)
    {
        var item = new Surrogate(this, state);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.EnterAsync(timeout, token);
    }
}