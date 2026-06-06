namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
public abstract class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> This constructor is INFRASTRUCTURE and shall not be used by application code.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public Transaction(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        try { Abort(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        try { await AbortAsync().ConfigureAwait(false); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Transaction({Connection})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// Determines if this instance has opened its associated connection, or not.
    /// </summary>
    bool HasOpenedConnection = false;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITransaction.Start"/>
    /// </summary>
    protected virtual void Start()
    {
        if (Connection.Transaction != null &&
            !ReferenceEquals(Connection.Transaction, this)) throw new InvalidOperationException(
                "The connection is already associated with other transaction.")
                .WithData(Connection);

        Connection.Transaction = this;
        if (!Connection.IsOpen)
        {
            Connection.Open();
            HasOpenedConnection = true;
        }
    }
    void ITransaction.Start() => Start();

    /// <summary>
    /// <inheritdoc cref="ITransaction.StartAsync(CancellationToken)"/>
    /// </summary>
    /// <param name="token"></param>
    protected virtual async ValueTask StartAsync(CancellationToken token)
    {
        if (Connection.Transaction != null &&
            !ReferenceEquals(Connection.Transaction, this)) throw new InvalidOperationException(
                "The connection is already associated with other transaction.")
                .WithData(Connection);

        Connection.Transaction = this;
        if (!Connection.IsOpen)
        {
            await Connection.OpenAsync(token).ConfigureAwait(false);
            HasOpenedConnection = true;
        }
    }
    ValueTask ITransaction.StartAsync(CancellationToken token) => StartAsync(token);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Commit()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        OnCommit();

        if (ReferenceEquals(this, Connection.Transaction)) Connection.Transaction = null;
        if (HasOpenedConnection) { Connection.Close(); HasOpenedConnection = false; }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        await OnCommitAsync(token).ConfigureAwait(false);

        if (ReferenceEquals(this, Connection.Transaction)) Connection.Transaction = null;
        if (HasOpenedConnection)
        {
            await Connection.CloseAsync().ConfigureAwait(false);
            HasOpenedConnection = false;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Abort()
    {
        if (IsDisposed) return;

        OnAbort();

        if (ReferenceEquals(this, Connection.Transaction)) Connection.Transaction = null;
        if (HasOpenedConnection)
        {
            Connection.Close();
            HasOpenedConnection = false;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask AbortAsync()
    {
        if (IsDisposed) return;

        await OnAbortAsync().ConfigureAwait(false);

        if (ReferenceEquals(this, Connection.Transaction)) Connection.Transaction = null;
        if (HasOpenedConnection)
        {
            await Connection.CloseAsync().ConfigureAwait(false);
            HasOpenedConnection = false;
        }
    }

    // ----------------------------------------------------

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