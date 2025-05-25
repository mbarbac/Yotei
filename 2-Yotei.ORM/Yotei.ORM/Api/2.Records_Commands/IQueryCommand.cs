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
    /// <returns></returns>
    IQueryCommand Select(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the ORDER BY clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source.Ordering()'.
    /// <br/>- Ordering: ASC, ASCENDING, DESC, DESCENDING.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand OrderBy(params Func<dynamic, object>[] specs);

    // ----------------------------------------------------

    /// <summary>
    /// Gets or sets the value of the DISTINCT clause.
    /// </summary>
    /// <remarks>
    /// The underlying database engine may not support DISTINCT operations.
    /// </remarks>
    [With] bool Distinct { get; set; }

    /// <summary>
    /// Adds to the contents of the JOIN clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source.As(...).On(...)'.
    /// <br/>- Alternate syntax: 'x => x.Join(join-type).Source.As(...).On(...)'.
    /// </summary>
    /// <remarks>
    /// The underlying database engine may not support JOIN operations.
    /// </remarks>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Join(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the GROUP BY clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// </summary>
    /// <remarks>
    /// The underlying database engine may not support GROUP BY operations.
    /// </remarks>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand GroupBy(params Func<dynamic, object>[] specs);

    /// <summary>
    /// Adds to the contents of the HAVING clause the ones obtained from parsing the given
    /// dynamic lambda expression.
    /// <br/>- Standard syntax: 'x => Condition' (and AND connector is used by default).
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// </summary>
    /// <remarks>
    /// The underlying database engine may not support GROUP BY operations.
    /// </remarks>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Having(params Func<dynamic, object>[] specs);
}