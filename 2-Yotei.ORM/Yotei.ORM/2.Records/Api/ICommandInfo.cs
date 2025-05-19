namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the information needed to run a command.
/// </summary>
[Cloneable]
public partial interface ICommandInfo
{
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
}