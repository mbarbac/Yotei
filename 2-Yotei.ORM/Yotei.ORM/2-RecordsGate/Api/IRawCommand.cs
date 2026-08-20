namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a database command whose contents are explicitly set.
/// </summary>
[Cloneable]
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <summary>
    /// Appends to the contents of this instance the ones specified by the given info instance.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    IRawCommand Append(ICommandInfo info);

    /// <summary>
    /// Appends to the contents of this instance the ones captured from the given text and the
    /// optional collection of associated command arguments, if any. If used, they must be encoded
    /// in the given text using either a '{n}' positional specification, of a '{name}' named one.
    /// In the later case, 'name' may or may not start with the engine's prefix.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Append(string text, params object?[]? args);

    /// <summary>
    /// Append to the contents of this instance the ones captured from parsing the given dynamic
    /// lambda expression.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IRawCommand Append(Func<dynamic, object> spec);

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICommand.Clear"/>
    /// </summary>
    /// <returns></returns>
    new IRawCommand Clear();
}