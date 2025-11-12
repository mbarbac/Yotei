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

    // ----------------------------------------------------

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

    /// <summary>
    /// Determines if this instance is or not in a consistent state (defined as so when the captured
    /// parameters match their normalized representation in the captured text).
    /// <br/> Instances may become not consistent when the 'Replace' methods are used.
    /// </summary>
    bool IsConsistent { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> This method accepts sources in inconsistent states.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> This method accepts sources in inconsistent states.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> This method accepts sources in inconsistent states.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given text and the collection of parameters obtained from
    /// the given range of values, if any, have been added to the original ones.
    /// <br/> If values are used, then they must be encoded in the given text using either a '{n}'
    /// positional specification, or a '{name}' named one (where 'name' may or may not start with
    /// the engine parameter prefix).
    /// <br/> Unused values or dangling specifications are not allowed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string text, params object?[]? range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original text has been replaced by the given one.
    /// <br/> This method does not try to match the names of the existing parameters with their
    /// representation in the given text, so this instance may end up in an inconsistent state.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string text, bool strict = true);

    /// <summary>
    /// Returns a new instance where the existing collection of parameters has been replaced by
    /// a new one obtained from the given range of values.
    /// <br/> This method does not try to match the names of the given parameters with the ones
    /// in the existing text, so this instance may end up in an inconsistent state.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceParameters(params object?[]? range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original contents have been cleared.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}