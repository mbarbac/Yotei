namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
public abstract class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Transaction(IConnection connection)
    {
        Connection = connection.ThrowWhenNull();
        (Connection as Connection)?.AddTransaction(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { (Connection as Connection)?.RemoveTransaction(this); } catch { }
        try { if (IsActive) Abort(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { (Connection as Connection)?.RemoveTransaction(this); } catch { }
        try { if (IsActive) await AbortAsync().ConfigureAwait(false); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Transaction({Connection}, {Level})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsActive => Level > 0;

    /// <summary>
    /// The nesting level of this instance. A value of 'zero' means that the underlying physical
    /// transaction has not been started. A value of 'one' means it has been started once, etc.
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Determines if this instance has opened its associated connection, or not.
    /// </summary>
    public bool HasOpenedConnection { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Start()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask StartAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Commit()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Abort()
    {
        if (IsDisposed) return;

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
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask AbortAsync()
    {
        if (IsDisposed) return;

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