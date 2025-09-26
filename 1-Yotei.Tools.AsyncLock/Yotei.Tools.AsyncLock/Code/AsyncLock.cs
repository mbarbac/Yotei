namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides lock-alike capabilities for synchronous, asynchronous and mixed scenarios, including
/// re-entrant ones. The lock method in this type return a disposable object that, when disposed,
/// either releases the parent lock or decreases its reentrancy count.
/// </summary>
public sealed partial class AsyncLock : DisposableClass
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AsyncLock() { }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        Semaphore.Dispose();
    }

    /// <inheritdoc/>
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
        OnDispose(disposing);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public override string ToString() => $"(Thread:{ThreadId}/Async:{AsyncId}, #Count:{Count})";

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

    /// <summary>
    /// Gets or sets the arbitrary context data that will be carried by the surrogate created by
    /// the next lock operation. All lock operations clear the value of this property.
    /// </summary>
    public object? Context { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Waits until this lock is captured.
    /// </summary>
    /// <returns></returns>
    public Surrogate Lock() => Lock(Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time. Returns a
    /// disposable object that, when disposed, either releases this lock or decreases its
    /// reentrancy count.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public Surrogate Lock(TimeSpan timeout)
    {
        var item = new Surrogate(this);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.Enter(timeout);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to capture this lock observing the given cancellation token, if any. Returns a
    /// disposable object that, when disposed, either releases this lock or decreases its
    /// reentrancy count.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(
        CancellationToken token = default) => LockAsync(Timeout.InfiniteTimeSpan, token);

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time, while observing
    /// the given cancellation token, if any. Returns a disposable object that, when disposed,
    /// either releases this lock or decreases its reentrancy count.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var item = new Surrogate(this);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.EnterAsync(timeout, token);
    }
}