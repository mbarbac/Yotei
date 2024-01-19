namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed to execute a database command.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// The command text.
    /// </summary>
    string CommandText { get; }

    /// <summary>
    /// The command parameters.
    /// </summary>
    IParameterList Parameters { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given text to this instance.
    /// <br/> Callers must add a heading space if such is neccesary.
    /// </summary>
    /// <param name="text"></param>
    void Add(string? text);

    /// <summary>
    /// Adds the given parameter to this instance.
    /// </summary>
    /// <param name="parameter"></param>
    void Add(IParameter parameter);

    /// <summary>
    /// Adds the parameters from the given range to this instance.
    /// </summary>
    /// <param name="range"></param>
    void Add(IEnumerable<IParameter> range);

    /// <summary>
    /// Adds to this instance the given text and parameters from the optional array of arguments.
    /// <br/> Text can be null if not used. Callers must add a heading space if needed.
    /// <br/> Arguments, if used, must be encoded in the text using:
    /// <br/> - The '{name}' of a regular parameter included in the optional array.
    /// <br/> - The '{name}' of the unique property of an anonymous type in the optional array, as
    /// in 'new { name = value }'.
    /// <br/> - A '{n}' positional specification, where 'n' is the index of the value in the given
    /// optional array.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    void Add(string? text, params object?[] args);

    /// <summary>
    /// Adds to this instance the text and parameters from the given source. Duplicated parameter
    /// names are transformed into new ones generated automatically.
    /// </summary>
    /// <param name="source"></param>
    void Add(ICommandInfo source);

    /// <summary>
    /// Clears the contents of this instance.
    /// </summary>
    void Clear();
}