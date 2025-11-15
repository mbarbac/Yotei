namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database command that can be executed under its associated connection.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The effective locale used by this instance, which may be the one explicitly assigned to
    /// it, or the one of its associated connection.
    /// </summary>
    ILocale Locale { get; }

    /// <summary>
    /// The locale used by this instance, or a default '<c>null</c>' value to indicate that the
    /// one of the associated connection shall be used instead.
    /// </summary>
    [With] ILocale? RawLocale { get; set; }

    /// <summary>
    /// Determines if this command is in a execution-ready state, or not.
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
    /// Clears this command.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}