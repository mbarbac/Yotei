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
    /// Adds to the HEAD clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. HEAD clauses, if any, appear before any other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand WithHead(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the TAIL clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. TAIL clauses, if any, appear after all other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand WithTail(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the WHERE clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand Where(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to this instance the specifications of the given columns.
    /// <br/>- Standard syntax: 'x => Target = Value'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand Columns(params Func<dynamic, object>[] specs);
}