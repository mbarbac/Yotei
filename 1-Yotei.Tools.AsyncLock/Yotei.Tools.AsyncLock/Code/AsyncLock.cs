using static System.ConsoleColor;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides lock-alike capabilities for synchronous, asynchronous and mixed scenarios, including
/// re-entrant ones. Its lock methods return a disposable object as the result of that operation
/// that, when disposed, either releases the parent lock or decreases its reentrancy count.
/// </summary>
public partial class AsyncLock : DisposableClass
{
    static int EnvironmentId => Environment.CurrentManagedThreadId;
    static readonly AsyncLocal<ulong> AsyncHolder = new() { Value = 0 };
    static ulong LastAsyncHolder = 0;

    readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    /// The ID of the last thread under which this lock was captured or its reentrancy count
    /// increased, or cero if it is not captured.
    /// </summary>
    public int ThreadId { get; private set; }

    /// <summary>
    /// The ID of the last async context under which this lock was captured or its reentrancy
    /// count increased, or cero if it is not captured.
    /// </summary>
    public ulong AsyncId { get; private set; }

    /// <summary>
    /// The number of re-entrant locks held by this instance, or cero if it is not captured.
    /// </summary>
    public int Count { get; private set; }

    // ------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AsyncLock() { }

    /// <inheritdoc/>
    public override string ToString() => $"(Thread:{ThreadId}/Async:{AsyncId}, #Count:{Count})";

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        Semaphore.Dispose();
    }

    /// <inheritdoc/>
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
        // Remember to validate that 'OnDispose()' can be used...
        OnDispose(disposing);
        return ValueTask.CompletedTask;
    }

    // ------------------------------------------------

    /// <summary>
    /// Waits until this lock can be captured. Returns a disposable object that, when disposed,
    /// either releases the lock or decreases its reentrancy count.
    /// <br/> The <paramref name="data"/> argument is used to pass context data to the surrogate
    /// object, typically for tracking purposes.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public Surrogate Lock(object? data = null)
    {
        return Lock(Timeout.InfiniteTimeSpan, data);
    }

    /// <summary>
    /// Tries to capture this lock waiting at most for the given amount of time. Returns a
    /// disposable object that, when disposed, it either releases the lock or decreases its
    /// reentrancy count.
    /// <br/> The <paramref name="data"/> argument is used to pass context data to the surrogate
    /// object, typically for tracking purposes.
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

    // ------------------------------------------------

    /// <summary>
    /// Waits until this lock can be captured. Returns a disposable object that, when disposed,
    /// either releases the lock or decreases its reentrancy count.
    /// <br/> The <paramref name="data"/> argument is used to pass context data to the surrogate
    /// object, typically for tracking purposes.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(object? data = null)
    {
        return LockAsync(data, CancellationToken.None);
    }

    /// <summary>
    /// Tries to capture this lock while observing the given cancellation token, if any. Returns
    /// a disposable object that, when disposed, it either releases the lock or decreases its
    /// reentrancy count.
    /// <br/> The <paramref name="data"/> argument is used to pass context data to the surrogate
    /// object, typically for tracking purposes.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(object? data = null, CancellationToken token = default)
    {
        return LockAsync(Timeout.InfiniteTimeSpan, data, token);
    }

    /// <summary>
    /// Tries to capture this lock waiting at most for the given amount of time. Returns a
    /// disposable object that, when disposed, it either releases the lock or decreases its
    /// reentrancy count.
    /// <br/> The <paramref name="data"/> argument is used to pass context data to the surrogate
    /// object, typically for tracking purposes.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(TimeSpan timeout, object? data = null)
    {
        return LockAsync(timeout, data, CancellationToken.None);
    }

    /// <summary>
    /// Tries to capture this lock waiting at most for the given amount of time, while observing
    /// the given cancellation token, if any. Returns a disposable object that, when disposed, it
    /// either releases the lock or decreases its reentrancy count.
    /// <br/> The <paramref name="data"/> argument is used to pass context data to the surrogate
    /// object, typically for tracking purposes.
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
    /// Prints the given message to the debug and console outputs.
    /// </summary>
    /// <param name="message"></param>
    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void Print(string message) => DebugEx.WriteLine(true, message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void Print(
        ConsoleColor color, string message) => DebugEx.WriteLine(true, color, message);
}