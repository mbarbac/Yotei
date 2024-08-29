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
    /// the given one, without trying to match it with the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceParameters(IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance where the original collection of parameters has been replaced with
    /// the new given ones, without trying to match them with the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[] range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the text and elements of the given one have been added to
    /// the original ones. The names of the added elements may change if they collide with any
    /// existing ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the text and elements of the given one have been added to
    /// the original ones. The names of the added elements may change if they collide with any
    /// existing ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfoBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and elements have been added to the original
    /// ones. If the text is null then the elements are just captured without trying to match
    /// their names with text specifications. Similarly, if no elements were given, the text is
    /// just captured without intercepting dangling specifications. Otherwise, the names or the
    /// ordinal positions of the given elements must match, and be used, as specifications in the
    /// given text.
    /// <br/> Specifications can either be positional '{n}' ones, where 'n' is the ordinal in the
    /// collection of elements, or named '{name}' ones, where 'name' can begin or not with the
    /// engines' parameter prefix. Named specifications must match with the name of any of the
    /// given elements, even if afterwards these names may change to prevent duplicates with the
    /// original captured parameters.
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