namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IConnection"/>
[Cloneable]
public partial class Connection : ORM.Code.Connection, IConnection
{
    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="connectionString"></param>
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

    /// <inheritdoc/>
    public override string ToString()
        => $"Relational.Connection({Engine}, {Server ?? "-"}, {Database ?? "-"})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new IEngine Engine => (IEngine)base.Engine;

    /// <inheritdoc/>
    public new IRecordsGate Records => (IRecordsGate)base.Records;

    // ----------------------------------------------------

    DbConnectionStringBuilder? _Builder;
    string? _Server;
    string? _Database;

    string? GetBuilderPart(string part)
    {
        if (_Builder is not null)
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

    /// <inheritdoc/>
    public string? ConnectionString
    {
        get => _Builder?.ConnectionString;
        set
        {
            if (value == null && (IsDisposed || OnDisposing))
            {
                _Builder = null;
                return;
            }
            ThrowIfDisposed();
            ThrowIfDisposing();

            using var disp = AsyncLock.Lock();

            if (IsOpen) throw new InvalidOperationException(
                "Cannot change the connection string of and opened instance.")
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
    

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public DbConnection? DbConnection { get; private set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IsolationLevel IsolationLevel
    {
        get => _IsolationLevel;
        set
        {
            Transaction = null!; // Throws if it was an active one.
            _IsolationLevel = value;
        }
    }
    IsolationLevel _IsolationLevel = Code.Transaction.ISOLATIONLEVEL;

    /// <inheritdoc/>
    public new ITransaction Transaction
    {
        get => (ITransaction)base.Transaction;
        protected set => base.Transaction = value;
    }

    /// <inheritdoc/>
    protected override ITransaction CreateTransaction() => new Transaction(this, IsolationLevel);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool IsOpen 
        => DbConnection != null && DbConnection.State == ConnectionState.Open;

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        try
        {
            if (_Builder == null) throw new InvalidOperationException(
                "Connection string is null.")
                .WithData(this);

            if (DbConnection != null) throw new InvalidOperationException(
                "This instance is already opened.")
                .WithData(this);

            DbConnection = Engine.Factory.CreateConnection() ??
                throw new DataException("Cannot create a physical database connection.");

            DbConnection.ConnectionString = _Builder.ConnectionString;

            DbConnection.Open();
        }
        catch
        {
            DbConnection = null;
            throw;
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnOpenAsync(CancellationToken token)
    {
        try
        {
            if (_Builder == null) throw new InvalidOperationException(
                "Connection string is null.")
                .WithData(this);

            if (DbConnection != null) throw new InvalidOperationException(
                "This instance is already opened.")
                .WithData(this);

            DbConnection = Engine.Factory.CreateConnection() ??
                throw new DataException("Cannot create a physical database connection.");

            DbConnection.ConnectionString = _Builder.ConnectionString;

            await DbConnection.OpenAsync(token).ConfigureAwait(false);
        }
        catch
        {
            DbConnection = null;
            throw;
        }
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        if (DbConnection != null)
        {
            DbConnection.Close();
            DbConnection.Dispose();
            DbConnection = null;
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnCloseAsync()
    {
        if (DbConnection != null)
        {
            await DbConnection.CloseAsync().ConfigureAwait(false);
            await DbConnection.DisposeAsync().ConfigureAwait(false);
            DbConnection = null;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IRecordsGate CreateRecordsGate() => new RecordsGate(this);
}