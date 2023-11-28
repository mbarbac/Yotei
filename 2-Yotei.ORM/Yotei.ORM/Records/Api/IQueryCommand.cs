namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented query command.
/// </summary>
public interface IQueryCommand : IEnumerableCommand<IRecord>
{
    /// <summary>
    /// Adds to this command the FROM specifications obtained from parsing the given dynamic
    /// lambda expression, using the following syntaxes:
    /// <br/>- 'x => expr'
    /// <br/>- 'x => x(expr).As(expr)', where the AS part is optional.
    /// <br/>- 'x => x.Source.As(expr)', where the AS part is optional.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand From(Func<dynamic, object> specs);

    /// <summary>
    /// Adds to this command the WHERE specifications obtained from parsing the given dynamic
    /// lambda expression, using the following syntaxes:
    /// <br/>- 'x => expr'
    /// <br/>- 'x => x.And(expr)'
    /// <br/>- 'x => x.Or(expr)'
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Where(Func<dynamic, object> specs);

    /// <summary>
    /// Adds to this command the ORDER BY specifications obtained from parsing the given dynamic
    /// lambda expression, using the following syntaxes:
    /// <br/>- 'x => expr'
    /// <br/>- 'x => expr.Asc()', to specify ascendinf order.
    /// <br/>- 'x => expr.Desc()', to specify descending order.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    IQueryCommand OrderBy(Func<dynamic, object> spec);

    /// <summary>
    /// Adds to this command the SELECT specifications obtained from parsing the given dynamic
    /// lambda expression, using the following syntaxes:
    /// <br/>- 'x => expr'
    /// <br/>- 'x => x(expr).As(expr)', where the AS part is optional.
    /// <br/>- 'x => x.Source.As(expr)', where the AS part is optional.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IQueryCommand Select(Func<dynamic, object>[] specs);

    /// <summary>
    /// Clears this command.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <returns></returns>
    new IQueryCommand Clear();
}