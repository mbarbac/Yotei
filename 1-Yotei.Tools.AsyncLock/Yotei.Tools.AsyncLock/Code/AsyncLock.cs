namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an object that provides lock-alike capabilities for synchronous, asynchronous and
/// mixed scenarios. Its lock methods return a disposable object as the result of that operation
/// that, when disposed, either releases the parent lock or decreases its reentrancy count.
/// </summary>
public partial class AsyncLock : DisposableClass
{
    static int EnvironmentId => Environment.CurrentManagedThreadId;

    static readonly AsyncLocal<long> AsyncHolder = new() { Value = 0 };
    static long LastAsyncHolder = 0;

    readonly SemaphoreSlim Semaphore = new(1, 1);
    
    /// <summary>
    /// The thread id on which this lock was captured, or zero if it is not captured.
    /// </summary>
    public int ThreadId { get; private set; }

    /// <summary>
    /// The async context id on which this lock was captured, or zero if it is not captured.
    /// </summary>
    public long AsyncId { get; private set; }

    /// <summary>
    /// The number of reentrant locks held on this object, or zero if it is not captured.
    /// </summary>
    public int Count { get; private set; }

    // ----------------------------------------------------

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
    public override string ToString() => $"({ThreadId}/{AsyncId}, #:{Count})";

    // ----------------------------------------------------

    /// <summary>
    /// Waits until this lock can be captured. Returns a disposable object as the result of this
    /// operation that, when disposed, either releases the lock or decreases its reentrancy count.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public Surrogate Lock(object? data = null) => Lock(Timeout.InfiniteTimeSpan, data);

    /// <summary>
    /// Tries to capture this lock waiting at most for the given amount of time. Returns a
    /// disposable object that, if the lock was captured, when disposed either releases the lock
    /// or decreases its reentrancy count.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public Surrogate Lock(TimeSpan timeout, object? data = null)
    {
        var item = new Surrogate(this, data);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.Enter(timeout);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Waits until this lock can be captured. Returns a disposable object that, when disposed,
    /// either releases the lock or decreases its reentrancy count.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(object? data = null)
    {
        return LockAsync(Timeout.InfiniteTimeSpan, data, CancellationToken.None);
    }

    /// <summary>
    /// Tries to capture this lock waiting at most for the given amount of time. Returns a
    /// disposable object that, if the lock was captured, when disposed either releases the lock
    /// or decreases its reentrancy count.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(TimeSpan timeout, object? data = null)
    {
        return LockAsync(timeout, data, CancellationToken.None);
    }

    /// <summary>
    /// Tries to capture this lock observing the given cancellation token, if any. Returns a
    /// disposable object that, if the lock was captured, when disposed either releases the lock
    /// or decreases its reentrancy count.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(object? data = null, CancellationToken token = default)
    {
        return LockAsync(Timeout.InfiniteTimeSpan, data, token);
    }

    /// <summary>
    /// Tries to capture this lock waiting at most for the given amount of time, while observing
    /// the given cancellation token, if any. Returns a disposable object that, if the lock was
    /// captured, when disposed either releases the lock or decreases its reentrancy count.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(
        TimeSpan timeout, object? data = null, CancellationToken token = default)
    {
        var item = new Surrogate(this, data);

        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder);
        return item.EnterAsync(timeout, token);
    }

    // ------------------------------------------------

    /// <summary>
    /// Invoked to print the given message on the debug environment.
    /// </summary>
    /// <param name="message"></param>
    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void Print(string message) => DebugEx.WriteLine(message);

    /// <summary>
    /// Invoked to print the given message on the debug environment.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void Print(ConsoleColor color, string message) => DebugEx.WriteLine(color, message);
}