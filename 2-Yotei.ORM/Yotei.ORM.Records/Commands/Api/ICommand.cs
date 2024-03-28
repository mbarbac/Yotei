namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command that can be executed against its associated connection.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The locale used by this command with culture-sensitive elements in the database. The
    /// default value of this property is null, meaning that the locales from its associated
    /// connection will be used instead.
    [WithGenerator]
    Locale? Locale { get; set; }

    /// <summary>
    /// Returns the locale to use by this command, either the one explicitly set, or the one
    /// from its associated connection.
    /// </summary>
    /// <returns></returns>
    Locale GetLocale();

    /// <summary>
    /// Gets the information needed to execute this command, using its default iterable mode.
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