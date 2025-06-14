namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database command that can be executed using a given connection.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <summary>
    /// Adds to this instance the the given text and optional arguments, if any.
    /// <br/> If the text is null, the it is ignored and the optional arguments are captured
    /// without any attempts of matching their names with any text specification.
    /// <br/> Similarly, if there are no elements in the optional list of arguments, the text
    /// is captured without intercepting any dangling spcifications.
    /// <br/> Otherwise, specifications are always bracket ones, either positional '{n}' ones,
    /// of named '{name}' ones (where name may or may not start with the engine parameters'
    /// prefix). No unused elements in the optional list of arguments are allowed, neither
    /// dangling specifications in the given text.
    /// <br/> If text is not null, then a space is added if needed.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Append(string? text, params object?[]? args);

    /// <summary>
    /// Adds to this instance the text and parameters obtained from parsing the given dynamic
    /// lambda expression.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IRawCommand Append<T>(Func<dynamic, T> spec);

    /// <inheritdoc cref="ICommand.Clear"/>
    new IRawCommand Clear();
}