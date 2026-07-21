namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
public class Transaction : ORM.Code.Transaction, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="isolationLevel"></param>
    [SuppressMessage("", "IDE0290")]
    public Transaction(IConnection connection, IsolationLevel isolationLevel) : base(connection)
    {
        DbTransaction = null!;
        IsolationLevel = isolationLevel;
    }

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
    /// <br/> The value of this property might be null if this instance was created explicitly.
    /// </summary>
    public DbTransaction DbTransaction { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IsolationLevel IsolationLevel { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnStart()
    {
        var conn = Connection.DbConnection!;
        DbTransaction = conn.BeginTransaction(IsolationLevel);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async ValueTask OnStartAsync(CancellationToken token)
    {
        var conn = Connection.DbConnection!;
        DbTransaction = await conn.BeginTransactionAsync(IsolationLevel, token).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnCommit()
    {
        if (DbTransaction == null) return;

        DbTransaction.Commit();
        DbTransaction.Dispose();
        DbTransaction = null!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async ValueTask OnCommitAsync(CancellationToken token)
    {
        if (DbTransaction == null) return;

        await DbTransaction.CommitAsync(token).ConfigureAwait(false);
        await DbTransaction.DisposeAsync().ConfigureAwait(false);
        DbTransaction = null!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnAbort()
    {
        if (DbTransaction == null) return;

        DbTransaction.Rollback();
        DbTransaction.Dispose();
        DbTransaction = null!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask OnAbortAsync()
    {
        if (DbTransaction == null) return;

        await DbTransaction.RollbackAsync().ConfigureAwait(false);
        await DbTransaction.DisposeAsync().ConfigureAwait(false);
        DbTransaction = null!;
    }
}