namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the information needed to run a command.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder GetBuilder();

    // ------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The command's text.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The command's parameters.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original text has been replaced by the new given one,
    /// without any attempts of matching any text specifications with the names of existing
    /// parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the original collection of parameters has been replaced
    /// with a new one obtained from the given range of values, without any attempts of matching
    /// their names with any existing specifications.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and the collection of parameters obtained
    /// from the given range of values have been added to the original ones.
    /// <br/> If text is null, then the range of value is captured without any attempts of
    /// matching their names with any text specifications. Similarly, if there are no items
    /// in the range of values, then the text is captured without intercepting any dangling
    /// specifications.
    /// <br/> Parameter specifications in the given text must always be bracket ones, either
    /// positional '{n}' or named '{name}' ones. Positional ones refer to the ordinal of the
    /// element in the range of values. Named ones contain the name of the parameter, or the
    /// name of the unique property of the given anonymous item. In both cases, 'name' may or
    /// may not start with the engine parameters' prefix, which is always used in the captured
    /// ones. If no bracketed, you can use raw parameter names as you wish.
    /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[]? range);
}