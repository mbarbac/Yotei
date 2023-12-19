namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.ITransaction"/>
/// </summary>
public class Transaction : ORM.Code.Transaction, ITransaction
{
    public const IsolationLevel ISOLATIONLEVEL = IsolationLevel.Serializable;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="isolationLevel"></param>
    public Transaction(IConnection connection, IsolationLevel isolationLevel)
        : base(connection)
        => IsolationLevel = isolationLevel;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"Relational.Transaction({Connection}, {Level}, {IsolationLevel})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new IConnection Connection => (IConnection)base.Connection;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IsolationLevel IsolationLevel
    {
        get => _IsolationLevel;
        set
        {
            if (value == _IsolationLevel) return;

            ThrowWhenDisposed();
            ThrowWhenDisposing();

            using (var disp = AsyncLock.Lock())
            {
                if (IsActive) throw new InvalidOperationException(
                    "Cannot change the isolation level of an active transaction.")
                    .WithData(value)
                    .WithData(this);

                _IsolationLevel = value;
            }
        }
    }
    IsolationLevel _IsolationLevel = ISOLATIONLEVEL;

    /// <summary>
    /// <inheritdoc cref="ITransaction.DbTransaction"/>
    /// </summary>
    internal DbTransaction? DbTransaction { get; private set; }
    DbTransaction? ITransaction.DbTransaction => DbTransaction;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnStart()
    {
        if (Connection.DbConnection == null) throw new InvalidOperationException(
            "The associated connection is not open.")
            .WithData(this);

        DbTransaction = Connection.DbConnection.BeginTransaction(IsolationLevel);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async ValueTask OnStartAsync(CancellationToken token)
    {
        if (Connection.DbConnection == null) throw new InvalidOperationException(
            "The associated connection is not open.")
            .WithData(this);

        DbTransaction =
            await Connection.DbConnection.BeginTransactionAsync(IsolationLevel, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnCommit()
    {
        if (DbTransaction != null)
        {
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = null;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async ValueTask OnCommitAsync(CancellationToken token)
    {
        if (DbTransaction != null)
        {
            await DbTransaction.CommitAsync(token).ConfigureAwait(false);
            await DbTransaction.DisposeAsync().ConfigureAwait(false);
            DbTransaction = null;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnAbort()
    {
        if (DbTransaction != null)
        {
            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = null;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask OnAbortAsync()
    {
        if (DbTransaction != null)
        {
            await DbTransaction.RollbackAsync().ConfigureAwait(false);
            await DbTransaction.DisposeAsync().ConfigureAwait(false);
            DbTransaction = null;
        }
    }
}