namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed by the underlying database to run a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The captured command's text, or an empty string.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The length of the captured command text.
    /// <br/> This property is provided for convenience reasons.
    /// </summary>
    int TextLen { get; }

    /// <summary>
    /// The captured command arguments, or an empty collection.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Gets the number of captured command arguments.
    /// <br/> This property is provided for convenience reasons.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source, bool iterable);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and the collection of parameters obtained from
    /// the given range of values, if any, have been added to the original ones.
    /// <br/> If both text and values are used, then the later shall be encoded in the text using
    /// either a positional '{n}' or a named '{name}' specification (where 'name' may or may not
    /// start with the engine prefix).
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[]? range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original text has been replaced by the given one. If it
    /// is a null one, then an empty string is used instead.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the existing collection of parameters has been replaced by
    /// a new one obtained from the given range of values.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original contents have been cleared.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}