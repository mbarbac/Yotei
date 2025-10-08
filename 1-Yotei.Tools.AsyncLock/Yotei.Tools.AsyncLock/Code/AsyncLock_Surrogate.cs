namespace Yotei.Tools.AsyncLock;

partial class AsyncLock
{
    // ====================================================
    /// <summary>
    /// Represents the result of a lock operation on a <see cref="AsyncLock"/> instance that,
    /// when disposed, either releases the parent lock, or decreases its reentrancy count, as
    /// appropriate.
    /// </summary>
    public class Surrogate : DisposableClass
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="context"></param>
        internal Surrogate(AsyncLock parent, object? context)
        {
            Parent = parent.ThrowWhenNull();
            Context = context;

            OldThreadId = EnvironmentId;
            OldAsyncId = AsyncHolder.Value;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"Old:(Thread:{OldThreadId}/Async:{OldAsyncId}), Parent:{Parent} {ContextData}";

        // ------------------------------------------------

        readonly int OldThreadId;
        readonly ulong OldAsyncId;

        /// <summary>
        /// The lock this instance refers to.
        /// </summary>
        public AsyncLock Parent { get; }

        /// <summary>
        /// Indicates if the operation that created this instance entered the lock successfully,
        /// or not.
        /// </summary>
        public bool IsEntered { get; private set; }

        /// <summary>
        /// Maintains the arbitrary context information given to the operation that created this
        /// instance. The value of this property is cleared when this instance is disposed.
        /// </summary>
        public object? Context { get; private set; }
        string ContextData => Context is null ? string.Empty : $", Data: {Context}";

        // ------------------------------------------------

        /// <summary>
        /// Tries to enter the parent lock.
        /// Returns this instance so that it can be used in a fluent-syntax fashion.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        internal Surrogate Enter(TimeSpan timeout)
        {
            var ms = timeout.ValidatedTimeout;
            var ini = DateTime.UtcNow;

            while (true)
            {
                var taken = false;
                try
                {
                    // Trying to capture the semaphore...
                    taken = Parent.Semaphore.Wait(timeout);
                    if (taken)
                    {
                        // Capturing the lock...
                        if (Parent.AsyncId == 0)
                        {
                            Print($"Capturing Sync: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Captured Sync: {this}");
                            IsEntered = true;
                            break;
                        }

                        // Increasing the lock...
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Increasing Sync: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Increased Sync: {this}");
                            IsEntered = true;
                            break;
                        }
                    }

                    // Validating timeout...
                    if (ms >= 0)
                    {
                        var now = DateTime.UtcNow;
                        if ((now - ini) > timeout)
                            throw new TimeoutException("Timeout elapsed.").WithData(Parent);
                    }

                    // Give a chance to other threads...
                    if (ms < 0) Thread.Sleep(0);
                }
                finally
                {
                    // Always releasing the semaphore if needed...
                    if (taken) Parent.Semaphore.Release();
                }
            }

            return this;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override void OnDispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            Context = null;
            if (!IsEntered) return;

            while (true)
            {
                if (Parent.IsDisposed) return;
                if (Parent.OnDisposing) return;

                Parent.Semaphore.Wait();
                try
                {
                    Print(ConsoleColor.Gray, $"Disposing Sync: {this}");

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
        /// Tries to enter the parent lock.
        /// Returns this instance so that it can be used in a fluent-syntax fashion.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        internal async ValueTask<Surrogate> EnterAsync(TimeSpan timeout, CancellationToken token)
        {
            var ms = timeout.ValidatedTimeout;
            var ini = DateTime.UtcNow;

            while (true)
            {
                var taken = false;
                try
                {
                    token.ThrowIfCancellationRequested();

                    // Trying to capture the semaphore...
                    taken = await Parent.Semaphore.WaitAsync(timeout, token).ConfigureAwait(false);
                    if (taken)
                    {
                        // Capturing the lock...
                        if (Parent.AsyncId == 0)
                        {
                            Print($"Capturing Async: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Captured Async: {this}");
                            IsEntered = true;
                            break;
                        }

                        // Increasing the lock...
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Increasing Async: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Increased Async: {this}");
                            IsEntered = true;
                            break;
                        }
                    }

                    // Validating timeout...
                    if (ms >= 0)
                    {
                        var now = DateTime.UtcNow;
                        if ((now - ini) > timeout)
                            throw new TimeoutException("Timeout elapsed.").WithData(Parent);
                    }

                    // Give a chance to other threads...
                    if (ms < 0) await Task.Delay(0, token);
                }
                finally
                {
                    // Always releasing the semaphore if needed...
                    if (taken) Parent.Semaphore.Release();
                }
            }

            return this;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override async ValueTask OnDisposeAsync(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            Context = null;
            if (!IsEntered) return;

            while (true)
            {
                if (Parent.IsDisposed) return;
                if (Parent.OnDisposing) return;

                await Parent.Semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    Print(ConsoleColor.Gray, $"Disposing Async: {this}");

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

        [Conditional("DEBUG_ASYNC_LOCK")]
        public static void Print(string message) => DebugEx.WriteLine(true, message);

        [Conditional("DEBUG_ASYNC_LOCK")]
        public static void Print(
        ConsoleColor color, string message) => DebugEx.WriteLine(true, color, message);
    }
}