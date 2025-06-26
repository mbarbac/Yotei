namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecordsGate"/>
public abstract class RecordsGate : IRecordsGate
{
    /// <summary>
    /// Initializes a new intance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordsGate(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <inheritdoc/>
    public IConnection Connection { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <inheritdoc/>
    public abstract ICommandExecutor CreateCommandExecutor(IExecutableCommand command);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual DbTokenVisitor CreateDbTokenVisitor(
       Locale? locale = null,
       bool useNullString = DbTokenVisitor.USENULLSTRING,
       bool captureValues = DbTokenVisitor.CAPTUREVALUES,
       bool convertValues = DbTokenVisitor.CONVERTVALUES,
       bool useQuotes = DbTokenVisitor.USEQUOTES,
       bool useTerminators = DbTokenVisitor.USETERMINATORS,
       string? rangeSeparator = DbTokenVisitor.RANGESEPARATOR)
    {
        return new(
            Connection,
            locale,
            useNullString,
            captureValues,
            convertValues,
            useQuotes,
            useTerminators,
            rangeSeparator);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual RawCommand Raw() => new(Connection);
    IRawCommand IRecordsGate.Raw() => Raw();

    /// <inheritdoc/>
    public virtual RawCommand Raw(string? text, params object?[] args) => new(Connection, text, args);
    IRawCommand IRecordsGate.Raw(string? text, params object?[] args) => Raw(text, args);

    /// <inheritdoc/>
    public virtual RawCommand Raw(Func<dynamic, object> spec) => new(Connection, spec);
    IRawCommand IRecordsGate.Raw(Func<dynamic, object> spec) => Raw(spec);

    /// <inheritdoc/>
    public virtual RawCommand Raw(Func<dynamic, string> spec) => new(Connection, spec);
    IRawCommand IRecordsGate.Raw(Func<dynamic, string> spec) => Raw(spec);

    /// <inheritdoc/>
    public virtual QueryCommand Query() => new(Connection);
    IQueryCommand IRecordsGate.Query() => Query();

    /// <inheritdoc/>
    public virtual QueryCommand Query(params Func<dynamic, object>[] specs) => Query().From(specs);
    IQueryCommand IRecordsGate.From(params Func<dynamic, object>[] specs) => Query(specs);

    /// <inheritdoc/>
    public virtual QueryCommand Query(params Func<dynamic, string>[] specs) => Query().From(specs);
    IQueryCommand IRecordsGate.From(params Func<dynamic, string>[] specs) => Query(specs);

    /// <inheritdoc/>
    public virtual InsertCommand Insert(Func<dynamic, object> table) => new(Connection, table);
    IInsertCommand IRecordsGate.Insert(Func<dynamic, object> table) => Insert(table);

    /// <inheritdoc/>
    public virtual InsertCommand Insert(Func<dynamic, string> table) => new(Connection, table);
    IInsertCommand IRecordsGate.Insert(Func<dynamic, string> table) => Insert(table);

    /// <inheritdoc/>
    public virtual UpdateCommand Update(Func<dynamic, object> table) => new(Connection, table);
    IUpdateCommand IRecordsGate.Update(Func<dynamic, object> table) => Update(table);

    /// <inheritdoc/>
    public virtual UpdateCommand Update(Func<dynamic, string> table) => new(Connection, table);
    IUpdateCommand IRecordsGate.Update(Func<dynamic, string> table) => Update(table);

    /// <inheritdoc/>
    public virtual DeleteCommand Delete(Func<dynamic, object> table) => new(Connection, table);
    IDeleteCommand IRecordsGate.Delete(Func<dynamic, object> table) => Delete(table);

    /// <inheritdoc/>
    public virtual DeleteCommand Delete(Func<dynamic, string> table) => new(Connection, table);
    IDeleteCommand IRecordsGate.Delete(Func<dynamic, string> table) => Delete(table);
}