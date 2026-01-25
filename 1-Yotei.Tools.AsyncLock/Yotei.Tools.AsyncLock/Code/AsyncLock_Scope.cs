namespace Yotei.Tools.AsyncLock;

partial class AsyncLock
{
    // ====================================================
    /// <summary>
    /// A disposable object that represents a scope for protected code execution under its parent
    /// <see cref="ExtendedLock"/> that, when disposed, either decreases its concurrenct count or
    /// releases it.
    /// </summary>
    public sealed class Scope : DisposableClass
    {
        internal readonly AsyncLock Master;
        internal readonly object? State;
        internal readonly int OldThread;
        internal readonly ulong OldAsync;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        internal Scope(AsyncLock master, object? state)
        {
            Master = master;
            State = state;
            OldThread = Environment.CurrentManagedThreadId;
            OldAsync = AsyncHolder.Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"(OldT:{OldThread}, OldA:{OldAsync}), Master:{Master}, State:{State}";

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void OnDispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;
            Master.Exit(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        protected override async ValueTask OnDisposeAsync(bool disposing)
        {
            if (IsDisposed || !disposing) return;
            await Master.ExitAsync(this).ConfigureAwait(false);
        }
    }
}