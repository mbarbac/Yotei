namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a database command whose contents are explicitly set.
/// </summary>
public partial interface IRawCommand : ICommand, IEnumerableCommand, IExecutableCommand
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    new IRawCommand Clone();

    /// <inheritdoc cref="ICommand.WithConnection(IConnection)"/>
    new IRawCommand WithConnection(IConnection value);

    /// <inheritdoc cref="ICommand.WithLocale(Locale)"/>
    new IRawCommand WithLocale(Locale value);

    // ----------------------------------------------------

    /// <inheritdoc cref="ICommand.Clear"/>
    new IRawCommand Clear();

    /// <summary>
    /// Adds to this instance the contents obtained from both parsing the given dynamic lambda
    /// expression, and the optional collection of values for the command arguments (which are
    /// used only when the expression resolves into a string).
    /// <br/> If any values are used, then they must be encoded in the given text using either a
    /// '{n}' positional specification or a '{name}' named one. In the later case, 'name' may or
    /// may not start with the engine's prefix. Unused values or dangling specifications are not
    /// allowed.
    /// <br/> Returns a reference to itself to support fluent syntax usage.
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRawCommand Append(Func<dynamic, object> spec, params object?[]? range);
}