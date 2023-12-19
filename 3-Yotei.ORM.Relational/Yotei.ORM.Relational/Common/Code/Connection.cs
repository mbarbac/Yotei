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
    public Connection(IEngine engine, string? connectionString = null)
        : base(engine)
        => ConnectionString = connectionString;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
        : base(source)
        => ConnectionString = source.ConnectionString;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"Relational.Connection({Engine}, {Server ?? "-"}, {Database ?? "-"})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new IEngine Engine => (IEngine)base.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? ConnectionString
    {
        get => _Builder?.ConnectionString;
        set
        {
            ThrowWhenDisposed();
            ThrowWhenDisposing();

            using var disp = AsyncLock.Lock();

            if (IsOpen) throw new InvalidOperationException(
                "Cannot change the connection string of an opened instance.")
                .WithData(value)
                .WithData(this);

            _Builder = null;
            _Server = null;
            _Database = null;

            if ((value = value.NullWhenEmpty()) == null) return;

            _Builder =
                Engine.Factory.CreateConnectionStringBuilder() ??
                throw new UnExpectedException("Cannot create a connection string builder.");

            _Builder.ConnectionString = value;
        }
    }
    DbConnectionStringBuilder? _Builder = null;

    string? GetBuilderPart(string part)
    {
        if (_Builder != null)
        {
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
                    ? span[..index].ToString().NullWhenEmpty()
                    : span.ToString().NullWhenEmpty();
            }
        }
        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Server
    {
        get
        {
            _Server ??= DbConnection?.DataSource.NullWhenEmpty() ??
                GetBuilderPart("server=") ??
                GetBuilderPart("data source=");

            return _Server;
        }
    }
    string? _Server = null!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Database
    {
        get
        {
            _Database ??= DbConnection?.Database.NullWhenEmpty() ??
                GetBuilderPart("database=") ??
                GetBuilderPart("initial catalog=");

            return _Database;
        }
    }
    string? _Database = null;

    /// <summary>
    /// <inheritdoc cref="IConnection.DbConnection"/>
    /// </summary>
    internal DbConnection? DbConnection = null;
    DbConnection? IConnection.DbConnection => DbConnection;

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
            if (_Builder == null) throw new InvalidOperationException(
                "Connection string is null.")
                .WithData(this);

            if (DbConnection != null) throw new InvalidOperationException(
                "This instance is already open.")
                .WithData(this);

            DbConnection = Engine.Factory.CreateConnection() ??
                throw new DataException("Cannot create a physical connection.");

            DbConnection.ConnectionString = _Builder.ConnectionString;
            DbConnection.Open();
        }
        catch
        {
            DbConnection = null;
            throw;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async ValueTask OnOpenAsync(CancellationToken token)
    {
        try
        {
            if (_Builder == null) throw new InvalidOperationException(
                "Connection string is null.")
                .WithData(this);

            if (DbConnection != null) throw new InvalidOperationException(
                "This instance is already open.")
                .WithData(this);

            DbConnection = Engine.Factory.CreateConnection() ??
                throw new DataException("Cannot create a physical connection.");

            DbConnection.ConnectionString = _Builder.ConnectionString;
            await DbConnection.OpenAsync(token).ConfigureAwait(false);
        }
        catch
        {
            DbConnection = null;
            throw;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnClose()
    {
        if (DbConnection != null)
        {
            DbConnection.Close();
            DbConnection.Dispose();
            DbConnection = null;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask OnCloseAsync()
    {
        if (DbConnection != null)
        {
            await DbConnection.CloseAsync().ConfigureAwait(false);
            await DbConnection.DisposeAsync().ConfigureAwait(false);
            DbConnection = null;
        }
    }
}