namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a database command whose raw SQL contents are explicitly set.
/// </summary>
[Cloneable]
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <summary>
    /// Appends to the contents of this instance the ones from the given command info.
    /// <br/> Returns a reference to itself to support fluent syntax usage.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    IRawCommand Append(ICommandInfo info);

    /// <summary>
    /// Appends to the contents of this instance the ones captured from parsing the given dynamic
    /// lambda expression.
    /// <br/> Returns a reference to itself to support fluent syntax usage.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IRawCommand Append(Func<dynamic, object> spec);

    /// <summary>
    /// Appends to the contents of this instance the ones captured from the given text and the
    /// optional collection of optional arguments, if any. If used, they must be encoded in that
    /// text using either a positional '{n}' specification, or a '{name}' named one. The text and
    /// values combination must represent a consistent state.
    /// <br/> Returns a reference to itself to support fluent syntax usage.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Append(string text, params object?[]? args);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IEnumerableCommand.Skip(int)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new IRawCommand Skip(int value);

    /// <summary>
    /// <inheritdoc cref="IEnumerableCommand.Take(int)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new IRawCommand Take(int value);

    /// <summary>
    /// <inheritdoc cref="ICommand.Clear"/>
    /// </summary>
    /// <returns></returns>
    new IRawCommand Clear();
}