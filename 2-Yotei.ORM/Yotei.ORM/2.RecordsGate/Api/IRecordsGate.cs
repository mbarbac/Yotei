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
    ICommandEnumerator CreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// Returns an object that can execute the given command, and return the integer produced
    /// by that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandExecutor CreateCommandExecutor(IExecutableCommand command);

    // ----------------------------------------------------

    /// <summary>
    /// Factory method to create an internal object with the ability of parsing database token
    /// chains that return the the <see cref="ICommandInfo"/> object that represents them for
    /// the underlying database engine.
    /// </summary>
    /// <param name="locale"></param>
    /// <returns></returns>
    DbTokenVisitor CreateDbTokenVisitor(Locale locale);
    
    // ----------------------------------------------------

    /// <summary>
    /// Returns a new empty RAW command.
    /// </summary>
    /// <returns></returns>
    IRawCommand Raw();

    /// <summary>
    /// Returns a new RAW command using the contents obtained from both parsing the given dynamic
    /// lambda expression, and the optional collection of values for the command arguments (which
    /// are used only when the expression resolves into a string).
    /// <br/> If any values are used, then they must be encoded in the given text using either a
    /// '{n}' positional specification or a '{name}' named one. In the later case, 'name' may or
    /// may not start with the engine's prefix. Unused values or dangling specifications are not
    /// allowed.
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Raw(Func<dynamic, object> spec, params object?[]? args);
}