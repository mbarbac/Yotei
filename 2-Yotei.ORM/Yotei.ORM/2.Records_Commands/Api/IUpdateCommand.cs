namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an UPDATE command.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IUpdateCommand
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
    IUpdateCommand WithHeads<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the TAIL of this instance the contents obtained from parsing the given dynamic
    /// lambda expressions.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand WithTails<T>(params Func<dynamic, T>[] specs);

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
    IUpdateCommand Where<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to this instance the specifications of the affected columns.
    /// <br/>- Standard syntax: 'x => Target = Value'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IUpdateCommand Columns<T>(params Func<dynamic, T>[] specs);

    /// <inheritdoc cref="ICommand.Clear"/>
    new IUpdateCommand Clear();
}