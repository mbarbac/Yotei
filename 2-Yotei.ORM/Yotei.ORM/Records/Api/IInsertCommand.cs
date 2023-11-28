namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented insert command.
/// </summary>
public interface IInsertCommand : IEnumerableCommand<IRecord>, IExecutableCommand, IPrimarySourced
{
    /// <summary>
    /// Adds to this command the VALUES specifications obtained from parsing the given dynamic
    /// lamda expression, each using the following syntax:
    /// <br/>- 'x => x.Source = expr'
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand Values(Func<dynamic, object>[] specs);

    /// <summary>
    /// Clears this command.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <returns></returns>
    new IInsertCommand Clear();
}