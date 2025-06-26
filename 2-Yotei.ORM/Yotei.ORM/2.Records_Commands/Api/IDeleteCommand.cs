namespace Yotei.ORM.Records;

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
    /// Adds to the HEAD of this instance the contents obtained from parsing the given dynamic
    /// lambda expressions.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IDeleteCommand WithHead<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the TAIL of this instance the contents obtained from parsing the given dynamic
    /// lambda expressions.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IDeleteCommand WithTail<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the WHERE clause the contents obtained from parsing the given dynamic lambda
    /// expressions.
    /// <br/>- Standard syntax: 'x => Condition'.
    /// <br/>- Alternate syntax: 'x => x.And(...)'.
    /// <br/>- Alternate syntax: 'x => x.Or(...)'.
    /// <br/> Note that using an entry without an appropriate AND or OR connector may be invalid SQL
    /// syntax.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IDeleteCommand Where<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Determines if an empty instance with no WHERE clauses, equivalent to a 'DELETE ALL' one,
    /// is valid or not.
    /// <br/> This property acts as a safety belt to prevent the execution of empty instances.
    /// <br/> The value of this property is not modified by <see cref="Clear"/> operations.
    /// </summary>
    [With] bool IsEmptyValid { get; set; }

    /// <inheritdoc cref="ICommand.Clear"/>
    new IDeleteCommand Clear();
}