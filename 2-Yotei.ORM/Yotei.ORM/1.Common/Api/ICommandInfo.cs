namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed to execute a command.
/// </summary>
public interface ICommandInfo
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
    /// Returns a new instance where the given text has been added to the original one. Callers
    /// must add a heading space to separate from the former contents if such is needed.
    /// <br/> The text may contain parameter placeholders not yet used.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo Add(string text);

    /// <summary>
    /// Returns a new instance where the parameters from the given range have been added to the
    /// original ones. If any has a name of an existing one, then an exception is thrown.
    /// <br/> On the flip side, you can add parameters whose name are not yet in the text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance with the text and arguments of the given source added to the
    /// original ones. The actual names of the added parameters may change to prevent name
    /// colisions.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance with the given text and parameters added to the original one.
    /// The actual names of the added parameters may change to prevent name colisions.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string text, IParameterList range);

    /// <summary>
    /// Returns a new instance with the given text and optional arguments added to the original
    /// contents. Callers must add a heading space to separate from the former contents if such
    /// is needed. If used, the arguments must be encoded using a '{n}' positional specification
    /// in the text, where 'n' refers to the index in that array of arguments. The specification
    /// is then changed based upon the concrete argument type:
    /// <br/>- regular parameter (its unmodified name is used),
    /// <br/>- anonymous type (the name of its unique property is used, prefix is added if needed),
    /// <br/>- any other value (the next available parameter name).
    /// <br/> In addion, text cannot be null, and unused optional arguments are not allowed.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    ICommandInfo Add(string text, params object?[] args);

    /// <summary>
    /// Returns a new instance with the original text and parameters removed.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}