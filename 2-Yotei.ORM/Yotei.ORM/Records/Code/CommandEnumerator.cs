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
    public CommandEnumerator(ICommand command, CancellationToken token = default)
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
    public override string ToString() => $"ORM.Enumerator({Command})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ICommand Command { get; }
    ORM.ICommand ORM.ICommandEnumerator.Command => Command;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool NativePaging { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Skip
    {
        get => _Skip;
        set => _Skip = value >= 0 ? value
            : throw new ArgumentException("Skip value cannot be less than cero.").WithData(value);
    }
    int _Skip = 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Take
    {
        get => _Take;
        set => _Take = value >= 0 ? value
            : throw new ArgumentException("Take value cannot be less than cero.").WithData(value);
    }
    int _Take = 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary> 
#pragma warning disable CS8766
    public IRecord? Current { get; private set; }
#pragma warning restore
    object IAsyncEnumerator<object>.Current => Current!;
    object IEnumerator.Current => Current!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ISchema? Schema { get; private set; }

    // ----------------------------------------------------

    bool _Initialized = false;
    bool _Terminated = false;
    bool _OpenedByThis = false;
    bool _TakeEmulated = false;
    int _TakeRemaining = -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    void IEnumerator.Reset() => throw new NotSupportedException();

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

        /// <summary>
        /// Initializes the execution.
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

            if (!NativePaging) // Emulating...
            {
                var skip = Skip; for (int i = 0; i < skip; i++)
                {
                    var r = OnNextResult();
                    if (r == null) return false;
                }
                var take = Take; if (take > 0)
                {
                    _TakeEmulated = true;
                    _TakeRemaining = take;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtains the next result.
        /// </summary>
        bool NextResult()
        {
            Current = null!;

            if (_TakeEmulated) // Emulating...
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

        /// <summary>
        /// Terminates the execution.
        /// </summary>
        void Terminate()
        {
            if (_Terminated) return;

            OnTerminate();
            TryCloseConnection();
            _Terminated = true;
            _Initialized = false; // Prevents reentrance...
        }

        /// <summary>
        /// Aborts the execution.
        /// </summary>
        void Abort()
        {
            if (_Terminated) return;

            OnAbort();
            TryCloseConnection();
            _Terminated = true;
            _Initialized = false; // Prevents reentrance...
        }
    }

    /// <summary>
    /// Invoked to initialize the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected abstract ISchema OnInitialize();

    /// <summary>
    /// Invoked to obtain the result of the next iteration of the command.
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

        /// <summary>
        /// Initializes the execution.
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

            if (!NativePaging) // Emulating...
            {
                var skip = Skip; for (int i = 0; i < skip; i++)
                {
                    var r = await OnNextResultAsync().ConfigureAwait(false);
                    if (r == null) return false;
                }
                var take = Take; if (take > 0)
                {
                    _TakeEmulated = true;
                    _TakeRemaining = take;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtains the next result.
        /// </summary>
        async ValueTask<bool> NextResultAsync()
        {
            Current = null!;

            if (_TakeEmulated) // Emulating...
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

        /// <summary>
        /// Terminates the execution.
        /// </summary>
        async ValueTask TerminateAsync()
        {
            if (_Terminated) return;

            await OnTerminateAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            _Terminated = true;
            _Initialized = false; // Prevents reentrance...
        }

        /// <summary>
        /// Aborts the execution.
        /// </summary>
        async ValueTask AbortAsync()
        {
            if (_Terminated) return;

            await OnAbortAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            _Terminated = true;
            _Initialized = false; // Prevents reentrance...
        }
    }

    /// <summary>
    /// Invoked to initialize the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<ISchema> OnInitializeAsync();

    /// <summary>
    /// Invoked to obtain the result of the next iteration of the command.
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
}