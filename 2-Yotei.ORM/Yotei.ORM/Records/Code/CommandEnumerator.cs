namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandEnumerator"/>
/// </summary>
public abstract class CommandEnumerator : DisposableClass, ICommandEnumerator
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    [SuppressMessage("", "IDE0290")]
    public CommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
    {
        Command = command.ThrowWhenNull();
        CancellationToken = token;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        if (!_Initialized) return;

        if (!_Terminated) OnAbort();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        if (!_Initialized) return;

        if (!_Terminated) await OnAbortAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Enumerator({Command})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerableCommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ISchema? Schema { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IRecord? Current { get; private set; }
    object? IEnumerator.Current => Current;

    // ----------------------------------------------------

    bool _Initialized;
    bool _Terminated;
    bool _OpenedByThis;
    bool _TakeEmulated;
    int _TakeRemaining;

    void TryOpenConnection()
    {
        if (Command.Connection.IsOpen) return;

        Command.Connection.Open();
        _OpenedByThis = true;
    }

    async ValueTask TryOpenConnectionAsync()
    {
        if (Command.Connection.IsOpen) return;

        await Command.Connection.OpenAsync(CancellationToken).ConfigureAwait(false);
        _OpenedByThis = true;
    }

    void TryCloseConnection()
    {
        if (!Command.Connection.IsOpen) return;
        if (!_OpenedByThis) return;

        Command.Connection.Close();
        _OpenedByThis = false;
    }

    async ValueTask TryCloseConnectionAsync()
    {
        if (!Command.Connection.IsOpen) return;
        if (!_OpenedByThis) return;

        await Command.Connection.CloseAsync().ConfigureAwait(false);
        _OpenedByThis = false;
    }

    /// <inheritdoc/>
    void IEnumerator.Reset() => throw new NotSupportedException();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        if (_Terminated) return false;
        try
        {
            while (!_Terminated)
            {
                if (!Initialize()) break;
                if (!NextResult()) break;
                return true;
            }
            if (_Initialized) Terminate();
            return false;
        }
        catch
        {
            if (_Initialized) Abort();
            throw;
        }

        /// <summary> Initializes the execution of the command.
        /// </summary>
        bool Initialize()
        {
            if (_Initialized) return true;
            _Initialized = true;

            TryOpenConnection();

            Schema = OnInitialize();
            if (Schema == null) throw new UnExpectedException(
                "Cannot obtain a schema while initializing the command execution.")
                .WithData(this);

            if (Command is IPagedCommand paged) // We may need to emulate paging...
            {
                if (!paged.NativePaging)
                {
                    var skip = paged.Skip; for (int i = 0; i < skip; i++)
                    {
                        var r = OnNextResult();
                        if (r == null) return false;
                    }
                    var take = paged.Take; if (take > 0)
                    {
                        _TakeEmulated = true;
                        _TakeRemaining = take;
                    }
                }
            }

            return true;
        }

        /// <summary> Gets the next available record.
        /// </summary>
        bool NextResult()
        {
            Current = null;

            if (_TakeEmulated) // Emulating paging...
            {
                if (_TakeRemaining > 0)
                {
                    Current = OnNextResult();
                    _TakeRemaining--;
                }
            }
            else // Standard case...
            {
                Current = OnNextResult();
            }

            return Current != null;
        }

        /// <summary> Terminates the execution.
        /// </summary>
        void Terminate()
        {
            if (_Terminated) return;

            OnTerminate();
            TryCloseConnection();
            _Terminated = true;
            _Initialized = false;
        }

        /// <summary> Terminates the execution.
        /// </summary>
        void Abort()
        {
            if (_Terminated) return;

            OnAbort();
            TryCloseConnection();
            _Terminated = true;
            _Initialized = false;
        }
    }

    /// <summary>
    /// Initializes the execution of the associated command.
    /// </summary>
    /// <returns></returns>
    protected abstract ISchema OnInitialize();

    /// <summary>
    /// Returns the next record produced by the execution of the associated command, or null.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecord? OnNextResult();

    /// <summary>
    /// Terminates the execution of the associated command.
    /// </summary>
    protected abstract void OnTerminate();

    /// <summary>
    /// Aborts the execution of the associated command.
    /// </summary>
    protected abstract void OnAbort();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        if (_Terminated) return false;
        try
        {
            while (!_Terminated)
            {
                if (!await InitializeAsync().ConfigureAwait(false)) break;
                if (!await NextResultAsync().ConfigureAwait(false)) break;
                return true;
            }
            if (_Initialized) await TerminateAsync().ConfigureAwait(false);
            return false;
        }
        catch
        {
            if (_Initialized) await AbortAsync().ConfigureAwait(false);
            throw;
        }

        /// <summary> Initializes the execution of the command.
        /// </summary>
        async ValueTask<bool> InitializeAsync()
        {
            if (_Initialized) return true;
            _Initialized = true;

            await TryOpenConnectionAsync().ConfigureAwait(false);

            Schema = await OnInitializeAsync().ConfigureAwait(false);

            if (Schema == null) throw new UnExpectedException(
                "Cannot obtain a schema while initializing the command execution.")
                .WithData(this);

            if (Command is IPagedCommand paged) // We may need to emulate paging...
            {
                if (!paged.NativePaging)
                {
                    var skip = paged.Skip; for (int i = 0; i < skip; i++)
                    {
                        var r = await OnNextResultAsync().ConfigureAwait(false);
                        if (r == null) return false;
                    }
                    var take = paged.Take; if (take > 0)
                    {
                        _TakeEmulated = true;
                        _TakeRemaining = take;
                    }
                }
            }

            return true;
        }

        /// <summary> Gets the next available record.
        /// </summary>
        async ValueTask<bool> NextResultAsync()
        {
            Current = null;

            if (_TakeEmulated) // Emulating paging...
            {
                if (_TakeRemaining > 0)
                {
                    Current = await OnNextResultAsync().ConfigureAwait(false);
                    _TakeRemaining--;
                }
            }
            else // Standard case...
            {
                Current = await OnNextResultAsync().ConfigureAwait(false);
            }

            return Current != null;
        }

        /// <summary> Terminates the execution.
        /// </summary>
        async ValueTask TerminateAsync()
        {
            if (_Terminated) return;

            await OnTerminateAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);

            _Terminated = true;
            _Initialized = false;
        }

        /// <summary> Terminates the execution.
        /// </summary>
        async ValueTask AbortAsync()
        {
            if (_Terminated) return;

            await OnAbortAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            
            _Terminated = true;
            _Initialized = false;
        }
    }

    /// <summary>
    /// Initializes the execution of the associated command.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<ISchema> OnInitializeAsync();

    /// <summary>
    /// Returns the next record produced by the execution of the associated command, or null.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<IRecord?> OnNextResultAsync();

    /// <summary>
    /// Terminates the execution of the associated command.
    /// </summary>
    protected abstract ValueTask OnTerminateAsync();

    /// <summary>
    /// Aborts the execution of the associated command.
    /// </summary>
    protected abstract ValueTask OnAbortAsync();
}