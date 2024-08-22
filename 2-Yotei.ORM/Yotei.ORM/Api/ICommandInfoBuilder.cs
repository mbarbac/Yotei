namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a builder for <see cref="ICommandInfo"/> instances.
/// </summary>
[Cloneable]
public partial interface ICommandInfoBuilder
{
    /// <summary>
    /// Returns a new instance based upon the contents captured by this one.
    /// </summary>
    /// <returns></returns>
    ICommandInfo ToInstance();

    // <summary>
    /// Obtains the current text captured by this instance.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Obtains the current list of parameters captured by this instance.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is currently an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Inconditionally replaces the text in this instance with the new given one, without trying
    /// to match it with the existing collection of parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    bool ReplaceText(string? text);

    /// <summary>
    /// Inconditionally replaces the collection of parameters in this instance with the new given
    /// ones, without trying to match it with the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ReplaceParameters(IEnumerable<IParameter> range);

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this instance the text and parameters of the given source one. The names of the
    /// added parameters may change if they collide with any existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    bool Add(ICommandInfo source);

    /// <summary>
    /// Adds to this instance the text and parameters of the given source one. The names of the
    /// added parameters may change if they collide with any existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    bool Add(ICommandInfoBuilder source);

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
    bool Add(string? text, params object?[] range);

    // ----------------------------------------------------

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns></returns>
    bool Clear();
}