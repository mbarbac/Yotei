namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the information needed by an underlying database to run a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Gets a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The actual text carried by this instance.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The actual collection of parameters carried by this instance.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    // ------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source command, using its
    /// default iterable mode, have been added to it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source command, using the
    /// requested iterable mode, have been added to it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommand source, bool iterable);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added to
    /// it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added to
    /// it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a copy of this instance where the given text and the parameters obtained from the
    /// given range of values have been added to it. If used, the parameters shall be encoded in
    /// the given text using either a positional '{n}' specification, or a '{name}' named one. The
    /// given text and values combination must represent a consistent state.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    ICommandInfo Add(string text, params object?[]? values);

    // ------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the given text, without validating its consistency,
    /// has been added to it.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo AddText(string text);

    /// <summary>
    /// Returns a copy of this instance where the parameters obtained from the given range of
    /// values, without validating their consistency, have been added to it.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    ICommandInfo AddValues(params object?[]? values);

    /// <summary>
    /// Returns a copy of this instance where the original text has been replaced by the given
    /// one, without validating its consistency.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string text);

    /// <summary>
    /// Returns a copy of this instance where the original collection of parameters has been
    /// replaced by the one obtained from the given range of values, without validating its
    /// consistency.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? values);

    /// <summary>
    /// Returns a copy of this instance where all the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();
}