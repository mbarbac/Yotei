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
    /// Gets the command text and parameters using its default iterable mode.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(out IParameterList parameters);

    /// <summary>
    /// Gets the command text and parameters trying to use the given iterable mode.
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    string GetText(bool iterable, out IParameterList parameters);
}