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

        if (!_Initialized) return;
        if (!_Terminated) OnAbort();
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (!_Initialized) return;
        if (!_Terminated) await OnAbortAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Enumerator({Command})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerableCommand Command { get; }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public ISchema? Schema => _Schema;
    ISchema? _Schema;

    /// <inheritdoc/>
    public IRecord? Current => _Current;
    IRecord? _Current;
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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool MoveNext()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        if (_Terminated) return false;

        // Main loop...
        try
        {
            while (!_Terminated)
            {
                if (!Initialize()) break;
                if (!NextElement()) break;
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

        // Initializes execution...
        bool Initialize()
        {
            if (_Initialized) return true;
            _Initialized = true;

            TryOpenConnection();

            _Schema = OnInitialize();

            if (!Command.SupportsNativePaging) // Emulate paging...
            {
                var skip = Command.Skip; if (skip > 0)
                {
                    for (int i = 0; i < skip; i++)
                    {
                        var r = OnNextResult();
                        if (r == null) return false;
                    }
                }
                var take = Command.Take; if (take >= 0)
                {
                    _TakeEmulated = true;
                    _TakeRemaining = take;
                }
            }

            return true;
        }

        // Gets next element, if any...
        bool NextElement()
        {
            _Current = null;

            if (_TakeEmulated) // Emulating paging...
            {
                if (_TakeRemaining > 0)
                {
                    _Current = OnNextResult();
                    _TakeRemaining--;
                }
            }
            else // Standard case...
            {
                _Current = OnNextResult();
            }

            return _Current != null;
        }

        // Terminates execution...
        void Terminate()
        {
            if (_Terminated) return;

            OnTerminate();
            TryCloseConnection();
            _Terminated = true;
            _Initialized = false;
        }

        // Aborts execution...
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
    /// Invoked to initialize the execution of the command. Returns the schema that describes the
    /// records to be produced, or null if this information is not available.
    /// </summary>
    /// <returns></returns>
    protected abstract ISchema? OnInitialize();

    /// <summary>
    /// Invoked to return the next result produced by the execution of the command, or null if no
    /// more records are available.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecord? OnNextResult();

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected abstract void OnTerminate();

    /// <summary>
    /// Invoked to abort the execution of the command.
    /// </summary>
    protected abstract void OnAbort();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        if (_Terminated) return false;

        // Main loop...
        try
        {
            while (!_Terminated)
            {
                if (!await InitializeAsync().ConfigureAwait(false)) break;
                if (!await NextElementAsync().ConfigureAwait(false)) break;
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

        // Initializes execution...
        async ValueTask<bool> InitializeAsync()
        {
            if (_Initialized) return true;
            _Initialized = true;

            await TryOpenConnectionAsync().ConfigureAwait(false);

            _Schema = await OnInitializeAsync().ConfigureAwait(false);

            if (!Command.SupportsNativePaging) // Emulate paging...
            {
                var skip = Command.Skip; if (skip > 0)
                {
                    for (int i = 0; i < skip; i++)
                    {
                        var r = await OnNextResultAsync().ConfigureAwait(false);
                        if (r == null) return false;
                    }
                }
                var take = Command.Take; if (take >= 0)
                {
                    _TakeEmulated = true;
                    _TakeRemaining = take;
                }
            }

            return true;
        }

        // Gets next element, if any...
        async ValueTask<bool> NextElementAsync()
        {
            _Current = null;

            if (_TakeEmulated) // Emulating paging...
            {
                if (_TakeRemaining > 0)
                {
                    _Current = await OnNextResultAsync().ConfigureAwait(false);
                    _TakeRemaining--;
                }
            }
            else // Standard case...
            {
                _Current = await OnNextResultAsync().ConfigureAwait(false);
            }

            return _Current != null;
        }

        // Terminates execution...
        async ValueTask TerminateAsync()
        {
            if (_Terminated) return;

            await OnTerminateAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            _Terminated = true;
            _Initialized = false;
        }

        // Aborts execution...
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
    /// Invoked to initialize the execution of the command. Returns the schema that describes the
    /// records to be produced, or null if this information is not available.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<ISchema?> OnInitializeAsync();

    /// <summary>
    /// Invoked to return the next result produced by the execution of the command, or null if no
    /// more records are available.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<IRecord?> OnNextResultAsync();

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected abstract ValueTask OnTerminateAsync();

    /// <summary>
    /// Invoked to abort the execution of the command.
    /// </summary>
    protected abstract ValueTask OnAbortAsync();

    // ----------------------------------------------------

    /// <inheritdoc/>
    void IEnumerator.Reset() => throw new NotSupportedException();
}