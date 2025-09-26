namespace Yotei.Tools;

partial class AsyncLock
{
    // ====================================================
    /// <summary>
    /// Represents the result of a lock operation on a <see cref="AsyncLock"/> instance. If the
    /// associated lock was acquired, disposing this object will release that lock or decrease
    /// its reentrancy count. Otherwise the dispose operation is ignored.
    /// </summary>
    public sealed class Surrogate : DisposableClass
    {
        /// <summary>
        /// Initializes a not-captured instance.
        /// </summary>
        /// <param name="parent"></param>
        internal Surrogate(AsyncLock parent)
        {
            Parent = parent.ThrowWhenNull();
            Context = parent.Context;
            parent.Context = null;

            OldThreadId = EnvironmentId;
            OldAsyncId = AsyncHolder.Value;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"Old:(Thread:{OldThreadId}/Async:{OldAsyncId}), Parent:{Parent}, {GetData()}";

        // ------------------------------------------------

        readonly int OldThreadId;
        readonly ulong OldAsyncId;

        /// <summary>
        /// The lock this instance refers to.
        /// </summary>
        public AsyncLock Parent { get; }

        /// <summary>
        /// Determines if this instance represents a captured lock, or not.
        /// </summary>
        public bool Captured { get; private set; }

        /// <summary>
        /// Maintains the the arbitrary context data captured from the parent lock as the result
        /// of a lock operation.
        /// </summary>
        public object? Context { get; }

        string GetData() => Context is null ? string.Empty : $"Data: {Context}";

        // ------------------------------------------------

        /// <summary>
        /// Tries to enter the lock, either by capturin it or by increasing its reentrancy count.
        /// Returns this instance so that it can be used in a fluent syntax fashion.
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
                            Captured = true;
                            break;
                        }

                        // Increasing the lock...
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Increasing Sync: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Increased Sync: {this}");
                            Captured = true;
                            break;
                        }
                    }

                    // Validating the requested timeout, if any...
                    if (ms >= 0)
                    {
                        var now = DateTime.UtcNow;
                        if ((now - ini) > timeout) throw new TimeoutException(
                            "Timeout has expired.")
                            .WithData(Parent);
                    }

                    // Give a chance to other threads...
                    if (ms < 0) Thread.Sleep(0);
                }
                finally
                {
                    // Always release the semaphore if taken...
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
            if (!Captured) return;

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
        /// Tries to enter the lock, either by capturin it or by increasing its reentrancy count.
        /// Returns this instance so that it can be used in a fluent syntax fashion.
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
                            Captured = true;
                            break;
                        }

                        // Increasing the lock...
                        if (Parent.AsyncId == OldAsyncId)
                        {
                            Print($"Increasing Async: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Increased Async: {this}");
                            Captured = true;
                            break;
                        }
                    }

                    // Validating the requested timeout, if any...
                    if (ms >= 0)
                    {
                        var now = DateTime.UtcNow;
                        if ((now - ini) > timeout) throw new TimeoutException(
                            "Timeout has expired.")
                            .WithData(Parent);
                    }

                    // Give a chance to other threads...
                    if (ms < 0) await Task.Delay(0, token);
                }
                finally
                {
                    // Always release the semaphore if taken...
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
            if (!Captured) return;

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