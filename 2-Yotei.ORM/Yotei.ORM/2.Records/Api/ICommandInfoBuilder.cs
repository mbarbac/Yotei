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

    /// <inheritdoc cref="ICommandInfo.Text"/>
    string Text { get; }

    /// <inheritdoc cref="ICommandInfo.Parameters"/>
    IParameterList Parameters { get; }

    /// <inheritdoc cref="ICommandInfo.IsEmpty"/>
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
    /// one, without trying to match it with the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ReplaceParameters(IEnumerable<IParameter> range);

    /// <summary>
    /// Inconditionally replaces the collection of parameters in this instance with the new given
    /// range of values, without trying to match them with the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ReplaceValues(params object?[] range);

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
    /// Adds to the contents of this instance the given text and elements. If the text is null
    /// then the elements are just captured without trying to match their names with any text
    /// specifications. Similarly, if no elements were given, the text is just captured without
    /// intercepting dangling specifications. Otherwise, the names or the ordinal positions of
    /// the given elements must match, and be used, as specifications in the given text.
    /// <br/> Specifications can either be positional '{n}' ones, where 'n' is the ordinal in the
    /// collection of elements, or named '{name}' ones, where 'name' can begin or not with the
    /// engines' parameter prefix. Named specifications must match with the name of any of the
    /// given elements, even if afterwards these names may change to prevent duplicates with the
    /// original captured parameters.
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