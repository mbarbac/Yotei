using static System.ConsoleColor;
namespace Yotei.Tools.AsyncLock;

// ========================================================
/// <summary>
/// Provides code execution protection capabilities for synchronous, asynchronous, reentrant and
/// mixed scenarios. Entering a lock obtains a scope that, when disposed, either releases the lock
/// or decreases its reentrancy count. A thread or asynchronous context holding a lock can access
/// it recursively, but must exit the lock the same number of times to fully release it.
/// </summary>
public sealed partial class AsyncLock : DisposableClass
{
#if HIDE_ASYNCHOLDER_CHANGES
    static readonly AsyncLocal<ulong> AsyncHolder = new(args =>
    {
        var old = args.PreviousValue;
        var now = args.CurrentValue;
        var changed = args.ThreadContextChanged;

        if ((old != now) && changed)
        {
            var sb = new StringBuilder();
            sb.Append($"Async Changed: {old} => {now}, Last:{LastAsyncHolder}, ");
            sb.Append($"Thread: {Environment.CurrentManagedThreadId}");
            ToDebug(DarkGray, sb.ToString());
        }
    })
#else
    static readonly AsyncLocal<ulong> AsyncHolder = new()
#endif
    { Value = 0 };

    static ulong LastAsyncHolder = 0;
    readonly SemaphoreSlim Semaphore = new(1, 1);
    int ThreadId;
    ulong AsyncId;
    int Count;

    /// <summary>
    /// The value that is used as an infinite amount of time. This value can be configured for
    /// any purposes.
    /// </summary>
    public static TimeSpan Infinite { get; set; } = Timeout.InfiniteTimeSpan;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public AsyncLock() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"(T:{ThreadId}, A:{AsyncId}, #:{Count})";

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
    protected override ValueTask OnDisposeAsync(bool disposing)
    {
        OnDispose(disposing);
        return ValueTask.CompletedTask;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Waits until this lock can be entered.
    /// Returns a disposable object that, when disposed, either releases this lock or decreases its
    /// reentrancy count.
    /// </summary>
    /// <param name="state">An arbitrary state to be kept by the scope.</param>
    /// <returns></returns>
    public Scope Enter(object? state = null) => Enter(Infinite, state);

    /// <summary>
    /// Tries to enter this lock waiting for at most the given amount of time.
    /// If entered, returns a disposable object that, when disposed, either releases this lock or
    /// decreases its reentrancy count. If not, then throws a time out exception.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="state">An arbitrary state to be kept by the scope.</param>
    /// <returns></returns>
    public Scope Enter(TimeSpan timeout, object? state = null)
    {
        var scope = new Scope(this, state); // Captures current thread and async id
        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder); // Increasing out of await
        return Enter(scope, timeout, state);
    }

    /// <summary>
    /// Invoked to enter the lock.
    /// </summary>
    Scope Enter(Scope scope, TimeSpan timeout, object? state)
    {
        var ms = timeout.ValidatedTimeout;
        var ini = DateTime.UtcNow;

        while (true)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            var taken = false;
            try
            {
                // Capturing the semaphore...
                taken = Semaphore.Wait(timeout);
                if (taken)
                {
                    if (AsyncId == 0)
                    {
                        ToDebug(Yellow, $"{scope} Capturing...");
                        ThreadId = Environment.CurrentManagedThreadId;
                        AsyncId = AsyncHolder.Value;
                        Count++;

                        ToDebug(Yellow, $"{scope} Captured...");
                        return scope;
                    }
                    if (AsyncId == scope.OldAsync)
                    {
                        ToDebug(Yellow, $"{scope} Increasing...");
                        AsyncId = AsyncHolder.Value;
                        Count++;

                        ToDebug(Yellow, $"{scope} Increased...");
                        return scope;
                    }
                }

                // Validating timeout...
                if (ms >= 0)
                {
                    var now = DateTime.UtcNow;
                    var span = now - ini;
                    if (span > timeout) throw new TimeoutException(
                        "Timeout expired.")
                        .WithData(ToString(), nameof(AsyncLock));
                }
            }
            finally { if (taken) Semaphore.Release(); }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// Invoked by the given scope to exit this lock.
    /// </summary>
    void Exit(Scope scope)
    {
        while (true)
        {
            if (IsDisposed) return;
            if (OnDisposing) return;
            if (Count == 0) return;

            var taken = false;
            try
            {
                taken = Semaphore.Wait(Infinite);
                if (taken)
                {
                    ToDebug(Blue, $"{scope} Exiting...");
                    Count--; if (Count < 0) Count = 0;
                    ThreadId = Count == 0 ? 0 : scope.OldThread;
                    AsyncId = Count == 0 ? 0 : scope.OldAsync;

                    ToDebug(Blue, $"{scope} Exited...");
                    return;
                }
            }
            finally { if (taken) Semaphore.Release(); }
            Thread.Sleep(1);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Waits until this lock can be entered while observing the given cancellation token.
    /// If entered, returns a disposable object that, when disposed, either releases this lock or
    /// decreases its reentrancy count. If not, then throws a cancellation exception.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Scope> EnterAsync(
        CancellationToken token = default) => EnterAsync(Infinite, null, token);

    /// <summary>
    /// Waits until this lock can be entered while observing the given cancellation token.
    /// If entered, returns a disposable object that, when disposed, either releases this lock or
    /// decreases its reentrancy count. If not, then throws a cancellation exception.
    /// </summary>
    /// <param name="state">An arbitrary state to be kept by the scope.</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Scope> EnterAsync(
        object? state, CancellationToken token = default) => EnterAsync(Infinite, state, token);

    /// <summary>
    /// Tries to enter this lock waiting for at most the given amount of time.
    /// If entered, returns a disposable object that, when disposed, either releases this lock or
    /// decreases its reentrancy count. If not, then throws a time out exception.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Scope> EnterAsync(
        TimeSpan timeout, CancellationToken token = default) => EnterAsync(timeout, null, token);

    /// <summary>
    /// Tries to enter this lock waiting for at most the given amount of time, while observing the
    /// given cancellation token.
    /// If entered, returns a disposable object that, when disposed, either releases this lock or
    /// decreases its reentrancy count. If not, then throws a time out exception or a cancellation
    /// one as appropriate.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="state"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Scope> EnterAsync(
        TimeSpan timeout, object? state = null, CancellationToken token = default)
    {
        var scope = new Scope(this, state); // Captures current thread and async id
        AsyncHolder.Value = Interlocked.Increment(ref LastAsyncHolder); // Increasing out of await
        return EnterAsync(scope, timeout, state, token);
    }

    /// <summary>
    /// Invoked to enter the lock.
    /// </summary>
    async ValueTask<Scope> EnterAsync(
        Scope scope, TimeSpan timeout, object? state, CancellationToken token)
    {
        var ms = timeout.ValidatedTimeout;
        var ini = DateTime.UtcNow;

        while (true)
        {
            ThrowIfDisposed();
            ThrowIfDisposing();
            token.ThrowIfCancellationRequested();

            var taken = false;
            try
            {
                // Capturing the semaphore...
                taken = await Semaphore.WaitAsync(timeout, token).ConfigureAwait(false);
                if (taken)
                {
                    if (AsyncId == 0)
                    {
                        ToDebug(Yellow, $"{scope} Capturing...");
                        ThreadId = Environment.CurrentManagedThreadId;
                        AsyncId = AsyncHolder.Value;
                        Count++;

                        ToDebug(Yellow, $"{scope} Captured...");
                        return scope;
                    }
                    if (AsyncId == scope.OldAsync)
                    {
                        ToDebug(Yellow, $"{scope} Increasing...");
                        AsyncId = AsyncHolder.Value;
                        Count++;

                        ToDebug(Yellow, $"{scope} Increased...");
                        return scope;
                    }
                }

                // Validating timeout...
                if (ms >= 0)
                {
                    var now = DateTime.UtcNow;
                    var span = now - ini;
                    if (span > timeout) throw new TimeoutException(
                        "Timeout expired.")
                        .WithData(ToString(), nameof(AsyncLock))
                        .WithData(ini)
                        .WithData(now)
                        .WithData(span)
                        .WithData(timeout.TotalMilliseconds);
                }
            }
            finally { if (taken) Semaphore.Release(); }
            await Task.Yield();
        }
    }

    /// <summary>
    /// Invoked by the given scope to exit this lock.
    /// </summary>
    async ValueTask ExitAsync(Scope scope)
    {
        while (true)
        {
            if (IsDisposed) return;
            if (OnDisposing) return;
            if (Count == 0) return;

            var taken = false;
            try
            {
                taken = await Semaphore.WaitAsync(Infinite).ConfigureAwait(false);
                if (taken)
                {
                    ToDebug(Blue, $"{scope} Exiting...");
                    Count--; if (Count < 0) Count = 0;
                    ThreadId = Count == 0 ? 0 : scope.OldThread;
                    AsyncId = Count == 0 ? 0 : scope.OldAsync;

                    ToDebug(Blue, $"{scope} Exited...");
                    return;
                }
            }
            finally { if (taken) Semaphore.Release(); }
            await Task.Yield();
        }
    }

    // ----------------------------------------------------

    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void ToDebug(string message)
    {
        Debug.WriteLine(message);
        Debug.Flush();
    }

    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void ToDebug(ConsoleColor forecolor, string message)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        ToDebug(message);
        Console.ForegroundColor = oldfore;
    }

    [Conditional("DEBUG_ASYNC_LOCK")]
    public static void ToDebug(ConsoleColor forecolor, ConsoleColor backcolor, string message)
    {
        var oldfore = Console.ForegroundColor; Console.ForegroundColor = forecolor;
        var oldback = Console.BackgroundColor; Console.BackgroundColor = backcolor;
        ToDebug(message);
        Console.BackgroundColor = oldback;
        Console.ForegroundColor = oldfore;
    }
}