namespace Yotei.ORM.Records;

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
    IBuilder GetBuilder();

    // ------------------------------------------------

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
    /// Returns a new instance where all the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    ICommandInfo Clear();

    /// <summary>
    /// Returns a new instance where the original text in this instance has been replaced by
    /// the new given one, without any attempt of matching any raw name specification with any
    /// existing parameter. No bracket specifications are allowed.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a new instance where the original collection of parameters in this instance has
    /// been inconditionally replaced by the new given one, without any attempt of matching their
    /// names with any specification in the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceParameters(IEnumerable<IParameter> range);

    /// <summary>
    /// Returns a new instance where the original collection of parameters in this instance has
    /// been inconditionally replaced by the one obtained from the given range of values, without
    /// without any attempt of matching their names with any specification in the existing text.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo ReplaceValues(params object?[]? range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to this
    /// instance. This method may change the name of the added parameters, and their associated
    /// text representations, to prevent name collisions with existing ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a new instance where the contents of the given source have been added to this
    /// instance. This method may change the name of the added parameters, and their associated
    /// text representations, to prevent name collisions with existing ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a new instance where given text and the collection of parameters obtained from
    /// the given range of values have been added to it.
    /// <br/> If text is null, then the range of values is just captured without any attempt
    /// of matching their names with any text specifications. Conversely, if no elements are
    /// given in the range, then the text is just captured without intercepting any dangling
    /// specifications.
    /// <br/> Parameter specifications in the text are always bracket ones, and can either be
    /// positional '{n}' or named '{name}' ones. Positional one refer to the ordinal of the
    /// element in the range of values. Named ones contain the name of the parameter or the
    /// unique property of the given anonymous item, and 'name' may or may not start with
    /// the engine parameters' prefix.
    /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
    /// <br/> You can use raw (non-bracketed) parameter names as you wish.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    ICommandInfo Add(string? text, params object?[]? range);
}