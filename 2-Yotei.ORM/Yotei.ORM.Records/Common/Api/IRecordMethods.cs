namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides access to the records-oriented capabilities of a given connection.
/// </summary>
public interface IRecordMethods
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Returns an object that can execute the given command and enumerate the records produced
    /// by that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICommandEnumerator CreateCommandEnumerator(IEnumerableCommand command, CancellationToken token = default);

    /// <summary>
    /// Returns an object that can execute the given command and return the integer produce by
    /// that execution.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandExecutor CreateCommandExecutor(IExecutableCommand command);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new command of the appropriate type for this instance.
    /// </summary>
    /// <returns></returns>
    IRawCommand Raw();

    /// <summary>
    /// Returns a new command of the appropriate type for this instance.
    /// <br/> If the text is null, then it is ignored.
    /// <br/> If the optional list of arguments is used, then their associated names or positions
    /// must be referenced in the given text, and all arguments must be used. Each can be:
    /// <br/> - a raw value, that appears in the text agains a '{n}' positional specification. A
    /// new parameter is added using a name generated automatically.
    /// <br/> - a regular parameter, that appears in the text either by its '{name}' or by its
    /// '{n}' positional specification. If that name is a duplicated one, then an exception is
    /// thrown.
    /// <br/> - an anonymous type, that appears in the text either by the '{name}' of its unique
    /// property, or by its '{n}' positional specification. If that name does not start by the
    /// engine's parameter prefix, then it is added automatically.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Raw(string? specs, params object?[] args);
}