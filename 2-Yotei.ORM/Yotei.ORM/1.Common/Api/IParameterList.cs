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

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.GetRange(int, int)"/>
    new IParameterList GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Replace(int, TItem)"/>
    new IParameterList Replace(int index, TItem item);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Add(TItem)"/>
    new IParameterList Add(TItem item);

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, added to it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out TItem? item);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.AddRange(IEnumerable{TItem})"/>
    new IParameterList AddRange(IEnumerable<TItem> range);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Insert(int, TItem)"/>
    new IParameterList Insert(int index, TItem item);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.InsertRange(int, IEnumerable{TItem})"/>
    new IParameterList InsertRange(int index, IEnumerable<TItem> range);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAt(int)"/>
    new IParameterList RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveRange(int, int)"/>
    new IParameterList RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Remove(TKey)"/>
    new IParameterList Remove(TKey key);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveLast(TKey)"/>
    new IParameterList RemoveLast(TKey key);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAll(TKey)"/>
    new IParameterList RemoveAll(TKey key);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Remove(Predicate{TItem})"/>
    new IParameterList Remove(Predicate<TItem> predicate);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveLast(Predicate{TItem})"/>
    new IParameterList RemoveLast(Predicate<TItem> predicate);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAll(Predicate{TItem})"/>
    new IParameterList RemoveAll(Predicate<TItem> predicate);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Clear"/>
    new IParameterList Clear();
}