namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed to run a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The captured command's text.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The captured command's parameters.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source);

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
    /// Returns a new instance where the given text and a collection of parameters obtained from
    /// the given range of values, if any, have been added to the original ones.
    /// <br/>- If both text and values are used, then those values must be encoded in the text
    /// using bracket specifications, either positional '{n}' or named '{name}' ones, where
    /// the name may or may not start with the engine's parameter prefix. Unused values or
    /// dangling specifications are not allowed.
    /// <br/>- If 'text' is null, then the range of values is just captured without trying to
    /// match their names with any bracket specification. Similarly, if 'range' is empty, then
    /// the text is just captured without intercepting dangling specifications.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the existing text has been replaced by the new given one,
    /// without trying to match the names of the captured parameters in that new text.
    /// <br/> No bracket '{...}' specifications are allowed.
    /// <br/> Incorrect usage of this method may render this instance unusable.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the existing collection of parameters has been replaced by
    /// a new one obtained from the given range of values (including empty ones), without trying
    /// to match their names with any bracket specifications in the existing text.
    /// <br/> Incorrect usage of this method may render this instance unusable.
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
}