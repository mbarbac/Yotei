namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a command that can be executed through its associated connection.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Gets the information needed to execute this command, using the default iterable mode of
    /// this instance.
    /// </summary>
    /// <returns></returns>
    ICommandInfo GetCommandInfo();

    /// <summary>
    /// Gets the information needed to execute this command, using the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo GetCommandInfo(bool iterable);
}