namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an INSERT command.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IInsertCommand
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
    IInsertCommand WithHeads<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to the TAIL of this instance the contents obtained from parsing the given dynamic
    /// lambda expressions.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand WithTails<T>(params Func<dynamic, T>[] specs);

    /// <summary>
    /// Adds to this instance the specifications of the affected columns.
    /// <br/>- Standard syntax: 'x => Target = Value'.
    /// <br/> Returns a reference to itself to enable a fluent syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specs"></param>
    /// <returns></returns>
    IInsertCommand Columns<T>(params Func<dynamic, T>[] specs);

    /// <inheritdoc cref="ICommand.Clear"/>
    new IInsertCommand Clear();
}