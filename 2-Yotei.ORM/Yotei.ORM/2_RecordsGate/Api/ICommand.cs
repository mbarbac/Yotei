namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a database command that can be executed against its associated connection.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Gets or sets the culture-sensitive locale to use with this command.
    /// <br/> When not explicitly set, its value defaults to the current culture.
    /// </summary>
    Locale Locale { get; set; }

    // ----------------------------------------------------

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