namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an INSERT command.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IInsertCommand
    : ICommand, IEnumerableCommand, IExecutableCommand, IPrimarySourced
{
    /// <summary>
    /// Adds to this instance the specifications of the given columns.
    /// <br/>- Standard syntax: 'x => Target = Value'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...) = Value'.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand Columns(params Func<dynamic, object>[] specs);
}