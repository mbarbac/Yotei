namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that can be executed against its associated connection.
/// </summary>
[Cloneable]
[WithGenerator]
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <summary>
    /// Appends to this command the given text and optional arguments.
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
    IRawCommand Append(string? specs, params object?[] args);

    /// <summary>
    /// Appends to this command the text and optional arguments obtained from parsing the given
    /// dynamic lambda expression.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IRawCommand Append(Func<dynamic, object> specs);

    /// <summary>
    /// Clears this command.
    /// </summary>
    /// <returns></returns>
    IRawCommand Clear();
}