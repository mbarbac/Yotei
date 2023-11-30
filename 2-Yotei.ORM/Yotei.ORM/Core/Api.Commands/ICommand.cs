namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a command that can be executed against an underlying database.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The locale used by this command with the culture-sensitive objects in the underlying
    /// database. The default value of this property is obtained from the current culture of
    /// the thread from where this instance was created.
    /// </summary>
    Locale Locale { get; set; }

    /// <summary>
    /// Obtains the actual command text that can be executed agains the underlying database,
    /// using the default iterable mode of this command.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(out IParameterList parameters);

    /// <summary>
    /// Obtains the actual command text that can be executed agains the underlying database,
    /// using the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(bool iterable, out IParameterList parameters);

    // ----------------------------------------------------

    /// <summary>
    /// Clears this command.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}