#pragma warning disable CS8766 // Nullability mismatch

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
        else OnTerminate();
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
        else await OnTerminateAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"ORM.Enumerator[{Command}]";

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
    object IEnumerator.Current => Current!;

    // ----------------------------------------------------

    bool _OpenedByThis = false;
    bool _Initialized = false;
    bool _Terminated = false;
    bool _TakeEmulated = false;
    int _TakeRemaining = -1;

    void IEnumerator.Reset() => throw new NotSupportedException();

    void OpenConnection()
    {
        if (Command.Connection.IsOpen) return;

        Command.Connection.Open();
        _OpenedByThis = true;
    }

    async ValueTask OpenConnectionAsync()
    {
        if (Command.Connection.IsOpen) return;

        await Command.Connection.OpenAsync(CancellationToken).ConfigureAwait(false);
        _OpenedByThis = true;
    }

    void CloseConnection()
    {
        if (!Command.Connection.IsOpen || !_OpenedByThis) return;

        Command.Connection.Close();
        _OpenedByThis = false;
    }

    async ValueTask CloseConnectionAsync()
    {
        if (!Command.Connection.IsOpen || !_OpenedByThis) return;

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
        /// Initializes the execution of the command.
        /// </summary>
        bool Initialize()
        {
            if (_Initialized) return true;
            _Initialized = true;

            OpenConnection();

            Schema = OnInitialize();
            if (Schema == null) throw new UnExpectedException(
                "Cannot obtain an schema while initializing the command execution.")
                .WithData(this);

            if (!Command.NativePaging) // Paging emulation, potentially very slow!
            {
                for (int i = 0, skip = Command.Skip; i < skip; i++)
                {
                    var r = OnNextResult();
                    if (r is null) return false;
                }
                var take = Command.Take; if (take > 0)
                {
                    _TakeEmulated = true;
                    _TakeRemaining = take;
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to obtain the next result of the command.
        /// </summary>
        bool NextResult()
        {
            Current = null;

            if (_TakeEmulated)
            {
                if (_TakeRemaining > 0)
                {
                    Current = OnNextResult();
                    _TakeRemaining--;
                }
            }
            else Current = OnNextResult();

            return Current is not null;
        }

        /// <summary>
        /// Terminates the execution of the command.
        /// </summary>
        void Terminate()
        {
            if (_Initialized)
            {
                if (!_Terminated) OnTerminate();
                CloseConnection();
            }

            _Terminated = true;
            _Initialized = false; // Prevents disposing re-entrance...
        }

        /// <summary>
        /// Aborts the execution of the command.
        /// </summary>
        void Abort()
        {
            if (_Initialized)
            {
                if (!_Terminated) OnAbort();
                CloseConnection();
            }

            _Terminated = true;
            _Initialized = false; // Prevents disposing re-entrance...
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
            CancellationToken.ThrowIfCancellationRequested();

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
        /// Initializes the execution of the command.
        /// </summary>
        async ValueTask<bool> InitializeAsync()
        {
            if (_Initialized) return true;
            _Initialized = true;

            await OpenConnectionAsync().ConfigureAwait(false);

            Schema = await OnInitializeAsync().ConfigureAwait(false);
            if (Schema == null) throw new UnExpectedException(
                "Cannot obtain an schema while initializing the command execution.")
                .WithData(this);

            if (!Command.NativePaging) // Paging emulation, potentially very slow!
            {
                for (int i = 0, skip = Command.Skip; i < skip; i++)
                {
                    var r = await OnNextResultAsync().ConfigureAwait(false);
                    if (r is null) return false;
                }
                var take = Command.Take; if (take > 0)
                {
                    _TakeEmulated = true;
                    _TakeRemaining = take;
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to obtain the next result of the command.
        /// </summary>
        async ValueTask<bool> NextResultAsync()
        {
            Current = null;

            if (_TakeEmulated)
            {
                if (_TakeRemaining > 0)
                {
                    Current = await OnNextResultAsync().ConfigureAwait(false);
                    _TakeRemaining--;
                }
            }
            else Current = await OnNextResultAsync().ConfigureAwait(false);

            return Current is not null;
        }

        /// <summary>
        /// Terminates the execution of the command.
        /// </summary>
        async ValueTask TerminateAsync()
        {
            if (_Initialized)
            {
                if (!_Terminated) await OnTerminateAsync().ConfigureAwait(false);
                await CloseConnectionAsync().ConfigureAwait(false);
            }

            _Terminated = true;
            _Initialized = false; // Prevents disposing re-entrance...
        }

        /// <summary>
        /// Aborts the execution of the command.
        /// </summary>
        async ValueTask AbortAsync()
        {
            if (_Initialized)
            {
                if (!_Terminated) await OnAbortAsync().ConfigureAwait(false);
                await CloseConnectionAsync().ConfigureAwait(false);
            }

            _Terminated = true;
            _Initialized = false; // Prevents disposing re-entrance...
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