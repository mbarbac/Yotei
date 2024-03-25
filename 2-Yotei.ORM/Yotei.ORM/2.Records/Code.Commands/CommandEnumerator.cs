#pragma warning disable IDE0290 // Use primary constructor

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
    public ISchema? Schema { get; private set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

        // Initializes the execution of the command...
        bool Initialize()
        {
            if (_Initialized) return true;
            _Initialized = true;

            TryOpenConnection();

            Schema = OnInitialize();
            if (Schema == null) throw new UnExpectedException(
                "Cannot obtain a schema while initializing the command execution.")
                .WithData(this);

            if (!Command.NativePaging) // We may need to emulate paging...
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

        // Gets the next available record, if any...
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

        // Terminates the execution...
        void Terminate()
        {
            if (_Terminated) return;

            OnTerminate();
            TryCloseConnection();
            _Terminated = true;
            _Initialized = false;
        }

        // Aborts the execution...
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

    /// <inheritdoc/>
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

        // Initializes the execution of the command...
        async ValueTask<bool> InitializeAsync()
        {
            if (_Initialized) return true;
            _Initialized = true;

            await TryOpenConnectionAsync().ConfigureAwait(false);

            Schema = await OnInitializeAsync().ConfigureAwait(false);
            if (Schema == null) throw new UnExpectedException(
                "Cannot obtain a schema while initializing the command execution.")
                .WithData(this);

            if (!Command.NativePaging) // We may need to emulate paging...
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

        // Gets the next available record, if any...
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

        // Terminates the execution...
        async ValueTask TerminateAsync()
        {
            if (_Terminated) return;

            await OnTerminateAsync().ConfigureAwait(false);
            await TryCloseConnectionAsync().ConfigureAwait(false);
            _Terminated = true;
            _Initialized = false;
        }

        // Aborts the execution...
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