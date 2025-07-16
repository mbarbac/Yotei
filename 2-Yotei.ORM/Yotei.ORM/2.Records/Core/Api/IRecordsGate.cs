namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides access to the record-oriented capabilities of the associated connection.
/// </summary>
public interface IRecordsGate
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }
    
    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute the given command, and enumerate through the records
    /// produced by that execution, if any.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// Returns an object that can execute the given command, and return the integer produced
    /// by that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandExecutor CreateCommandExecutor(IExecutableCommand command);

    // ----------------------------------------------------

    /// <summary>
    /// Factory method to create an object with the ability of parsing db-token chains returning
    /// the <see cref="ICommandInfo"/> object that represents that chain for the underlying
    /// database engine.
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="useNullString"></param>
    /// <param name="captureValues"></param>
    /// <param name="convertValues"></param>
    /// <param name="useQuotes"></param>
    /// <param name="useTerminators"></param>
    /// <param name="rangeSeparator"></param>
    /// <returns></returns>
    DbTokenVisitor CreateDbTokenVisitor(
        Locale? locale = null,
        bool useNullString = DbTokenVisitor.USENULLSTRING,
        bool captureValues = DbTokenVisitor.CAPTUREVALUES,
        bool convertValues = DbTokenVisitor.CONVERTVALUES,
        bool useQuotes = DbTokenVisitor.USEQUOTES,
        bool useTerminators = DbTokenVisitor.USETERMINATORS,
        string? rangeSeparator = DbTokenVisitor.RANGESEPARATOR);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new empty RAW command.
    /// </summary>
    /// <returns></returns>
    IRawCommand Raw();

    /// <summary>
    /// Returns a new RAW command using the given dynamic lambda expression and the optional
    /// collection of arguments.
    /// <br/> If an optional collection of arguments is provided, then the specification must
    /// resolve to a string, and those arguments must be encoded in that string using either a
    /// positional '{n}' or named '{name}' specification. If it is provided but the expression
    /// does not resolve to a string, or if there is a mismatch between the encoded arguments
    /// and the actual ones in that collection, then an exception is thrown.
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Raw(Func<dynamic, object> spec, params object?[]? args);
}