namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Provides access to the records-oriented capabilities of the associated connection.
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
    /// Returns a new empty RAW command.
    /// </summary>
    /// <returns></returns>
    IRawCommand Raw();

    /// <summary>
    /// Returns a new RAW command using the given text and optional arguments.
    /// <br/> If the text is null, the it is ignored and the optional arguments are captured
    /// without any attempts of matching their names with any text specification.
    /// <br/> Similarly, if there are no elements in the optional list of arguments, the text
    /// is captured without intercepting any dangling spcifications.
    /// <br/> Otherwise, specifications are always bracket ones, either positional '{n}' ones,
    /// of named '{name}' ones (where name may or may not start with the engine parameters'
    /// prefix). No unused elements in the optional list of arguments are allowed, neither
    /// dangling specifications in the given text.
    /// <br/> If text is not null, then a space is added if needed.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Raw(string? text, params object?[] args);

    /// <summary>
    /// Returns a new RAW command using the text and arguments obtained from parsing the given
    /// dynamic lambda expressions.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IRawCommand Raw(Func<dynamic, object?> spec);
}