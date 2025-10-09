namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents a connection with an underlying relational database.
/// <br/> Instances of this type are not supposed to be shared among threads.
/// </summary>
public class Connection : ORM.Connection
{
    public const IsolationLevel ISOLATIONLEVEL = IsolationLevel.Serializable;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="connectionString"></param>
    public Connection(Engine engine, string? connectionString = null)
        : base(engine)
        => ConnectionString = connectionString;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
        : base(source)
        => ConnectionString = source.ConnectionString;

    /// <inheritdoc/>
    public override Connection Clone() => new(this);

    /// <inheritdoc/>
    public override string ToString()
        => $"Relational.Connection({Engine}, {Server ?? "-"}, {Database ?? "-"})";

    // ----------------------------------------------------

    /// <inheritdoc cref="ORM.Connection.Engine"/>
    public new Engine Engine => (Engine)base.Engine;

    /// <summary>
    /// The default isolation level for the transactions created by this instance.
    /// </summary>
    public IsolationLevel IsolationLevel { get; set; } = ISOLATIONLEVEL;

    /// <summary>
    /// Invoked to create a new transaction of the appropriate type for this instance, with the
    /// given initial isolation level.
    /// </summary>
    /// <param name="isolationLevel"></param>
    /// <returns></returns>
    public virtual Transaction CreateTransaction(
        IsolationLevel isolationLevel) => new(this, isolationLevel);

    /// <summary>
    /// <inheritdoc/> This method uses the default isolation level of this instance.
    /// </summary>
    /// <returns></returns>
    public override Transaction CreateTransaction() => CreateTransaction(IsolationLevel);

    // ----------------------------------------------------

    DbConnectionStringBuilder? _Builder;
    string? _Server;
    string? _Database;

    /// Obtains the value associated with the given part specification.
    string? GetBuilderPart(string part)
    {
        if (_Builder is null) return null;

        if (_Builder.TryGetValue(part, out var value) &&
                value is not null &&
                value is string str)
            return str;

        var cnstr = _Builder.ConnectionString;
        var index = cnstr.IndexOf(part, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
            var span = cnstr.AsSpan(index)[part.Length..];
            index = span.IndexOf(';');

            return index >= 0
                ? span[..index].ToString().NullWhenEmpty(true)
                : span.ToString().NullWhenEmpty(true);
        }

        return null;
    }

    /// <summary>
    /// The connection string used by this instance, or '<c>null</c>' if its value has not been
    /// set yet. The setter throws an exception if this connection is opened.
    /// </summary>
    public string? ConnectionString
    {
        get => _Builder?.ConnectionString;
        set
        {
            ThrowIfDisposed();
            ThrowIfDisposing();
            
            using var disp = Lock.Enter();

            if (IsOpen) throw new InvalidOperationException(
                "Cannot change the connection string of an open connection.")
                .WithData(value)
                .WithData(this);

            _Builder = null;
            _Server = null;
            _Database = null;
            if ((value = value.NullWhenEmpty(true)) is null) return;

            _Builder =
                Engine.ProviderFactory.CreateConnectionStringBuilder() ??
                throw new UnExpectedException("Cannot create a connection string builder.");

            _Builder.ConnectionString = value;
        }
    }

    /// <summary>
    /// The server this instance connects to, or <c>null</c> if this information is not available.
    /// </summary>
    public string? Server => _Server ??= (
        DbConnection?.DataSource.NullWhenEmpty(true) ??
        GetBuilderPart("server=") ??
        GetBuilderPart("data source="));

    /// <summary>
    /// The database, or catalog this instance connects to, or <c>null</c> if this information is
    /// not available.
    /// </summary>
    public string? Database => _Database ??= (
        DbConnection?.Database.NullWhenEmpty(true) ??
        GetBuilderPart("database=") ??
        GetBuilderPart("initial catalog="));

    // ----------------------------------------------------

    /// <summary>
    /// Maintains the underlying physical connection.
    /// </summary>
    internal DbConnection? DbConnection { get; private set; }

    /// <inheritdoc/>
    public override bool IsOpen
        => DbConnection != null && DbConnection.State == ConnectionState.Open;

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        throw null;
    }

    /// <inheritdoc/>
    protected override ValueTask OnOpenAsync(CancellationToken token)
    {
        throw null;
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        throw null;
    }

    /// <inheritdoc/>
    protected override ValueTask OnCloseAsync()
    {
        throw null;
    }
}