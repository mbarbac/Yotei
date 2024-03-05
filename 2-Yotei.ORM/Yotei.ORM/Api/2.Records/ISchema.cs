using T = Yotei.ORM.ISchemaEntry;
using K = Yotei.ORM.IIdentifier;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// The ordered collection of schema entries that describe the structure and contents of an
/// associated record.
/// </summary>
[Cloneable]
public partial interface ISchema : IFrozenList<K, T>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Gets the index of the first element in this collection with the given key, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Gets the index of the last element in this collection with the given key, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Gets the indexes of the elements in this collection with the given key.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

    /// <summary>
    /// Returns the collection of indexes of the elements in this instances whose identifiers
    /// match the given specifications. Matching is performed by comparing parts from right to
    /// left, where any null or empty part in the specifications is considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the collection of indexes of the elements in this instances whose identifiers
    /// match the given specifications. Matching is performed by comparing parts from right to
    /// left, where any null or empty part in the specifications is considered an implicit match.
    /// <br/> If the returned collection contains just one element, then it is placed in the out
    /// argument. Otherwise, it is set to null.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out T? unique);

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{K, T}.GetRange(int, int)"/>
    new ISchema GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Replace(int, T)"/>
    new ISchema Replace(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.Add(T)"/>
    new ISchema Add(T item);

    /// <inheritdoc cref="IFrozenList{K, T}.AddRange(IEnumerable{T})"/>
    new ISchema AddRange(IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.Insert(int, T)"/>
    new ISchema Insert(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.InsertRange(int, IEnumerable{T})"/>
    new ISchema InsertRange(int index, IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAt(int)"/>
    new ISchema RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveRange(int, int)"/>
    new ISchema RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(K)"/>
    new ISchema Remove(K key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new ISchema RemoveLast(K key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(K)"/>
    new ISchema RemoveAll(K key);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(Predicate{T})"/>
    new ISchema Remove(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(Predicate{T})"/>
    new ISchema RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(Predicate{T})"/>
    new ISchema RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.Clear"/>
    new ISchema Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with first element with the given identifier removed. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ISchema Remove(string identifier);

    /// <summary>
    /// Returns a new instance with last element with the given identifier removed. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ISchema RemoveLast(string identifier);

    /// <summary>
    /// Returns a new instance with all elements with the given identifier removed. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ISchema RemoveAll(string identifier);
}