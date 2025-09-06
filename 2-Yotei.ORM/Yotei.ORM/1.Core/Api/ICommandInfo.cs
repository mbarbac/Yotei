namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed to run a command.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

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

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source, bool iterable);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and the collection of parameters obtained from
    /// the given range of values, if any, have been added to the original ones.
    /// <br/> If values are used, then they must be encoded in the given text using either a '{n}'
    /// positional specification or a '{name}' named one. In the later case, 'name' may or may not
    /// start with the engine's prefix. Unused values or dangling specifications are not allowed.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string text, params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given text has been added to the existing one, without
    /// intercepting dangling specifications.
    /// <br/> Incorrect usage of this method may render this instance in an inconsistent state.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo AddTextUnsafe(string text);

    /// <summary>
    /// Returns a new instance where the parameters obtained from the given collection of values,
    /// if any, have been added to the original ones, without trying to match their names with
    /// any existing bracket specification in the existing text.
    /// <br/> Incorrect usage of this method may render this instance in an inconsistent state.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo AddValuesUnsafe(params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the existing text has been replaced by the new given one,
    /// without trying to match the names of the captured parameters in that new text.
    /// <br/> No bracket '{...}' specifications are allowed.
    /// <br/> Incorrect usage of this method may render this instance in an inconsistent state.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceTextUnsafe(string text);

    /// <summary>
    /// Returns a new instance where the existing collection of parameters has been replaced by
    /// a new one obtained from the given range of values (including empty collections), without
    /// trying to match their names with any bracket specifications in the existing text.
    /// <br/> Incorrect usage of this method may render this instance in an inconsistent state.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValuesUnsafe(params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}