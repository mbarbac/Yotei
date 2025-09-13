
namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommandEnumerator"/>
public abstract class CommandEnumerator : DisposableClass, ICommandEnumerator
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
    {
        Command = command.ThrowWhenNull();
        CancellationToken = token;
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (!Initialized) return;
        if (!Terminated) OnReset();
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (!Initialized) return;
        if (!Terminated) await OnResetAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Enumerator({Command})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerableCommand Command { get; }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <inheritdoc/>
    public IRecord Current { get; private set; } = default!;
    object IEnumerator.Current => Current;

    /// <inheritdoc/>
    public ISchema? Schema { get; private set; }

    // ----------------------------------------------------

    bool Initialized;
    bool Terminated;
    bool OpenedByThis;
    bool TakeEmulated;
    int TakeRemaining;

    // Invoked to open the associated connection, if needed.
    void TryOpenConnection()
    {
        if (Command.Connection.IsOpen) return;

        Command.Connection.Open();
        OpenedByThis = true;
    }

    // Invoked to open the associated connection, if needed.
    async ValueTask TryOpenConnectionAsync()
    {
        if (Command.Connection.IsOpen) return;

        await Command.Connection.OpenAsync(CancellationToken).ConfigureAwait(false);
        OpenedByThis = true;
    }

    // Invoked to close the associated connection, if needed.
    void TryCloseConnection()
    {
        if (!Command.Connection.IsOpen) return;
        if (!OpenedByThis) return;

        Command.Connection.Close();
        OpenedByThis = false;
    }

    // Invoked to close the associated connection, if needed.
    async ValueTask TryCloseConnectionAsync()
    {
        if (!Command.Connection.IsOpen) return;
        if (!OpenedByThis) return;

        await Command.Connection.CloseAsync().ConfigureAwait(false);
        OpenedByThis = false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Reset() => OnReset();

    /// <summary>
    /// Invoked to reset this instance.
    /// Inheritors must call the base method.
    /// </summary>
    protected virtual void OnReset()
    {
        if (!Initialized) return;
        if (!Terminated) OnAbort();

        TryCloseConnection();
        Initialized = false;
        Terminated = false;
        OpenedByThis = false;
        TakeEmulated = false;
        TakeRemaining = 0;
    }

    /// <summary>
    /// Invoked to reset this instance.
    /// Inheritors must call the base method.
    /// </summary>
    /// <returns></returns>
    protected virtual async ValueTask OnResetAsync()
    {
        if (!Initialized) return;
        if (!Terminated) await OnAbortAsync().ConfigureAwait(false);

        await TryCloseConnectionAsync().ConfigureAwait(false);
        Initialized = false;
        Terminated = false;
        OpenedByThis = false;
        TakeEmulated = false;
        TakeRemaining = 0;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool MoveNext()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();
        CancellationToken.ThrowIfCancellationRequested();

        // Main loop...
        if (Terminated) return false;
        try
        {
            while (!Terminated)
            {
                if (!Initialize()) break;
                if (!NextElement()) break;
                return true;
            }

            if (Initialized) Terminate();
            return false;
        }
        catch
        {
            if (Initialized)
            {
                try { Abort(); }
                catch { }
            }
            throw;
        }

        /// <summary>
        /// Initializes the execution of the command...
        /// </summary>
        bool Initialize()
        {
            if (Initialized) return true;
            Initialized = true;

            TryOpenConnection();

            Schema = OnInitialize();

            var engine = Command.Connection.Engine;
            if (!engine.SupportsNativePaging || !Command.SupportsNativePaging) // Emulate paging...
            {
                var skip = Command.Skip;
                if (skip > 0)
                {
                    for (int i = 0; i < skip; i++)
                    {
                        var r = OnNextResult();
                        if (r is null) return false;
                    }
                }

                var take = Command.Take;
                if (take > 0)
                {
                    TakeEmulated = true;
                    TakeRemaining = take;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtains the next element from the execution of the command...
        /// </summary>
        bool NextElement()
        {
            Current = default!;

            if (TakeEmulated) // Emulating paging...
            {
                if (TakeRemaining > 0)
                {
                    Current = OnNextResult();
                    TakeRemaining--;
                }
            }
            else // Standard case...
            {
                Current = OnNextResult();
            }

            return Current is not null;
        }

        /// <summary>
        /// Terminates execution of the command...
        /// </summary>
        void Terminate()
        {
            if (Terminated) return;

            OnTerminate();
            TryCloseConnection();
            Terminated = true;
            Initialized = false;
        }

        /// <summary>
        /// Aborts execution of the command...
        /// </summary>
        void Abort()
        {
            if (Terminated) return;

            OnAbort();
            TryCloseConnection();
            Terminated = true;
            Initialized = false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to initialize the execution of the command. Returns the schema that describes the
    /// records to be produced, or null if this information is not available.
    /// </summary>
    /// <returns></returns>
    protected abstract ISchema? OnInitialize();

    /// <summary>
    /// Invoked to return the next result produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecord OnNextResult();

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected abstract void OnTerminate();

    /// <summary>
    /// Invoked to abort the execution of the associated command.
    /// </summary>
    protected abstract void OnAbort();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();
        CancellationToken.ThrowIfCancellationRequested();

        // Main loop...
        if (Terminated) return false;
        try
        {
            while (!Terminated)
            {
                if (!await InitializeAsync().ConfigureAwait(false)) break;
                if (!await NextElementAsync().ConfigureAwait(false)) break;
                return true;
            }

            if (Initialized) await TerminateAsync().ConfigureAwait(false);
            return false;
        }
        catch
        {
            if (Initialized)
            {
                try { await AbortAsync().ConfigureAwait(false); }
                catch { }
            }
            throw;
        }

        /// <summary>
        /// Initializes the execution of the command...
        /// </summary>
        async ValueTask<bool> InitializeAsync()
        {
            if (Initialized) return true;
            Initialized = true;

            await TryOpenConnectionAsync().ConfigureAwait(false);

            Schema = await OnInitializeAsync().ConfigureAwait(false);

            var engine = Command.Connection.Engine;
            if (!engine.SupportsNativePaging || !Command.SupportsNativePaging) // Emulate paging...
            {
                var skip = Command.Skip;
                if (skip > 0)
                {
                    for (int i = 0; i < skip; i++)
                    {
                        var r = await OnNextResultAsync().ConfigureAwait(false);
                        if (r is null) return false;
                    }
                }

                var take = Command.Take;
                if (take > 0)
                {
                    TakeEmulated = true;
                    TakeRemaining = take;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtains the next element from the execution of the command...
        /// </summary>
        async ValueTask<bool> NextElementAsync()
        {
            Current = default!;

            if (TakeEmulated) // Emulating paging...
            {
                if (TakeRemaining > 0)
                {
                    Current = await OnNextResultAsync().ConfigureAwait(false);
                    TakeRemaining--;
                }
            }
            else // Standard case...
            {
                Current = await OnNextResultAsync().ConfigureAwait(false);
            }

            return Current is not null;
        }

        /// <summary>
        /// Terminates execution of the command...
        /// </summary>
        async ValueTask TerminateAsync()
        {
            if (Terminated) return;

            await OnTerminateAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            Terminated = true;
            Initialized = false;
        }

        /// <summary>
        /// Aborts execution of the command...
        /// </summary>
        async ValueTask AbortAsync()
        {
            if (Terminated) return;

            await OnAbortAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            Terminated = true;
            Initialized = false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to initialize the execution of the command. Returns the schema that describes the
    /// records to be produced, or null if this information is not available.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<ISchema?> OnInitializeAsync();

    /// <summary>
    /// Invoked to return the next result produced by the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<IRecord> OnNextResultAsync();

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected abstract ValueTask OnTerminateAsync();

    /// <summary>
    /// Invoked to abort the execution of the associated command.
    /// </summary>
    protected abstract ValueTask OnAbortAsync();
}