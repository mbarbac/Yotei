namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented delete command.
/// </summary>
public interface IDeleteCommand : IEnumerableCommand<IRecord>, IExecutableCommand, IPrimarySourced
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
    IDeleteCommand Where(Func<dynamic, object> specs);

    /// <summary>
    /// Determines if this instance is considered a valid one even if no filters are specified.
    /// The default value of this property is false meaning that, if this instance is an empty
    /// one, an exception will be thrown when executed.
    /// </summary>
    bool EmptyAllowed { get; set; }

    /// <summary>
    /// Clears this command.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <returns></returns>
    new IDeleteCommand Clear();
}