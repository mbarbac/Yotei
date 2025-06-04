namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a QUERY command.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IQueryCommand : ICommand, IEnumerableCommand
{
    /// <summary>
    /// Adds to the HEAD clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. HEAD clauses, if any, appear before any other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand WithHead(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the TAIL clauses of this instance the one obtained from parsing the given dynamic
    /// lambda expression. TAIL clauses, if any, appear after all other contents, separated by
    /// spaces.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand WithTail(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the FROM clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand From(params Func<dynamic, object>[] specs);

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
    IQueryCommand Where(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the SELECT clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
    /// <br/>- Alternate syntax: 'x => x.All()'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Select(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the ORDER BY clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source.Ordering()'.
    /// <br/>- Ordering: ASC, ASCENDING, DESC, DESCENDING.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand OrderBy(params Func<dynamic, object>[] specs);

    // ----------------------------------------------------

    /// <summary>
    /// Sets the value of the DISTINCT clause to the given value.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="value"></param>
    /// <remarks>
    /// The underlying database engine may not support DISTINCT operations.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand Distinct(bool value);

    /// <summary>
    /// The actual value to use with the DISTINCT clause.
    /// </summary>
    bool DistinctValue { get; }

    /// <summary>
    /// Adds to the contents of the JOIN clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source.As(...).On(...)'.
    /// <br/>- Alternate syntax: 'x => x.Join(join-type).Source.As(...).On(...)'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// The underlying database engine may not support JOIN operations.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand Join(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the GROUP BY clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// The underlying database engine may not support GROUP BY operations.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand GroupBy(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the HAVING clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// The underlying database engine may not support GROUP BY operations.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand Having(params Func<dynamic, object>[] specs);
}