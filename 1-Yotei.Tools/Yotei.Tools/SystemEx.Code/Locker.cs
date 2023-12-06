#if DEBUG_LOCKER && DEBUG
#define DEBUGPRINT
# endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an object that provides lock capabilities for synchronous, asynchronous, and mixed
/// scenarios. Its family of <c>Lock...()</c> methods return a disposable object as the result of
/// the operation that, when disposed, either releases the parent lock or decreases its reentrancy
/// count.
/// </summary>
public class Locker : DisposableClass
{
    static int EnvironmentId => Environment.CurrentManagedThreadId;

    [SuppressMessage("", "IDE0044")]
    static AsyncLocal<long> AsyncHolder = new() { Value = 0 };
    static long AsyncCount = 0;

    readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Locker() { }

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({ThreadId}/{AsyncId}, #:{Count})";

    /// <summary>
    /// The thread on which this lock has been captured, or <c>zero</c> if it is not captured.
    /// </summary>
    public int ThreadId { get; private set; }

    /// <summary>
    /// The async context id on which this lock has been captured, or <c>zero</c> if it is not
    /// captured.
    /// </summary>
    public long AsyncId { get; private set; }

    /// <summary>
    /// The number of reentrant locks held on this lock, or <c>zero</c> if it is not captured.
    /// </summary>
    public int Count { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// The event invoked for debugging purposes.
    /// </summary>
    public EventHandler<string> OnDebug = null!;

    /// <summary>
    /// Prepends the given message with a header appropriate for debug purposes.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string Format(
        string message) => $"{Environment.TickCount} - [{EnvironmentId,3}]: {message}";

    // ----------------------------------------------------

    /// <summary>
    /// Captures this lock.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Surrogate Lock(object? data = null) => Lock(Timeout.InfiniteTimeSpan, data);

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public Surrogate Lock(TimeSpan timeout, object? data = null)
    {
        var item = new Surrogate(this, data);

        AsyncHolder.Value = Interlocked.Increment(ref AsyncCount);
        return item.Enter(timeout);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to capture this lock observing the given cancellation token, if any.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(
        CancellationToken token = default) => LockAsync(null, token);

    /// <summary>
    /// Tries to capture this lock observing the given cancellation token, if any.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(
        object? data, CancellationToken token = default)
        => LockAsync(Timeout.InfiniteTimeSpan, data, token);

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time, while observing
    /// the given cancellation token, if any.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(
        TimeSpan timeout, CancellationToken token = default) => LockAsync(timeout, null, token);

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time, while observing
    /// the given cancellation token, if any.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(
        TimeSpan timeout, object? data, CancellationToken token = default)
    {
        var item = new Surrogate(this, data);

        AsyncHolder.Value = Interlocked.Increment(ref AsyncCount);
        return item.EnterAsync(timeout, token);
    }

    // ====================================================
    /// <summary>
    /// Represents a captured lock. When disposed, instances of this class either release their
    /// parent lock, or decrease their reentrancy count.
    /// </summary>
    public class Surrogate : DisposableClass
    {
        readonly int OldThreadId;
        readonly long OldAsyncId;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        internal Surrogate(Locker parent, object? data)
        {
            Parent = parent;
            Data = data;
            OldThreadId = EnvironmentId;
            OldAsyncId = AsyncHolder.Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"Old:({OldThreadId}/{OldAsyncId}), Lock:{Parent}{GetData()}";

        /// <summary>
        /// The parent lock this instance was created for.
        /// </summary>
        public Locker Parent { get; private set; }

        /// <summary>
        /// The state date passed to this object when trying to obtain a lock.
        /// </summary>
        public object? Data { get; private set; }
        string GetData() => Data == null ? string.Empty : $", Data:{Data}";

        // ------------------------------------------------

        /// <summary>
        /// Invoked when trying to enter the lock.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        internal Surrogate Enter(TimeSpan timeout)
        {
            var ini = DateTime.Now;

            while (true)
            {
                var taken = false;
                try
                {
                    taken = Parent.Semaphore.Wait(timeout);
                    if (taken)
                    {
                        if (Parent.AsyncId == 0)
                        {
                            Print($"Sync Capturing: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Sync Captured:  {this}");
                            break;
                        }
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Sync Increasing: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Sync Increased:  {this}");
                            break;
                        }
                    }
                    ValidateExpiration(ini, timeout);
                }
                finally
                {
                    if (taken) Parent.Semaphore.Release();
                }
            }

            return this;
        }

        /// <summary>
        /// Invoked when trying to enter the lock.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        internal async ValueTask<Surrogate> EnterAsync(TimeSpan timeout, CancellationToken token)
        {
            var ini = DateTime.Now;

            while (true)
            {
                var taken = false;
                try
                {
                    taken = await Parent.Semaphore.WaitAsync(timeout, token).ConfigureAwait(false);
                    if (taken)
                    {
                        if (Parent.AsyncId == 0)
                        {
                            Print($"Async Capturing: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Async Captured:  {this}");
                            break;
                        }
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Async Increasing: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Async Increased:  {this}");
                            break;
                        }
                    }
                    ValidateExpiration(ini, timeout);
                }
                finally
                {
                    if (taken) Parent.Semaphore.Release();
                }
            }

            return this;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void OnDispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            while (true)
            {
                if (Parent.IsDisposed) return;
                if (Parent.OnDisposing) return;

                Parent.Semaphore.Wait();
                try
                {
                    Print($"Sync Disposing: {this}");

                    Parent.Count--;
                    Parent.ThreadId = Parent.Count == 0 ? 0 : OldThreadId;
                    Parent.AsyncId = Parent.Count == 0 ? 0 : OldAsyncId;
                    return;
                }
                finally { Parent.Semaphore.Release(); }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        protected override async ValueTask OnDisposeAsync(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            while (true)
            {
                if (Parent.IsDisposed) return;
                if (Parent.OnDisposing) return;

                await Parent.Semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    Print($"Async Disposing: {this}");

                    Parent.Count--;
                    Parent.ThreadId = Parent.Count == 0 ? 0 : OldThreadId;
                    Parent.AsyncId = Parent.Count == 0 ? 0 : OldAsyncId;
                    return;
                }
                finally { Parent.Semaphore.Release(); }
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Validates that the timeout period has not expired, or throws an exception otherwise.
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="timeout"></param>
        void ValidateExpiration(DateTime ini, TimeSpan timeout)
        {
            if (timeout != Timeout.InfiniteTimeSpan)
            {
                if ((DateTime.Now - ini) > timeout)
                    throw new TimeoutException("Timeout has expired.")
                    .WithData(Parent, nameof(Locker));
            }
        }

        /// <summary>
        /// Prints the given debug message if such is enabled.
        /// </summary>
        /// <param name="message"></param>
        [Conditional("DEBUGPRINT")]
        void Print(string message)
        {
            var handler = Parent.OnDebug;
            handler?.Invoke(Parent, Format(message));
        }
    }
}