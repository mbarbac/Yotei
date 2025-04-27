namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that can be executed against an underlying database.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// Obtains a new builder using the contents of this instance
    /// </summary>
    /// <returns></returns>
    ICommandBuilder GetBuilder();

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
    /// Returns a new instance where the original text has been inconditionally replaced by
    /// the new given one, without any attempt of matching the names of the existing parameters
    /// with any text specification.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommand ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the original collection of parameters has been replaced
    /// inconditionally by the new given one, without any attempt to match their names or ordinals
    /// with the any possible specifications in the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommand ReplaceParameters(IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance where the original collection of parameters has been replaced
    /// inconditionally with the new one obtained from the given range of values, without any
    /// attempt to match their names or ordinals with the possible specifications in the 
    /// existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommand ReplaceValues(params object?[] range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given text and parameters have been added to the original
    /// one.
    /// <br/> This method may change the names of the added parameters, and their corresponding
    /// text representation, if any collides with an existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommand Add(ICommand source);

    /// <summary>
    /// Returns a new instance where the text and parameters of the given source have been added
    /// to the original one.
    /// <br/> This method may change the names of the added parameters, and their corresponding
    /// text representation, if any collides with an existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommand Add(ICommandBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and elements have been added to the original
    /// one. If text is null, then the range of elements is just captured without any attempt of
    /// matching their names with any text specification. If no elements are given, them the text
    /// is just captured without intercepting any dangling specification. Otherwise, the names or
    /// the ordinal positions of the elements must match with the corresponding specifications in
    /// the given text.
    /// <br/> Specifications can either be positional '{n}' ones (where 'n' is the ordinal in the
    /// collections of elements), or named '{name} ones (where 'name' can begin or not with the
    /// engine's parameter prefix). Named specifications, if used, must match with the name of
    /// any of the given elements.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommand Add(string? text, params object?[] range);

    /// <summary>
    /// Returns a new instance where the original text and collection of parameters have been
    /// both cleared.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}