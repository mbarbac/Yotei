using TItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// </summary>
public interface IParameterList : IFrozenList<TKey, TItem>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Returns the next available parameter name.
    /// </summary>
    /// <returns></returns>
    string NextName();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.GetRange(int, int)"/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    new IParameterList GetRange(int index, int count);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Replace(int, TItem)"/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new IParameterList Replace(int index, TItem item);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Add(TItem)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new IParameterList Add(TItem item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and the next
    /// available name in the collection, has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out TItem? item);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.AddRange(IEnumerable{TItem})"/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    new IParameterList AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Insert(int, TItem)"/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new IParameterList Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and the next
    /// available name in the collection, has been added to it.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList InsertNew(int index, object? value, out TItem? item);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.InsertRange(int, IEnumerable{TItem})"/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    new IParameterList InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAt(int)"/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new IParameterList RemoveAt(int index);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveRange(int, int)"/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    new IParameterList RemoveRange(int index, int count);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Remove(TKey)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    new IParameterList Remove(TKey key);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveLast(TKey)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    new IParameterList RemoveLast(TKey key);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAll(TKey)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    new IParameterList RemoveAll(TKey key);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Remove(Predicate{TItem})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    new IParameterList Remove(Predicate<TItem> predicate);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveLast(Predicate{TItem})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    new IParameterList RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAll(Predicate{TItem})"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    new IParameterList RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Clear"/>
    /// </summary>
    /// <returns></returns>
    new IParameterList Clear();
}