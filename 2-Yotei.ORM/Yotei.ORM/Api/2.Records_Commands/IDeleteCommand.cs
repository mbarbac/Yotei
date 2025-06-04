namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a DELETE command.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IDeleteCommand
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
    IDeleteCommand WithHead(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the TAIL clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. TAIL clauses, if any, appear after all other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IDeleteCommand WithTail(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Determines if a 'DELETE ALL' operation is allowed or not.
    /// <br/> This property acts as a safety net throwing an exception if no filters are set
    /// and there is an attempt of executing this command.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IDeleteCommand WithDeleteAll(bool value);

    /// <summary>
    /// The actual value to use for DELETE ALL.
    /// </summary>
    bool DeleteAll { get; }

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
    IDeleteCommand Where(params Func<dynamic, object>[] specs);
}