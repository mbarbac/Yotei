namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="ITransaction"/>
public class Transaction : ORM.Code.Transaction, ITransaction
{
    public const IsolationLevel ISOLATIONLEVEL = IsolationLevel.Serializable;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="isolationLevel"></param>
    public Transaction(IConnection connection, IsolationLevel isolationLevel = ISOLATIONLEVEL)
        : base(connection)
        => IsolationLevel = isolationLevel;

    /// <inheritdoc/>
    public override string ToString()
        => $"Relational.Transaction({Connection}, {Level}, {IsolationLevel})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new IConnection Connection => (IConnection)base.Connection;

    /// <inheritdoc/>
    public IsolationLevel IsolationLevel
    {
        get => _IsolationLevel;
        set
        {
            if (value == _IsolationLevel) return;

            ThrowIfDisposed();
            ThrowIfDisposing();

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

    /// <inheritdoc/>
    public DbTransaction? DbTransaction { get; private set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnStart()
    {
        if (Connection.DbConnection is null) throw new InvalidOperationException(
            "The associated connection is not opened.")
            .WithData(this);

        DbTransaction = Connection.DbConnection.BeginTransaction(IsolationLevel);
    }

    /// <inheritdoc/>
    protected override async ValueTask OnStartAsync(CancellationToken token = default)
    {
        if (Connection.DbConnection is null) throw new InvalidOperationException(
            "The associated connection is not opened.")
            .WithData(this);

        DbTransaction = 
            await Connection.DbConnection.BeginTransactionAsync(IsolationLevel, token)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override void OnCommit()
    {
        if (DbTransaction != null)
        {
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = null;
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnCommitAsync(CancellationToken token = default)
    {
        if (DbTransaction != null)
        {
            await DbTransaction.CommitAsync(token).ConfigureAwait(false);
            await DbTransaction.DisposeAsync().ConfigureAwait(false);
            DbTransaction = null;
        }
    }

    /// <inheritdoc/>
    protected override void OnAbort()
    {
        if (DbTransaction != null)
        {
            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = null;
        }
    }

    /// <inheritdoc/>
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