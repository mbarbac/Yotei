namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents database command whose contents can be explicitly set.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <summary>
    /// Appends to this instance the contents obtained from parsing the given dynamic lambda
    /// expression.
    /// <br/> If an optional collection of arguments is provided, then the specification must
    /// resolve to a string, and those arguments must be encoded in that string using either a
    /// positional '{n}' or named '{name}' specification. If it is provided but the expression
    /// does not resolve to a string, or if there is a mismatch between the encoded arguments
    /// and the actual ones in that collection, then an exception is thrown.
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Append(Func<dynamic, object> spec, params object?[]? args);

    /// <inheritdoc cref="ICommand.Clear"/>
    new IRawCommand Clear();
}