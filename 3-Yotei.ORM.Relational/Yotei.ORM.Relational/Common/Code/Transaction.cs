
namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.ITransaction"/>
/// </summary>
public class Transaction : ORM.Code.Transaction, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="isolation"></param>
    public Transaction(IConnection connection, IsolationLevel isolation)
        : base(connection)
        => IsolationLevel = isolation;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Relational.Transaction({Connection}, {IsolationLevel})";

    // ----------------------------------------------------

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
            ThrowWhenDisposed();
            ThrowWhenDisposing();

            using (var disp = AsyncLock.Lock())
            {
                if (IsActive) throw new InvalidOperationException(
                    "Cannot change the isolation level of an active transaction.")
                    .WithData(this);

                _IsolationLevel = value;
            }
        }
    }
    IsolationLevel _IsolationLevel = default;

    // ----------------------------------------------------

    /// <summary>
    /// The underlying physical transaction, or null if any.
    /// </summary>
    protected DbTransaction? DbTransaction { get; private set; }

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
            await Connection.DbConnection.BeginTransactionAsync(IsolationLevel).ConfigureAwait(false);
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