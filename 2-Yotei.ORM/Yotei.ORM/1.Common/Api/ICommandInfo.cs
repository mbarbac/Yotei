namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed to execute a database command.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The text of the command.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The parameters of the command.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given text added to the original one.
    /// <br/> Text can be null if not used.
    /// <br/> Callers must add a heading space in that text if needed.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text);

    /// <summary>
    /// Returns a new instance with the given range of parameters added to the original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance with the given text and the parameters obtained from the optional
    /// list of arguments added to the original one.
    /// <br/> Text can be null if not used.
    /// <br/> Callers must add a heading space in that text if needed.
    /// <br/> Arguments, if used, must be encoded within that text using:
    /// <br/> - The '{name}' of a regular parameter in the optional list.
    /// <br/> - The '{name}' of the unique property of an anonymous type in the optional list,
    /// ie: 'new { name = value }'.
    /// <br/> - A '{n}' positional specification, 'n' being the index of the value in the
    /// optional list.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[] args);

    /// <summary>
    /// Returns a new instance with the text and arguments of the given source added to the
    /// original instance.
    /// <br/> The names of the added arguments may be adjusted in case they were the same of any
    /// existing one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance with the original text and parameters removed.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}