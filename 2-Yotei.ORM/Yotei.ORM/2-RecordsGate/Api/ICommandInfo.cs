namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the information needed by an underlying database to run a command.
/// <br/> Instances of this type are mutable ones to allow then to adapt to concrete scenarios.
/// <br/> The methods in this type may render its instances in an inconsistent state, which is
/// considered as an an acceptable artifact while the command is being constructed.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The actual text carried by this instance.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The actual collection of parameters carried by this instance.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Determines if this instance is in a consistent state or not.
    /// <br/> Note that empty instances are considered consistent.
    /// </summary>
    bool IsConsistent { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this instance the contents of the given source, using its default iterable mode.
    /// The name of its parameters are adjusted to prevent name collisions.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    bool Add(ICommand source);

    /// <summary>
    /// Adds to this instance the contents of the given source, using the requested iterable mode.
    /// The name of its parameters are adjusted to prevent name collisions.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    bool Add(ICommand source, bool iterable);

    /// <summary>
    /// Adds to this instance the contents of the given source, using its default iterable mode.
    /// The name of its parameters are adjusted to prevent name collisions.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    bool Add(ICommandInfo source);

    /// <summary>
    /// Adds to this instance the given text and the parameters obtained from the given range of
    /// values. If used, the parameters must be encoded in the given text using either a positional
    /// '{n}' form, or a named '{name}' one (where 'name' may or may not begin with the engine's
    /// prefix). No dangling specifications or unused parameters are allowed.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Add(string text, params object?[]? range);

    /// <summary>
    /// Adds to this instance the given text without performing any validations.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    bool AddText(string text);

    /// <summary>
    /// Adds to this instance the parameters obtained from the given range of values, without any
    /// matching of their names with the existing text.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool AddValues(params object?[]? range);

    /// <summary>
    /// Replaces the text of this instance with the given one without performing any validations.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    bool ReplaceText(string text);

    /// <summary>
    /// Replaces the parameters of this instance with the ones obtained from the given range of
    /// values, without performing any validations.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ReplaceValues(params object?[]? range);

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns></returns>
    bool Clear();
}