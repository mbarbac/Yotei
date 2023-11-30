namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandEnumerator{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CommandEnumerator<T> : DisposableClass, ICommandEnumerator<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(IEnumerableCommand<T> command, CancellationToken token = default)
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

        if (_Initialized)
        {
            if (!_Terminated) OnAbort();
            else OnTerminate();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (_Initialized)
        {
            if (!_Terminated) await OnAbortAsync().ConfigureAwait(false);
            else await OnTerminateAsync().ConfigureAwait(false);
        }
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
    public IEnumerableCommand<T> Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
#pragma warning disable CS8766
    public T? Current { get; private set; } = null;
    object IEnumerator.Current => Current!;
#pragma warning restore CS8766

    // ----------------------------------------------------

    bool _OpenedByThis = false;
    bool _Initialized = false;
    bool _Terminated = false;
    bool _TakeEmulated = false;
    int _TakeRemaining = -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    void IEnumerator.Reset() => throw new NotSupportedException();

    /// <summary>
    /// Opens the associated connection, if needed.
    /// </summary>
    void OpenConnection()
    {
        if (!Command.Connection.IsOpen)
        {
            Command.Connection.Open();
            _OpenedByThis = true;
        }
    }

    /// <summary>
    /// Opens the associated connection, if needed.
    /// </summary>
    async ValueTask OpenConnectionAsync()
    {
        if (!Command.Connection.IsOpen)
        {
            await Command.Connection.OpenAsync(CancellationToken).ConfigureAwait(false);
            _OpenedByThis = true;
        }
    }

    /// <summary>
    /// Closes the associated connection, if needed.
    /// </summary>
    void CloseConnection()
    {
        if (Command.Connection.IsOpen && _OpenedByThis)
        {
            Command.Connection.Close();
            _OpenedByThis = false;
        }
    }

    /// <summary>
    /// Closes the associated connection, if needed.
    /// </summary>
    async ValueTask CloseConnectionAsync()
    {
        if (Command.Connection.IsOpen && _OpenedByThis)
        {
            await Command.Connection.CloseAsync().ConfigureAwait(false);
            _OpenedByThis = false;
        }
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
                if (!DoInitialize()) break;
                if (!DoNextResult()) break;
                return true;
            }
            if (_Initialized) DoTerminate();
            return false;
        }
        catch
        {
            if (_Initialized) DoAbort();
            throw;
        }

        // Initializes the execution of the command...
        bool DoInitialize()
        {
            if (_Initialized) return true;
            _Initialized = true;

            OpenConnection();

            if (!Command.NativePaging) // Emulating paging if such is needed...
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

        // Obtains the next available result, if any...
        bool DoNextResult()
        {
            Current = null;

            if (_TakeEmulated)
            {
                if (_TakeRemaining > 0) // Emulating paging if such is needed...
                {
                    Current = OnNextResult();
                    _TakeRemaining--;
                }
            }
            else
            {
                Current = OnNextResult();
            }

            return Current is not null;
        }

        // Terminates the execution of the command...
        void DoTerminate()
        {
            if (!_Terminated && _Initialized)
            {
                OnTerminate();
                CloseConnection();
            }

            _Terminated = true;
            _Initialized = false; // Prevents calling initialize again...
        }

        // Aborts the execution of the command...
        void DoAbort()
        {
            if (!_Terminated && _Initialized)
            {
                OnAbort();
                CloseConnection();
            }

            _Terminated = true;
            _Initialized = false; // Prevents calling initialize again...
        }
    }

    /// <summary>
    /// Invoked to initialize the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected abstract bool OnInitialize();

    /// <summary>
    /// Invoked to return the result of the next iteration of the command, or null if any.
    /// </summary>
    /// <returns></returns>
    protected abstract T? OnNextResult();

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
                if (!await DoInitializeAsync().ConfigureAwait(false)) break;
                if (!await DoNextResultAsync().ConfigureAwait(false)) break;
                return true;
            }
            if (_Initialized) await DoTerminateAsync().ConfigureAwait(false);
            return false;
        }
        catch
        {
            if (_Initialized) await DoAbortAsync().ConfigureAwait(false);
            throw;
        }

        // Initializes the execution of the command...
        async ValueTask<bool> DoInitializeAsync()
        {
            if (_Initialized) return true;
            _Initialized = true;

            await OpenConnectionAsync().ConfigureAwait(false);

            if (!Command.NativePaging) // Emulating paging if such is needed...
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

        // Obtains the next available result, if any...
        async ValueTask<bool> DoNextResultAsync()
        {
            Current = null;

            if (_TakeEmulated) // Emulating paging if such is needed...
            {
                if (_TakeRemaining > 0)
                {
                    Current = await OnNextResultAsync().ConfigureAwait(false);
                    _TakeRemaining--;
                }
            }
            else
            {
                Current = await OnNextResultAsync().ConfigureAwait(false);
            }

            return Current is not null;
        }

        // Terminates the execution of the command...
        async ValueTask DoTerminateAsync()
        {
            if (!_Terminated && _Initialized)
            {
                await OnTerminateAsync().ConfigureAwait(false);
                await CloseConnectionAsync().ConfigureAwait(false);
            }

            _Terminated = true;
            _Initialized = false; // Prevents calling initialize again...
        }

        // Aborts the execution of the command...
        async ValueTask DoAbortAsync()
        {
            if (!_Terminated && _Initialized)
            {
                await OnAbortAsync().ConfigureAwait(false);
                await CloseConnectionAsync().ConfigureAwait(false);
            }

            _Terminated = true;
            _Initialized = false; // Prevents calling initialize again...
        }
    }

    /// <summary>
    /// Invoked to initialize the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<bool> OnInitializeAsync();

    /// <summary>
    /// Invoked to return the result of the next iteration of the command, or null if any.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask<T?> OnNextResultAsync();

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected abstract ValueTask OnTerminateAsync();

    /// <summary>
    /// Invoked to abort the execution of the command.
    /// </summary>
    protected abstract ValueTask OnAbortAsync();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList()
    {
        var list = new List<T>();

        while (MoveNext())
            if (Current is not null) list.Add(Current);
        
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<List<T>> ToListAsync()
    {
        var list = new List<T>();

        while (await MoveNextAsync().ConfigureAwait(false))
            if (Current is not null) list.Add(Current);

        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => ToList().ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<T[]> ToArrayAsync()
    {
        var list = await ToListAsync().ConfigureAwait(false);
        return list.ToArray();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T? First()
    {
        if (MoveNext())
        {
            var r = Current;
            if (r is not null) return r;
        }
        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<T?> FirstAsync()
    {
        if (await MoveNextAsync().ConfigureAwait(false))
        {
            var r = Current;
            if (r is not null) return r;
        }
        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T? Last()
    {
        T? r = null;

        while (MoveNext())
        {
            var temp = Current;
            if (temp is not null) r = temp;
        }
        return r;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<T?> LastAsync()
    {
        T? r = null;

        while (await MoveNextAsync().ConfigureAwait(false))
        {
            var temp = Current;
            if (temp is not null) r = temp;
        }
        return r;
    }
}