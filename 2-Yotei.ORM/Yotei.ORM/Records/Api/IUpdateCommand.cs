namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented update command.
/// </summary>
public interface IUpdateCommand : IEnumerableCommand<IRecord>, IExecutableCommand, IPrimarySourced
{
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
    IUpdateCommand Where(Func<dynamic, object> specs);

    /// <summary>
    /// Adds to this command the VALUES specifications obtained from parsing the given dynamic
    /// lamda expression, each using the following syntax:
    /// <br/>- 'x => x.Source = expr'
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand Values(Func<dynamic, object>[] specs);

    /// <summary>
    /// Clears this command.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <returns></returns>
    new IUpdateCommand Clear();
}