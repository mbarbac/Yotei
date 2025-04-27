namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a builder for <see cref="ICommand"/> instances.
/// </summary>
[Cloneable]
public partial interface ICommandBuilder
{
    /// <summary>
    /// Returns a new instance based upon the contents captured by this builder.
    /// </summary>
    /// <returns></returns>
    ICommand ToInstance();

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

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

    // ----------------------------------------------------

    /// <summary>
    /// Inconditionally replaces the text in this instance with the new given one, without any
    /// attempt of matching the names of the existing parameters with any text specification.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Whether changes have been made or not.</returns>
    bool ReplaceText(string? text);

    /// <summary>
    /// Inconditionally replaces the collection of parameters captured by this instance with the
    /// new given one, without any attempt to match their names or ordinals with the any possible
    /// specifications in the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>Whether changes have been made or not.</returns>
    bool ReplaceParameters(IEnumerable<IParameter> range);

    /// <summary>
    /// Inconditionally replaces the collection of parameters captured by this instance with the
    /// new ones obtained from the given range of values, without any attempt to match their names
    /// or ordinals with the possible specifications in the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>Whether changes have been made or not.</returns>
    bool ReplaceValues(params object?[] range);

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this instance the text and parameters of the given source.
    /// <br/> This method may change the names of the added parameters, and their corresponding
    /// text representation, if any collides with an existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>Whether changes have been made or not.</returns>
    bool Add(ICommand source);

    /// <summary>
    /// Adds to this instance the text and parameters of the given source.
    /// <br/> This method may change the names of the added parameters, and their corresponding
    /// text representation, if any collides with an existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>Whether changes have been made or not.</returns>
    bool Add(ICommandBuilder source);

    /// <summary>
    /// Adds to the contents of this instance the given text and elements. If text is null, then
    /// the range of elements is just captured without any attempt of matching their names with
    /// any text specification. If no elements are given, them the text is just captured without
    /// intercepting any dangling specification. Otherwise, the names or the ordinal positions
    /// of the elements must match with the corresponding specifications in the given text.
    /// <br/> Specifications can either be positional '{n}' ones (where 'n' is the ordinal in the
    /// collections of elements), or named '{name} ones (where 'name' can begin or not with the
    /// engine's parameter prefix). Named specifications, if used, must match with the name of
    /// any of the given elements.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns>Whether changes have been made or not.</returns>
    bool Add(string? text, params object?[] range);

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns>Whether changes have been made or not.</returns>
    bool Clear();
}