namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandEnumerator"/>
/// </summary>
public abstract class CommandEnumerator : DisposableClass, ICommandEnumerator
{
    bool _WithSchema, _OpenedByThis;
    bool _Initialized, _Terminated;
    bool _TakeEmulated; int _TakeRemaining;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        Command = command;
        CancellationToken = token;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || disposing) return;

        if (!_Initialized) return;
        if (!_Terminated) Reset();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || disposing) return;

        if (!_Initialized) return;
        if (!_Terminated) await ResetAsync().ConfigureAwait(false);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerableCommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IRecord? Current { get; private set; }
    object IEnumerator.Current => Current!;

    /// <summary>
    /// The schema carried by the records produced by this instance, if any.
    /// </summary>
    public ISchema? Schema { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ICommandEnumerator WithSchema(bool value)
    {
        _WithSchema = value;
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public void WithSchema(out bool value) { value = _WithSchema; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> It is expected inheritors to invoke this base method AFTER they have finished with
    /// their own reset operations.
    /// </summary>
    public virtual void Reset()
    {
        if (_Initialized) Abort();

        TryCloseConnection();
        _Initialized = _Terminated = false;
        _TakeEmulated = false; _TakeRemaining = 0;
    }

    /// <summary>
    /// <inheritdoc cref="Reset"/>
    /// </summary>
    /// <returns></returns>
    public virtual async ValueTask ResetAsync()
    {
        if (_Initialized) await AbortAsync().ConfigureAwait(false);

        await TryCloseConnectionAsync().ConfigureAwait(false);
        _Initialized = _Terminated = false;
        _TakeEmulated = false; _TakeRemaining = 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        if (CancellationToken.IsCancellationRequested) Terminate();
        try
        {
            // Main loop...
            while (!_Terminated)
            {
                if (!Initialize()) break;
                if (!NextElement()) break;
                return true;
            }

            // Termination cycle...
            if (_Initialized) Terminate();
            return false;
        }
        catch
        {
            if (_Initialized)
            {
                try { Abort(); }
                catch { }
            }
            throw;
        }
    }

    /// <summary>
    /// Invoked to perform initialization actions.
    /// </summary>
    bool Initialize()
    {
        if (_Initialized) return true;
        _Initialized = true;

        // Opening connection and executing...
        TryOpenConnection();
        Schema = OnInitialize(_WithSchema);

        // Emulating paging if needed...
        var engine = Command.Connection.Engine;
        if (!engine.NativePaging || !Command.SupportsNativePaging)
        {
            Command.Skip(out var skip);
            Command.Take(out var take);

            if (skip > 0)
            {
                for (int i = 0; i < skip; i++)
                {
                    var record = OnNextElement();
                    if (record is null) return false;
                }
            }
            if (take > 0)
            {
                _TakeEmulated = true;
                _TakeRemaining = take;
            }
        }

        // Finishing...
        return true;
    }

    /// <summary>
    /// Invoked to obtain the next element.
    /// </summary>
    bool NextElement()
    {
        if (!_Initialized) return false;

        // Always to begin with a cleared state whatever it happens next...
        Current = default;

        // Emulating paging if needed...
        if (_TakeEmulated)
        {
            if (_TakeRemaining > 0)
            {
                Current = OnNextElement();
                _TakeRemaining--;
            }
        }

        // Standard case
        else
        {
            Current = OnNextElement();
        }

        // Finishing...
        return Current is not null;
    }

    /// <summary>
    /// Invoked to perform termination actions.
    /// </summary>
    void Terminate()
    {
        if (_Terminated) return;

        OnTerminate();
        TryCloseConnection();
        _Terminated = true;
        _Initialized = false;
        _TakeEmulated = false;
        _TakeRemaining = 0;
    }

    /// <summary>
    /// Invoked to perform abort actions.
    /// </summary>
    void Abort()
    {
        if (_Terminated) return;

        OnAbort();
        TryCloseConnection();
        _Terminated = true;
        _Initialized = false;
        _TakeEmulated = false;
        _TakeRemaining = 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to perform initialization actions. If schema is requested, then this method must
    /// return a valid one. Otherwise, it must return <see langword="null"/>.
    /// </summary>
    /// <param name="withSchema"></param>
    /// <returns></returns>
    protected abstract ISchema? OnInitialize(bool withSchema);

    /// <summary>
    /// Invoked to return the next record produced by the execution of the command, or null if
    /// no more are available. In addition, if <see cref="Schema"/> is not null, it is expected
    /// that it is used to produce the returned record.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecord? OnNextElement();

    /// <summary>
    /// Invoked to perform termination actions.
    /// </summary>
    protected abstract void OnTerminate();

    /// <summary>
    /// Invoked to perform abort actions.
    /// </summary>
    protected abstract void OnAbort();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        if (CancellationToken.IsCancellationRequested) await TerminateAsync().ConfigureAwait(false);
        try
        {
            // Main loop...
            while (!_Terminated)
            {
                if (!await InitializeAsync().ConfigureAwait(false)) break;
                if (!await NextElementAsync().ConfigureAwait(false)) break;
                return true;
            }

            // Termination cycle...
            if (_Initialized) await TerminateAsync().ConfigureAwait(false);
            return false;
        }
        catch
        {
            if (_Initialized)
            {
                try { await AbortAsync().ConfigureAwait(false); }
                catch { }
            }
            throw;
        }
    }

    /// <summary>
    /// Invoked to perform initialization actions.
    /// </summary>
    async ValueTask<bool> InitializeAsync()
    {
        if (_Initialized) return true;
        _Initialized = true;

        // Opening connection and executing...
        await TryOpenConnectionAsync().ConfigureAwait(false);
        Schema = await OnInitializeAsync(_WithSchema).ConfigureAwait(false);

        // Emulating paging if needed...
        var engine = Command.Connection.Engine;
        if (!engine.NativePaging || !Command.SupportsNativePaging)
        {
            Command.Skip(out var skip);
            Command.Take(out var take);

            if (skip > 0)
            {
                for (int i = 0; i < skip; i++)
                {
                    var record = await OnNextElementAsync().ConfigureAwait(false);
                    if (record is null) return false;
                }
            }
            if (take > 0)
            {
                _TakeEmulated = true;
                _TakeRemaining = take;
            }
        }

        // Finishing...
        return true;
    }

    /// <summary>
    /// Invoked to obtain the next element.
    /// </summary>
    async ValueTask<bool> NextElementAsync()
    {
        if (!_Initialized) return false;

        // Always to begin with a cleared state whatever it happens next...
        Current = default;

        // Emulating paging if needed...
        if (_TakeEmulated)
        {
            if (_TakeRemaining > 0)
            {
                Current = await OnNextElementAsync().ConfigureAwait(false);
                _TakeRemaining--;
            }
        }

        // Standard case
        else
        {
            Current = await OnNextElementAsync().ConfigureAwait(false);
        }

        // Finishing...
        return Current is not null;
    }

    /// <summary>
    /// Invoked to perform termination actions.
    /// </summary>
    async ValueTask TerminateAsync()
    {
        if (_Terminated) return;

        await OnTerminateAsync().ConfigureAwait(false);
        await TryCloseConnectionAsync().ConfigureAwait(false);
        _Terminated = true;
        _Initialized = false;
        _TakeEmulated = false;
        _TakeRemaining = 0;
    }

    /// <summary>
    /// Invoked to perform abort actions.
    /// </summary>
    async ValueTask AbortAsync()
    {
        if (_Terminated) return;

        await OnAbortAsync().ConfigureAwait(false);
        await TryCloseConnectionAsync().ConfigureAwait(false);
        _Terminated = true;
        _Initialized = false;
        _TakeEmulated = false;
        _TakeRemaining = 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to perform initialization actions. If schema is requested, then this method must
    /// return a valid one. Otherwise, it must return <see langword="null"/>.
    /// </summary>
    /// <param name="withSchema"></param>
    /// <returns></returns>
    protected abstract ValueTask<ISchema?> OnInitializeAsync(bool withSchema);

    /// <summary>
    /// Invoked to return the next record produced by the execution of the command, or null if
    /// no more are available. In addition, if <see cref="Schema"/> is not null, it is expected
    /// that it is used to produce the returned record.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<IRecord?> OnNextElementAsync();

    /// <summary>
    /// Invoked to perform termination actions.
    /// </summary>
    protected abstract ValueTask OnTerminateAsync();

    /// <summary>
    /// Invoked to perform abort actions.
    /// </summary>
    protected abstract ValueTask OnAbortAsync();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to open the associated connection, if needed.
    /// </summary>
    void TryOpenConnection()
    {
        if (Command.Connection.IsOpen) return;

        Command.Connection.Open();
        _OpenedByThis = true;
    }

    /// <summary>
    /// Invoked to open the associated connection, if needed.
    /// </summary>
    async ValueTask TryOpenConnectionAsync()
    {
        if (Command.Connection.IsOpen) return;

        await Command.Connection.OpenAsync(CancellationToken).ConfigureAwait(false);
        _OpenedByThis = true;
    }

    /// <summary>
    /// Invoked to close the associated connection, if possible.
    /// </summary>
    void TryCloseConnection()
    {
        if (!Command.Connection.IsOpen) return;

        if (_OpenedByThis)
        {
            Command.Connection.Close();
            _OpenedByThis = false;
        }
    }

    /// <summary>
    /// Invoked to close the associated connection, if possible.
    /// </summary>
    async ValueTask TryCloseConnectionAsync()
    {
        if (!Command.Connection.IsOpen) return;

        if (_OpenedByThis)
        {
            await Command.Connection.CloseAsync().ConfigureAwait(false);
            _OpenedByThis = false;
        }
    }
}