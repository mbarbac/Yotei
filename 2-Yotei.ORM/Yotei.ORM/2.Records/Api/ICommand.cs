namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that can be executed using a given connection.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The locale to use when using culture-sensitive objects in the database.
    /// <br/> Its default value is the one of the thread that created this instance.
    /// </summary>
    [With] Locale Locale { get; }

    /// <summary>
    /// Obtains the information needed to execute this command, using its default iterable mode.
    /// </summary>
    /// <returns></returns>
    ICommandInfo GetCommandInfo();

    /// <summary>
    /// Obtains the information needed to execute this command, using the given iterable mode if
    /// possible.
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo GetCommandInfo(bool iterable);

    // ----------------------------------------------------

    /// <summary>
    /// Clears all the contents of this instance.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}