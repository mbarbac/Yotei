namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command whose contents are explicitly set.
/// </summary>
[Cloneable]
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <summary>
    /// Appends to this command the given text and optional arguments.
    /// <br/> Text can be null if it is not used.
    /// <br/> Arguments, if used, shall be encoded in the command text using:
    /// <br/> - The '{name}' of a regular parameter passed as an argument in the optional array,
    /// <br/> - The '{name}' of the unique property of an anonymous type passed as an argument in
    /// the optional array, as in 'new { name = value }',
    /// <br/> - A '{n}' positional form, where 'n' is the ordinal index of the associated value
    /// in the optional array of arguments.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Append(string? specs, params object?[] args);

    /// <summary>
    /// Appends to this command the text and arguments obtained from parsing the given dynamic
    /// lambda expression.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IRawCommand Append(Func<dynamic, object> specs);
}