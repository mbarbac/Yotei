using T = Yotei.ORM.IIdentifierSinglePart;
using K = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary multi-part identifier.
/// </summary>
[Cloneable]
public partial interface IIdentifierMultiPart : IIdentifier, IFrozenList<K?, T>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{K, T}.Contains(K)"/>
    new bool Contains(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexOf(K)"/>
    new int IndexOf(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.LastIndexOf(K)"/>
    new int LastIndexOf(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexesOf(K)"/>
    new List<int> IndexesOf(K? key);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the ones obtained
    /// from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierMultiPart Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance with the added elements obtained from the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierMultiPart Add(string? value);

    /// <summary>
    /// Returns a new instance with the added elements obtained from the given range of values.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierMultiPart AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value inserted starting
    /// at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierMultiPart Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values inserted
    /// starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierMultiPart InsertRange(int index, IEnumerable<string?> range);

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{K, T}.GetRange(int, int)"/>
    new IIdentifierMultiPart GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Replace(int, T)"/>
    new IIdentifierMultiPart Replace(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.Add(T)"/>
    new IIdentifierMultiPart Add(T item);

    /// <inheritdoc cref="IFrozenList{K, T}.AddRange(IEnumerable{T})"/>
    new IIdentifierMultiPart AddRange(IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.Insert(int, T)"/>
    new IIdentifierMultiPart Insert(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.InsertRange(int, IEnumerable{T})"/>
    new IIdentifierMultiPart InsertRange(int index, IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAt(int)"/>
    new IIdentifierMultiPart RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveRange(int, int)"/>
    new IIdentifierMultiPart RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(K)"/>
    new IIdentifierMultiPart Remove(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new IIdentifierMultiPart RemoveLast(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(K)"/>
    new IIdentifierMultiPart RemoveAll(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(Predicate{T})"/>
    new IIdentifierMultiPart Remove(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(Predicate{T})"/>
    new IIdentifierMultiPart RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(Predicate{T})"/>
    new IIdentifierMultiPart RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.Clear"/>
    new IIdentifierMultiPart Clear();
}