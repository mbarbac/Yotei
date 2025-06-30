namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database command that can be executed using a given connection.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    [With] IConnection Connection { get; }

    /// <summary>
    /// The locale to use with culture-sensitive objects in the database.
    /// </summary>
    [With] Locale Locale { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the state of this instance is execution ready, or not.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Obtains the information needed to run a command, using its default iterable mode.
    /// </summary>
    /// <returns></returns>
    ICommandInfo GetCommandInfo();

    /// <summary>
    /// Obtains the information needed to run a command, using the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo GetCommandInfo(bool iterable);

    // ----------------------------------------------------

    /// <summary>
    /// Clears the contents captured by this instance.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}