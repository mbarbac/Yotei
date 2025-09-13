namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed to run a command.
/// </summary>
public partial interface ICommandInfo
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    ICommandInfo Clone();

    // ----------------------------------------------------

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

    /// <summary>
    /// Determines if this instance is in a consistent state, defined as when the parameters
    /// match with their normalized representation in text.
    /// </summary>
    /// <returns></returns>
    bool IsConsistent();

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> This method accepts sources that are in an inconsistent state.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source, bool iterable);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> This method accepts sources that are in an inconsistent state.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to the
    /// original ones.
    /// <br/> This method accepts sources that are in an inconsistent state.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a new instance where the given text and the collection of parameters obtained from
    /// the given range of values, if any, have been added to the original ones.
    /// <br/> If values are used, then they must be encoded in the given text using either a '{n}'
    /// positional specification or a '{name}' named one. In the later case, 'name' may or may not
    /// start with the engine's prefix.
    /// <br/> Unused values or dangling specifications are not allowed.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string text, params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the existing text has been replaced by the new given one,
    /// without any attempt to match any existing parameter name with any bracket specification
    /// in the given text.
    /// <br/> This method may cause this instance to end up in an inconsistent state, so it
    /// should be used with caution.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string text);

    /// <summary>
    /// Returns a new instance where the existing collection of parameters has been replaced by
    /// the one obtained from the given range of values, if any, including empty ranges, without
    /// any attempt to match their names or ordinal positions with any bracket specification in
    /// the existing text.
    /// <br/> This method may cause this instance to end up in an inconsistent state, so it
    /// should be used with caution.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}