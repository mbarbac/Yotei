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
    /// The locale used by this command, or null (by default) to use the one of its associated
    /// connection.
    /// </summary>
    [With] ILocale? Locale { get; set; }

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