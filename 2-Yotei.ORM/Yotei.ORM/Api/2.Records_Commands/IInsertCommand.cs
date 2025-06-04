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
    /// Adds to the HEAD clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. HEAD clauses, if any, appear before any other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand WithHead(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the TAIL clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. TAIL clauses, if any, appear after all other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand WithTail(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to this instance the specifications of the given columns.
    /// <br/>- Standard syntax: 'x => Target = Value'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...) = Value'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand Columns(params Func<dynamic, object>[] specs);
}