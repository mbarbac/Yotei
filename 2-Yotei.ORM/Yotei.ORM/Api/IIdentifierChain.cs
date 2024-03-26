using T = Yotei.ORM.IIdentifierPart;
using K = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary multi-part database identifier.
/// </summary>
[Cloneable]
public partial interface IIdentifierChain : IIdentifier, IFrozenList<K?, T>
{
    /// <inheritdoc cref="IFrozenList{K, T}.Contains(K)"/>
    new bool Contains(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexesOf(K)"/>
    new int IndexOf(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.LastIndexOf(K)"/>
    new int LastIndexOf(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.IndexesOf(K)"/>
    new List<int> IndexesOf(K? key);

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{K, T}.GetRange(int, int)"/>
    new IIdentifierChain GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Replace(int, T)"/>
    new IIdentifierChain Replace(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.Add(T)"/>
    new IIdentifierChain Add(T item);

    /// <inheritdoc cref="IFrozenList{K, T}.AddRange(IEnumerable{T})"/>
    new IIdentifierChain AddRange(IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.Insert(int, T)"/>
    new IIdentifierChain Insert(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.InsertRange(int, IEnumerable{T})"/>
    new IIdentifierChain InsertRange(int index, IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAt(int)"/>
    new IIdentifierChain RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveRange(int, int)"/>
    new IIdentifierChain RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(K)"/>
    new IIdentifierChain Remove(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new IIdentifierChain RemoveLast(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(K)"/>
    new IIdentifierChain RemoveAll(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(Predicate{T})"/>
    new IIdentifierChain Remove(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new IIdentifierChain RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(Predicate{T})"/>
    new IIdentifierChain RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.Clear"/>
    new IIdentifierChain Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced
    /// by the new ones obtained from the given value. If no changes have been made, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given value have been added
    /// to the collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="value"></param>
    IIdentifierChain Add(string? value);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of values have
    /// been added to the collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierChain AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given value have been inserted
    /// starting at the given index into the collection. If no changes have been made, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierChain Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance where the elements from the given range of values have been inserted
    /// into the collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierChain InsertRange(int index, IEnumerable<string?> range);
}