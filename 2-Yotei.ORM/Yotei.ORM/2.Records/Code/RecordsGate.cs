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
    public virtual IRawCommand Raw() => new RawCommand(Connection);

    /// <inheritdoc/>
    public virtual IRawCommand Raw(
        string? text, params object?[] args) => new RawCommand(Connection, text, args);

    /// <inheritdoc/>
    public virtual IRawCommand Raw(
        Func<dynamic, object> spec) => new RawCommand(Connection, spec);

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
}