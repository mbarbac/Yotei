namespace Yotei.ORM;

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
    /// Returns a new instance where the existing text has been inconditionally replaced by
    /// the new given one, without any attempt of matching any text specifications with the
    /// names of the existing parameters.
    /// <br/> No bracket '{...}' specifications are allowed.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the existing collecion of parameters has been replaced
    /// inconditionally by the one obtained from the given range of values, without any attempt
    /// of matching their names with any text specification.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();

    /// <summary>
    /// Returns a new instance where the contents from the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents from the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and the collection of parameters obtained
    /// from the given range of values, if any, have been added to the original contents.
    /// <br/>- If <paramref name="text"/> is null, then the range of values is just captured
    /// without any attempts of matching their names with bracket specifications. Similarly,
    /// if <paramref name="range"/> is empty, then the text is captured without intercepting
    /// any dangling specifications.
    /// <br/>- Parameter specifications must always be bracket ones, either positional '{n}'
    /// or named '{name}' ones, where name contains the name of the parameter or the name of
    /// the unique property of an anonymous item. In both cases, 'name' may or may not start
    /// with the engine parameters' prefix.
    /// <br/>- No unused parameters are allowed, neither dangling bracket specifications.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[]? range);
}