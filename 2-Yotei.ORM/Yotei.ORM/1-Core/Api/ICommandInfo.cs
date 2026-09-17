namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the information needed by an underlying database to run a command.
/// <br/> Instance of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The command text carried by this instance.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// The collection of command parameters carried by this instance.
    /// </summary>
    IParameterList Parameters { get; }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Determines if this instance is in a consistent state or not.
    /// <br/> Empty instances are consistent by definition, but not execution ready.
    /// </summary>
    bool IsConsistent { get; }

    /// <summary>
    /// Determines if this instance is in an execution-ready state, or not.
    /// <br/> Inheritors shall invoke the base one first.
    /// </summary>
    bool IsValid { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added
    /// to it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>A new copy if  changes have been made. This instance otherwise.</returns>
    ICommandInfo Add(ICommandInfo source);

    /// <summary>
    /// Returns a copy of this instance where the contents of the given source have been added
    /// to it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>A new copy if  changes have been made. This instance otherwise.</returns>
    ICommandInfo Add(IBuilder source);

    /// <summary>
    /// Returns a copy of this instance where given text and the collection of parameters obtained
    /// from the given optional values have been added to it.
    /// <br/> If the text is null, then it is ignored.
    /// <br/> If text is not null, then the values must be encoded using either a positional '{n}'
    /// specification, or a named '{name}' one (where if 'name' is not prefixed with the engine's
    /// prefix, it is added automatically).
    /// </summary>
    /// <param name="text"></param>
    /// <param name="values"></param>
    /// <returns>A new copy if  changes have been made. This instance otherwise.</returns>
    ICommandInfo Add(string? text, params object?[]? values);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where text carried by this instance has been replaced
    /// by the given one.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>A new copy if  changes have been made. This instance otherwise.</returns>
    ICommandInfo ReplaceText(string? text);

    /// <summary>
    /// Returns a copy of this instance where the collection of parameters carried by this
    /// instance has been replaced by the one obtained from the given values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns>A new copy if  changes have been made. This instance otherwise.</returns>
    ICommandInfo ReplaceValues(params object?[]? values);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance its contents have been cleared.
    /// </summary>
    /// <returns>A new copy if  changes have been made. This instance otherwise.</returns>
    ICommandInfo Clear();
}