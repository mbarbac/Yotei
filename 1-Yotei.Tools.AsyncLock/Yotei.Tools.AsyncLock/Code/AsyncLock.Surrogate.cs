using static System.ConsoleColor;

namespace Yotei.Tools;

public partial class AsyncLock
{
    // ====================================================
    /// <summary>
    /// Represents the result of an <see cref="AsyncLock"/>'s lock operation. If the lock was
    /// acquired, then disposing this object will release the lock or decrease its reentrancy
    /// count. Otherwise, the dispose operation will do nothing.
    /// </summary>
    public class Surrogate : DisposableClass
    {
        /// <summary>
        /// Initializes a new not-captured instance.
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

        /// <summary>
        /// The lock this instance refers to.
        /// </summary>
        public AsyncLock Parent { get; }

        /// <summary>
        /// Determines if this instance represents a captured lock, or not.
        /// </summary>
        public bool Captured { get; private set; }

        /// <summary>
        /// Represents context data passed to this instance when trying to capture the lock, or
        /// null if any is given.
        /// </summary>
        public object? Data { get; }

        string GetData() => Data is null ? string.Empty : $"Data:{Data}";

        readonly int OldThreadId;
        readonly ulong OldAsyncId;

        // ------------------------------------------------

        /// <summary>
        /// Invoked when trying to enter the lock, capturing it or increasing its reentrancy count.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        internal Surrogate Enter(TimeSpan timeout)
        {
            var ms = timeout.ValidateTimeout();
            var ini = DateTime.UtcNow;

            while (true)
            {
                var taken = false; // Initializing status...

                try // Trying to capture the semaphore...
                {
                    taken = Parent.Semaphore.Wait(timeout);
                    if (taken)
                    {
                        if (Parent.AsyncId == 0) // Capturing the lock...
                        {
                            Print($"Capturing Async: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Captured Async: {this}");
                            Captured = true;
                            break;
                        }

                        if (Parent.AsyncId == OldAsyncId) // Increasing the lock...
                        {
                            Print($"Increasing Async: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Increased Async: {this}");
                            Captured = true;
                            break;
                        }
                    }

                    if (ms >= 0) // Let's wait the requested timeout, if needed...
                    {
                        var now = DateTime.UtcNow;

                        if ((now - ini) > timeout) throw new TimeoutException(
                            "Timeout has expired.")
                            .WithData(Parent, nameof(AsyncLock));
                    }
                }
                finally // Always releasing the semaphore if taken....
                {
                    if (taken) Parent.Semaphore.Release();
                }
            }

            return this;
        }

        /// <summary>
        /// Invoked when trying to enter the lock, capturing it or increasing its reentrancy count.
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
                var taken = false; // Initializing status...

                try // Trying to capture the semaphore...
                {
                    taken = await Parent.Semaphore.WaitAsync(timeout, token).ConfigureAwait(false);
                    if (taken)
                    {
                        if (Parent.AsyncId == 0) // Capturing the lock...
                        {
                            Print($"Capturing Async: {this}");
                            Parent.ThreadId = EnvironmentId;
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Captured Async: {this}");
                            Captured = true;
                            break;
                        }

                        else if (Parent.AsyncId == OldAsyncId) // Increasing the lock...
                        {
                            Print($"Increasing Async: {this}");
                            Parent.AsyncId = AsyncHolder.Value;
                            Parent.Count++;

                            Print($"Increased Async: {this}");
                            Captured = true;
                            break;
                        }

                        else // Cannot handle any other situation...
                        {
                            throw new UnExpectedException(
                                "Unknown situation.")
                                .WithData(Parent, nameof(AsyncLock));
                        }
                    }

                    if (ms >= 0) // Let's wait the requested timeout, if needed...
                    {
                        var now = DateTime.UtcNow;

                        if ((now-ini) > timeout) throw new TimeoutException(
                            "Timeout has expired.")
                            .WithData(Parent, nameof(AsyncLock));
                    }
                }
                finally // Always releasing the semaphore if taken....
                {
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
                    Print(Gray, $"Disposing Async: {this}");

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
                    Print(Gray, $"Disposing Async: {this}");

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
    }
}