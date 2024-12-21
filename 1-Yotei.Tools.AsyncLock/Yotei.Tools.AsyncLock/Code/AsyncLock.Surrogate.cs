//#if DEBUG_ASYNC_LOCK && DEBUG
//#define DEBUGPRINT
//#endif

using static System.ConsoleColor;

namespace Yotei.Tools;

public partial class AsyncLock
{
    // ====================================================
    /// <summary>
    /// Represents the result of a lock operation. If the lock was captured, when instances of
    /// this class are disposed, then they either release their parent lock, or decrease their
    /// reentrancy count.
    /// </summary>
    public class Surrogate : DisposableClass
    {
        /// <summary>
        /// Initializes a new not-entered instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        internal Surrogate(AsyncLock parent, object? data)
        {
            Parent = parent.ThrowWhenNull();
            Data = data;

            OldThreadId = EnvironmentId;
            OldAsyncId = AsyncHolder.Value;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"Old:({OldThreadId}/{OldAsyncId}), Lock:{Parent}, {GetData()}";

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override void OnDispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            if (!Captured) return;

            while (true)
            {
                if (Parent.IsDisposed) return;
                if (Parent.OnDisposing) return;

                Parent.Semaphore.Wait();
                try
                {
                    Print(Gray, $"Sync disposing: {this}");

                    Parent.Count--;
                    Parent.ThreadId = Parent.Count == 0 ? 0 : OldThreadId;
                    Parent.AsyncId = Parent.Count == 0 ? 0 : OldAsyncId;
                    return;
                }
                finally
                {
                    Parent.Semaphore.Release();
                }
            }
        }

        /// <inheritdoc/>
        protected override async ValueTask OnDisposeAsync(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            if (!Captured) return;

            while (true)
            {
                if (Parent.IsDisposed) return;
                if (Parent.OnDisposing) return;

                await Parent.Semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    Print(Gray, $"Async disposing: {this}");

                    Parent.Count--;
                    Parent.ThreadId = Parent.Count == 0 ? 0 : OldThreadId;
                    Parent.AsyncId = Parent.Count == 0 ? 0 : OldAsyncId;
                    return;
                }
                finally
                {
                    Parent.Semaphore.Release();
                }
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
            var ms = timeout.ValidateTimeout();
            var ini = DateTime.UtcNow;

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
                            Print($"Sync capturing : {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Sync captured  : {this}");
                            Captured = true;
                            break;
                        }
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Sync increasing : {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Sync increased  : {this}");
                            Captured = true;
                            break;
                        }
                    }

                    if (ms >= 0)
                    {
                        var now = DateTime.UtcNow;
                        if ((now - ini) > timeout) throw new TimeoutException(
                            "Timeout has expired.")
                            .WithData(Parent, nameof(AsyncLock));
                    }
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
            var ms = timeout.ValidateTimeout();
            var ini = DateTime.UtcNow;

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
                            Print($"Async capturing: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Async captured : {this}");
                            Captured = true;
                            break;
                        }
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Async increasing: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Async increased : {this}");
                            Captured = true;
                            break;
                        }
                    }

                    if (ms >= 0)
                    {
                        var now = DateTime.UtcNow;
                        if ((now - ini) > timeout) throw new TimeoutException(
                            "Timeout has expired.")
                            .WithData(Parent, nameof(AsyncLock));
                    }
                }
                finally
                {
                    if (taken) Parent.Semaphore.Release();
                }
            }

            return this;
        }

        // ------------------------------------------------

        readonly int OldThreadId;
        readonly long OldAsyncId;

        /// <summary>
        /// The parent lock this instance refers to.
        /// </summary>
        public AsyncLock Parent { get; }

        /// <summary>
        /// Determines if this instance has captured its parent lock, or increased its reentrancy
        /// count, or not.
        /// </summary>
        public bool Captured { get; private set; }

        /// <summary>
        /// The state data passed to this instance when capturing a lock, or null if any.
        /// </summary>
        public object? Data { get; }

        string GetData() => Data is null ? string.Empty : $"Data:{Data}";

        // ------------------------------------------------

        /// <summary>
        /// Invoked to print the given message on the debug environment.
        /// </summary>
        /// <param name="message"></param>
        [Conditional("DEBUGPRINT")]
        static void Print(string message) => DebugEx.WriteLine(message);

        /// <summary>
        /// Invoked to print the given message on the debug environment.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        [Conditional("DEBUGPRINT")]
        static void Print(ConsoleColor color, string message) => DebugEx.WriteLine(color, message);
    }
}