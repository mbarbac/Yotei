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
    ICommandInfoBuilder ToBuilder();

    /// <summary>
    /// Obtains the current text captured by this instance.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Obtains the current list of parameters captured by this instance.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original text has been inconditionally replaced by the
    /// new given one, without trying to match it with the existing collection of parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the original collection of parameters has been replaced with
    /// the given ones, without trying to match it with the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceParameters(IEnumerable<IParameter> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the text and parameters of the given one have been added to
    /// the original ones. The names of the added parameters may change if they collide with any
    /// existing ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the text and parameters of the given one have been added to
    /// the original ones. The names of the added parameters may change if they collide with any
    /// existing ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfoBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and parameters have been added to the original
    /// ones.
    /// <br/>- If the text is null, then the parameters are just captured without trying to match
    /// them with any specifications in the text.
    /// <br/>- Similarly, if no parameters are given, the text is captured without validating any
    /// specifications.
    /// <br/>- Otherwise, if there are parameters with no corresponding specifications in the
    /// given text, of if the text contains specifications with no matching parameters, then an
    /// exception is thrown.
    /// <br/>- Specifications can either be positional '{n}' ones, where 'n' is the ordinal in the
    /// parameters' collection, or named '{name}' ones, where 'name' can begin or not with the
    /// engines' parameter prefix, matching either the name of a parameter in that collection, or
    /// with the name of the unique property of an anonymous type in that range.
    /// <br/>- They names of the added parameters may change if the collide with any existing
    /// ones.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[] range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original contents have been removed.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}