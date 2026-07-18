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
}