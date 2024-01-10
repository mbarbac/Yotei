namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary command that can be executed against an underlying database.
/// </summary>
[Cloneable]
public partial interface ICommand
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Obtains the text and parameters that can executed against the underlying database, using
    /// the default iterable mode of this command.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(out IParameterList parameters);

    /// <summary>
    /// Obtains the text and parameters that can executed against the underlying database, using
    /// the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(bool iterable, out IParameterList parameters);
}