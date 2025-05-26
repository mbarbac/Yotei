namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an UPDATE command.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IUpdateCommand
    : ICommand, IEnumerableCommand, IExecutableCommand, IPrimarySourced
{
    /// <summary>
    /// Adds to the contents of the WHERE clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IUpdateCommand Where(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to this instance the specifications of the given columns.
    /// <br/>- Standard syntax: 'x => Target = Value'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...) = Value'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IUpdateCommand Columns(params Func<dynamic, object>[] specs);
}