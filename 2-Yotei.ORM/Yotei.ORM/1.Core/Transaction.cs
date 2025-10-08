namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a nestable database transaction associated with a given connection, that manages
/// how many times it has been started and committed, reverting to the underlying physical one
/// only when neccesary.
/// </summary>
public abstract class Transaction : DisposableClass
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Transaction(Connection connection)
    {
        Connection = connection.ThrowWhenNull();
        Connection.AddTransaction(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Transaction({Connection}, {Level})";

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { Connection.RemoveTransaction(this); } catch { }
        try { if (IsActive) Abort(); } catch { }
        try { Lock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { Connection.RemoveTransaction(this); } catch { }
        try { if (IsActive) await AbortAsync().ConfigureAwait(false); } catch { }
        try { await Lock.DisposeAsync().ConfigureAwait(false); } catch { }
    }

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    public Connection Connection { get; }

    /// <summary>
    /// Determines if this transaction is active, or not.
    /// </summary>
    public bool IsActive => Level > 0;

    /// <summary>
    /// Indicates the current nesting level of this instance. A value of zero means it has not
    /// been started yet.
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// The object used to synchronize operations of this instance.
    /// </summary>
    public AsyncLock Lock { get; } = new();

    /// <summary>
    /// Determines if this instance has opened its associated connection, or not.
    /// </summary>
    public bool HasOpenedConnection { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// Either starts the underlying physical transaction or increses the nesting level of this
    /// instance.
    /// </summary>
    public void Start()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        using var disp = Lock.Enter();

        if (Level >= 1) Level++;
        else
        {
            if (!Connection.IsOpen)
            {
                Connection.Open();
                HasOpenedConnection = true;
            }

            OnStart();
            Level = 1;
        }
    }

    /// <summary>
    /// Either starts the underlying physical transaction or increses the nesting level of this
    /// instance.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask StartAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        await using var disp = await Lock.EnterAsync(token).ConfigureAwait(false);

        if (Level >= 1) Level++;
        else
        {
            if (!Connection.IsOpen)
            {
                await Connection.OpenAsync(token).ConfigureAwait(false);
                HasOpenedConnection = true;
            }

            await OnStartAsync(token).ConfigureAwait(false);
            Level = 1;
        }
    }

    /// <summary>
    /// Either decreases the nesting level of this instance or commits the underlying physical
    /// transaction.
    /// </summary>
    public void Commit()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        using var disp = Lock.Enter();

        if (Level == 0)
        {
            return;
        }
        else if (Level == 1)
        {
            OnCommit();
            Level = 0;

            if (HasOpenedConnection)
            {
                HasOpenedConnection = false;
                Connection.Close();
            }
        }
        else
        {
            Level--;
        }
    }

    /// <summary>
    /// Either decreases the nesting level of this instance or commits the underlying physical
    /// transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        await using var disp = await Lock.EnterAsync(token).ConfigureAwait(false);

        if (Level == 0)
        {
            return;
        }
        else if (Level == 1)
        {
            await OnCommitAsync(token).ConfigureAwait(false);
            Level = 0;

            if (HasOpenedConnection)
            {
                HasOpenedConnection = false;
                await Connection.CloseAsync().ConfigureAwait(false);
            }
        }
        else
        {
            Level--;
        }
    }

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if it was already started,
    /// and resets the nesting level to zero.
    /// </summary>
    public void Abort()
    {
        if (IsDisposed) return;

        using var disp = Lock.Enter();

        if (Level > 0)
        {
            OnAbort();

            if (HasOpenedConnection)
            {
                HasOpenedConnection = false;
                Connection.Close();
            }
        }
        Level = 0;
    }

    /// <summary>
    /// Inconditionally aborts the underlying physical transaction, if it was already started,
    /// and resets the nesting level to zero.
    /// </summary>
    /// <returns></returns>
    public async ValueTask AbortAsync()
    {
        if (IsDisposed) return;

        await using var disp = await Lock.EnterAsync().ConfigureAwait(false);

        if (Level > 0)
        {
            await OnAbortAsync().ConfigureAwait(false);

            if (HasOpenedConnection)
            {
                HasOpenedConnection = false;
                await Connection.CloseAsync().ConfigureAwait(false);
            }
        }
        Level = 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    protected abstract void OnStart();

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnStartAsync(CancellationToken token);

    /// <summary>
    /// Invoked to commit the underlying physical transaction
    /// </summary>
    protected abstract void OnCommit();

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnCommitAsync(CancellationToken token);

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    protected abstract void OnAbort();

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnAbortAsync();
}