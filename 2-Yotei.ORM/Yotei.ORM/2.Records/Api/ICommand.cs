namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that can be executed against a database.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The locale used of the culture-sensitive objected in the database.
    /// <br/> Its default value is the one of the thread that created this instance.
    /// </summary>
    [With] Locale Locale { get; }

    /// <summary>
    /// Obtains the information needed to run this command, using its default iterable mode.
    /// </summary>
    /// <returns></returns>
    ICommandInfo GetCommandInfo();

    /// <summary>
    /// Obtains the information needed to run this command, using the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo GetCommandInfo(bool iterable);
}