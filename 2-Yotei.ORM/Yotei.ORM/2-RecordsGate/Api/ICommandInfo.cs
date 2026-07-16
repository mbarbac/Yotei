namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the information needed an underlying database needs to run a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The text of the command.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The collection of parameters carried by this command.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added
    /// to it, using its default iterable mode.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added
    /// to it, using the requested iterable mode.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source, bool iterable);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added
    /// to it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added
    /// to it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a copy of this instance where the given text and the collection of parameters
    /// obtained from the given values have been added to it.
    /// <br/> If both text and values are used, the later ones must be enconded into the text
    /// using either a positional '{n}' form, or a named '{name}' one, where 'name' may or
    /// may not start with the engine's prefix.
    /// <br/> If text is null, then only the values are taken into consideration.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the original text has been replaced by the new given
    /// one.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string text);

    /// <summary>
    /// Returns a copy of this instance where the original collection of parameters has been
    /// replaced by the one obtained from the given collection of values.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? range);

    /// <summary>
    /// Returns a copy of this instance where its original text and parametes are cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}