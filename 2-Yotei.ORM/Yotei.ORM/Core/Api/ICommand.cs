namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a command that can be executed against an underlying database.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The locale used by this command with the culture-sensitive elements in the database. Its
    /// default value is obtained from the thread from which this instance was created.
    /// </summary>
    Locale Locale { get; set; }

    /// <summary>
    /// Returns the command text that can be executed against the underlying database, using the
    /// default iterable mode of this command.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(out IParameterList parameters);

    /// <summary>
    /// Returns the command text that can be executed against the underlying database, using the
    /// given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(bool iterable, out IParameterList parameters);

    /// <summary>
    /// Clears this command.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}