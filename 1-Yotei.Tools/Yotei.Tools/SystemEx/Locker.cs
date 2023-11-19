#if DEBUG_LOCKER && DEBUG
#define DEBUGPRINT
#endif

using Color = System.ConsoleColor;
using Console = Yotei.Tools.Diagnostics.ConsoleWrapper;
using Debug = Yotei.Tools.Diagnostics.DebugWrapper;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represent lock capabilities for mixed synchronous, asynchronous, and mixed scenarios. The
/// <c>Lock</c> family of methods return, as the result of their operation, a disposable object
/// that, when disposed, either release its parent lock or decrease its reentrancy count.
/// </summary>
public class Locker : DisposableClass
{
    static int EnvironmentId => Environment.CurrentManagedThreadId;
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
    /// <returns></returns>
    public override string ToString() => $"({ThreadId}/{AsyncId}, #:{Count})";

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
    /// Captures this lock. The returned disposable object carries the given state.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Surrogate Lock(object? data = null)
    {
        return Lock(null, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public Surrogate Lock(TimeSpan timeout)
    {
        return Lock(null, timeout);
    }

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time. The returned
    /// disposable object carries the given state.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public Surrogate Lock(object? data, TimeSpan timeout)
    {
        var item = new Surrogate(this, data);

        AsyncHolder.Value = Interlocked.Increment(ref AsyncCount);
        return item.Enter(timeout);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to capture this lock.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(CancellationToken token = default)
    {
        return LockAsync(null, Timeout.InfiniteTimeSpan, token);
    }

    /// <summary>
    /// Tries to capture this lock while observing the given cancellation token. The returned
    /// disposable object carries the given state.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(object? data, CancellationToken token = default)
    {
        return LockAsync(data, Timeout.InfiniteTimeSpan, token);
    }

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time, while observing
    /// the given cancellation token.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        return LockAsync(null, timeout, token);
    }

    /// <summary>
    /// Tries to capture this lock waiting for at most the given amount of time, while observing
    /// the given cancellation token. The returned disposable object carries the given state.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ValueTask<Surrogate> LockAsync(object? data, TimeSpan timeout, CancellationToken token = default)
    {
        var item = new Surrogate(this, data);

        AsyncHolder.Value = Interlocked.Increment(ref AsyncCount);
        return item.EnterAsync(timeout, token);
    }

    // ====================================================
    /// <summary>
    /// Represents a captured lock. When instances of this class are disposed, they either
    /// release its parent lock, or decreases its reentrancy count.
    /// </summary>
    public class Surrogate : DisposableClass
    {
        readonly int OldThreadId;
        readonly long OldAsyncId;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="parent"></param>
        internal Surrogate(Locker parent, object? data = null)
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
        /// Validates the timeout has not expired, or throws an exception otherwise.
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="timeout"></param>
        void ValidateTimeout(DateTime ini, TimeSpan timeout)
        {
            if (timeout != Timeout.InfiniteTimeSpan)
            {
                if ((DateTime.Now - ini) > timeout)
                    throw new TimeoutException("Timeout has expired")
                        .WithData(Parent, nameof(Locker));
            }
        }

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
                            OnDebug(Color.Cyan, $"Sync Capturing: {this}");

                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            OnDebug(Color.Cyan, $"Sync Captured: {this}");
                            break;
                        }
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            OnDebug(Color.Blue, $"Sync Increasing: {this}");

                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            OnDebug(Color.Blue, $"Sync Increased: {this}");
                            break;
                        }
                    }
                    ValidateTimeout(ini, timeout);
                }
                finally { if (taken) Parent.Semaphore.Release(); }
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
                            OnDebug(Color.Cyan, $"Async Capturing: {this}");

                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            OnDebug(Color.Cyan, $"Async Captured: {this}");
                            break;
                        }
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            OnDebug(Color.Blue, $"Async Increasing: {this}");

                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            OnDebug(Color.Blue, $"Async Increased: {this}");
                            break;
                        }
                    }
                    ValidateTimeout(ini, timeout);
                }
                finally { if (taken) Parent.Semaphore.Release(); }
            }

            Parent.ThreadId = EnvironmentId;
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
                    OnDebug(Color.DarkMagenta, $"Sync disposing: {this}");

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
                    OnDebug(Color.DarkMagenta, $"Sync disposing: {this}");

                    Parent.Count--;
                    Parent.ThreadId = Parent.Count == 0 ? 0 : OldThreadId;
                    Parent.AsyncId = Parent.Count == 0 ? 0 : OldAsyncId;
                    return;
                }
                finally { Parent.Semaphore.Release(); }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints a debug message.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    [Conditional("DEBUGPRINT")]
    internal static void OnDebug(
        Color color, string message) => Debug.WriteLine(color, Format(message));

    /// <summary>
    /// Prints the given message in both the console and in the debug environment (if it is
    /// enabled).
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    internal static void Print(
        Color color, string message) => Console.WriteLine(true, color, Format(message));

    /// <summary>
    /// Returns a string suitable for informational purposes.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string Format(
        string message) => $"{Environment.TickCount}  - [{EnvironmentId,3}]: {message}";
}