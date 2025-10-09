namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a nestable database transaction associated with a given relational connection,
/// that manages how many times it has been started and committed, reverting to the underlying
/// physical one only when neccesary.
/// </summary>
public class Transaction : ORM.Transaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="isolationLevel"></param>
    public Transaction(
        Connection connection, IsolationLevel isolationLevel = Connection.ISOLATIONLEVEL)
        : base(connection)
        => IsolationLevel = isolationLevel;

    /// <inheritdoc/>
    public override string ToString()
        => $"Relational.Transaction({Connection}, {Level}, {IsolationLevel})";

    // ----------------------------------------------------

    /// <inheritdoc cref="ORM.Transaction.Connection"/>
    public new Connection Connection => (Connection)base.Connection;

    /// <summary>
    /// Gets or sets the isolation level used by this instance.
    /// <br/> The setter throws an exception if this instance is active.
    /// </summary>
    public IsolationLevel IsolationLevel
    {
        get;
        set
        {
            if (field == value) return;

            ThrowIfDisposed();
            ThrowIfDisposing();

            using var disp = Lock.Enter();

            if (IsActive) throw new InvalidOperationException(
                "Cannot change the isolation level of an active transaction.")
                .WithData(value)
                .WithData(this);

            field = value;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Maintains the underlying physical transaction.
    /// </summary>
    internal DbTransaction? DbTransaction { get; private set; }

    /// <inheritdoc/>
    protected override void OnStart()
    {
        if (!Connection.IsOpen) throw new InvalidOperationException(
            "The associated connection is not opened.")
            .WithData(this);

        if (DbTransaction is not null) throw new InvalidOperationException(
            "The physical transaction is already started.")
            .WithData(this);

        DbTransaction = Connection.DbConnection!.BeginTransaction(IsolationLevel);
    }

    /// <inheritdoc/>
    protected override async ValueTask OnStartAsync(CancellationToken token)
    {
        if (!Connection.IsOpen) throw new InvalidOperationException(
            "The associated connection is not opened.")
            .WithData(this);

        if (DbTransaction is not null) throw new InvalidOperationException(
            "The physical transaction is already started.")
            .WithData(this);

        DbTransaction =
            await Connection.DbConnection!.BeginTransactionAsync(IsolationLevel, token)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override void OnCommit()
    {
        if (DbTransaction is not null)
        {
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = null;
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnCommitAsync(CancellationToken token)
    {
        if (DbTransaction is not null)
        {
            await DbTransaction.CommitAsync(token).ConfigureAwait(false);
            await DbTransaction.DisposeAsync().ConfigureAwait(false);
            DbTransaction = null;
        }
    }

    /// <inheritdoc/>
    protected override void OnAbort()
    {
        if (DbTransaction is not null)
        {
            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = null;
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnAbortAsync()
    {
        if (DbTransaction is not null)
        {
            await DbTransaction.RollbackAsync().ConfigureAwait(false);
            await DbTransaction.DisposeAsync().ConfigureAwait(false);
            DbTransaction = null;
        }
    }
}