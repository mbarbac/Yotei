namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable]
public partial class Connection : ORM.Code.Connection, IConnection
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="connectionString"></param>
    public Connection(IEngine engine, string? connectionString = null)
        : base(engine)
        => ConnectionString = connectionString;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Connection(Connection other) : base(other)
    {
        ConnectionString = other.ConnectionString;
        IsolationLevel = other.IsolationLevel;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"Relational.Connection({Engine}, {Server ?? "-"}, {Database ?? "-"})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new IEngine Engine => (IEngine)base.Engine;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the value of the given part.
    /// </summary>
    static string? GetPart(DbConnectionStringBuilder builder, string part)
    {
        if (builder.TryGetValue(part, out var value) &&
            value != null &&
            value is string str) return str;

        str = builder.ConnectionString;
        var index = str.IndexOf(part, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
            var span = str.AsSpan(index)[part.Length..];            
            index = span.IndexOf('=');
            if (index >= 0) span = span[(index + 1)..];

            index = span.IndexOf(';');
            str = index >= 0
                ? span[..index].ToString().NullWhenEmpty(trim: true)!
                : span.ToString().NullWhenEmpty(trim: true)!;

            return str;
        }

        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? ConnectionString
    {
        get;
        set
        {
            if (IsDisposed || OnDisposing)
            {
                field = null; Server = null; Database = null;
                return;
            }

            if (IsOpen) throw new InvalidOperationException(
                "Cannot set the connection string of an active connection.")
                .WithData(this);

            if ((value = value?.Trim()) == null)
            {
                field = null; Server = null; Database = null;
                return;
            }

            var builder = Engine.DbFactory.CreateConnectionStringBuilder() ??
                throw new UnExpectedException("Cannot create a connection string builder.").WithData(this);

            builder.ConnectionString = value;
            field = builder.ConnectionString;

            Server = GetPart(builder, "server") ?? GetPart(builder, "data source");
            Database = GetPart(builder, "database") ?? GetPart(builder, "initial catalog");
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Server { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Database { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public DbConnection? DbConnection
    {
        get;
        private set;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsOpen
        => DbConnection != null && DbConnection.State == ConnectionState.Open;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnOpen()
    {
        try
        {
            if (ConnectionString == null) throw new InvalidOperationException(
                "The connection string of this instance is not set yet.").WithData(this);

            DbConnection = Engine.DbFactory.CreateConnection() ??
                throw new UnExpectedException("Cannot create a physical connection.").WithData(this);

            DbConnection.ConnectionString = ConnectionString;
            DbConnection.Open();
        }
        catch
        {
            DbConnection?.Dispose();
            DbConnection = null;
            throw;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override async ValueTask OnOpenAsync(CancellationToken token)
    {
        try
        {
            if (ConnectionString == null) throw new InvalidOperationException(
                "The connection string of this instance is not set yet.").WithData(this);

            DbConnection = Engine.DbFactory.CreateConnection() ??
                throw new UnExpectedException("Cannot create a physical connection.").WithData(this);

            await DbConnection.OpenAsync(token).ConfigureAwait(false);
        }
        catch
        {
            if (DbConnection != null) await DbConnection.DisposeAsync().ConfigureAwait(false);
            DbConnection = null;
            throw;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnClose()
    {
        if (DbConnection == null) return;

        DbConnection.Close();
        DbConnection.Dispose();
        DbConnection = null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override async ValueTask OnCloseAsync()
    {
        if (DbConnection == null) return;

        await DbConnection.CloseAsync().ConfigureAwait(false);
        await DbConnection.DisposeAsync().ConfigureAwait(false);
        DbConnection = null;
    }

    // ----------------------------------------------------

    public const IsolationLevel ISOLATIONLEVEL = IsolationLevel.Serializable;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IsolationLevel IsolationLevel { get; set; } = ISOLATIONLEVEL;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override ITransaction CreateTransaction() => new Transaction(this, IsolationLevel);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new ITransaction? Transaction => (ITransaction?)base.Transaction;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public new ITransaction StartTransaction() => (ITransaction)base.StartTransaction();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public new async ValueTask<ITransaction> StartTransactionAsync(
        CancellationToken token = default)
        => (ITransaction)await base.StartTransactionAsync(token);
}