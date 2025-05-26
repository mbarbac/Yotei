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
    /// Determines if a 'DELETE ALL' operation is allowed or not.
    /// <br/> This property acts as a safety net throwing an exception if no filters are set
    /// and there is an attempt of executing this command.
    /// <br/> The setter with a <c>true</c> value clears all captured filters.
    /// <br/> Similarly, setting any filter sets this property to <c>false</c>.
    /// </summary>
    [With] bool DeleteAll { get; set; }

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
    IDeleteCommand Where(params Func<dynamic, object>[] specs);
}