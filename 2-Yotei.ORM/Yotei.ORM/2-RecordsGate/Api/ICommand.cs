namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a database command that can be executed by its associated connection.
/// <br/> Instances of this type are typically mutable ones.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Determines whether this instance is in a valid runnable state, or not.
    /// </summary>
    bool IsValid { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the information needed to run this command, using its default iterable mode.
    /// </summary>
    /// <returns></returns>
    ICommandInfo GetCommandInfo();

    /// <summary>
    /// Obtains the information needed to run this command, using the requested iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo GetCommandInfo(bool iterable);

    // ----------------------------------------------------

    /// <summary>
    /// Clears the contents of this instance.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}