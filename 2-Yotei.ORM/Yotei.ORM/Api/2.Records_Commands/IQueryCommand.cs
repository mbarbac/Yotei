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
    /// Adds to the contents of the FROM clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand From(params Func<dynamic, object>[] specs);

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
    IQueryCommand Where(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the SELECT clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
    /// <br/>- Alternate syntax: 'x => x.All()'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand Select(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the ORDER BY clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source.Ordering()'.
    /// <br/>- Ordering: ASC, ASCENDING, DESC, DESCENDING.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand OrderBy(params Func<dynamic, object>[] specs);

    // ----------------------------------------------------

    /// <summary>
    /// Sets the value of the DISTINCT clause to the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IQueryCommand Distinct(bool value);

    /// <summary>
    /// Adds to the contents of the JOIN clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source.As(...).On(...)'.
    /// <br/>- Alternate syntax: 'x => x.Join(join-type).Source.As(...).On(...)'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// The underlying database engine may not support JOIN operations.
    /// <br/>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand Join(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the GROUP BY clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// The underlying database engine may not support GROUP BY operations.
    /// <br/>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand GroupBy(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the HAVING clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// </summary>
    /// <param name="specs"></param>
    /// <remarks>
    /// The underlying database engine may not support GROUP BY operations.
    /// <br/>
    /// All syntaxes can be preceeded or followed by invoke tokens to represent head and tail
    /// elements before and after the given spec, as in: 'x => x(...).yyy.x(...)'.
    /// </remarks>
    /// <returns></returns>
    IQueryCommand Having(params Func<dynamic, object>[] specs);
}