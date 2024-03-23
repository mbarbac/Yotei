namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information obtained from a command for its execution.
/// </summary>
public interface ICommandInfo
{
    /// <summary>
    /// The text of command that can be executed against its associated database.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The collection of parameters captured for the command.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given raw text has been added to the existing one.
    /// <br/> If the text is null, then it is ignored.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo AddText(string? text);

    /// <summary>
    /// Returns a new instance where the given parameters have been added to the existing ones.
    /// <br/> This method fails if the name of any of the given parameters is a duplicated one,
    /// but makes no validations that those names appear in the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo AddParameters(params IParameter[] range);

    /// <summary>
    /// Returns a new instance where the given raw text and parameters from the given range have
    /// been added to the existing ones.
    /// <br/> If the text is null, then it is ignored.
    /// <br/> If the optional list of arguments is used, then their associated names or positions
    /// must be referenced in the given text, and all arguments must be used. Each can be:
    /// <br/> - a raw value, that appears in the text agains a '{n}' positional specification. A
    /// new parameter is added using a name generated automatically.
    /// <br/> - a regular parameter, that appears in the text either by its '{name}' or by its
    /// '{n}' positional specification. If that name is a duplicated one, then an exception is
    /// thrown.
    /// <br/> - an anonymous type, that appears in the text either by the '{name}' of its unique
    /// property, or by its '{n}' positional specification. If that name does not start by the
    /// engine's parameter prefix, then it is added automatically.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[] range);

    /// <summary>
    /// Returns a new instance where the text and parameters from the given source have been
    /// added to the existing ones.
    /// <br/> This method may change the name of the added arguments, and their references in the
    /// added text, to prevent duplicated ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the text and parameters from the given source have been
    /// added to the existing ones, in addition to the given separator.
    /// <br/> This method may change the name of the added arguments, and their references in the
    /// added text, to prevent duplicated ones.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, ICommandInfo source);

    /// <summary>
    /// Returns a new empty instance.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}