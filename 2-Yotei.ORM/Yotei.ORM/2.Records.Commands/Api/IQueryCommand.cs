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
    /// Adds to the HEAD of this instance the contents obtained from parsing the given dynamic
    /// lambda expressions.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand WithHeads<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the TAIL of this instance the contents obtained from parsing the given dynamic
    /// lambda expressions.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand WithTails<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the FROM clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => x.Source'.
    /// <br/>- Alternate syntax: 'x => x.Source.As(...)'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand From<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the WHERE clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => Condition'.
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Where<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the ORDER BY clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => Element'.
    /// <br/>- Alternate syntax: 'x => Element.Ordering()', where 'Ordering' can be: ASCENDING,
    /// ASC, DESCENDING, DESC.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand OrderBy<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the SELECT clause the contents obtained from parsing the given dynamic lambda
    /// expressions. If no SELECT clauses are used, then this instance is equivalent to a
    /// 'SELECT *' one.
    /// <br/>- Standard syntax: 'x => x.Element'.
    /// <br/>- Alternate syntax: 'x => x.Element.As(...)'.
    /// <br/>- Alternate syntax: 'x => x.Source.All()'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Select<T>(params Func<dynamic, T>[] specs);

    /// <inheritdoc cref="ICommand.Clear"/>
    new IQueryCommand Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Gets or sets the value of the DISTINCT clause.
    /// <br/> Note that the underlying engine MAY NOT support this capability.
    /// </summary>
    [With] bool Distinct {get; set; }

    /// <summary>
    /// Adds to the JOIN clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => x.Source.As(...).On(...)'.
    /// <br/>- Alternate syntax: 'x => x.JoinType(join-type).Source.As(...).On(...)'.
    /// <br/> Note that the underlying engine MAY NOT support this capability.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Join<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the GROUP BY clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => x.Element'.
    /// <br/> Note that the underlying engine MAY NOT support this capability.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand GroupBy<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the HAVING clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => Condition'.
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// <br/> Note that the underlying engine MAY NOT support this capability.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Having<T>(params Func<dynamic, T>[] specs);
}