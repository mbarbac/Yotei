using T = Yotei.ORM.Records.ISchemaEntry;
using K = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Maintains the ordered collection of schema entries that describe the structure and contents 
/// of a record.
/// <br/> Duplicated elements are accepted as far as they are the same one.
/// </summary>
[Cloneable]
public partial interface ISchema : IFrozenList<K, T>, IEquatable<ISchema>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Returns the collection of the indexes of the elements in this instance that match the
    /// given specifications. Comparison is performed by comparing parts from right to left, where
    /// any null or empty specification one is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the collection of the indexes of the elements in this instance that match the
    /// given specifications. Comparison is performed by comparing parts from right to left, where
    /// any null or empty specification one is taken as an implicit match. If there is just one
    /// match, then it is placed in the out argument.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out T? unique);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Gets the index of the first element in this collection with the given identifier, or -1
    /// if it is not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Gets the index of the last element in this collection with the given identifier, or -1
    /// if it is not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Gets the indexes of the elements in this collection with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

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

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new ISchema RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(Predicate{T})"/>
    new ISchema RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.Clear"/>
    new ISchema Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the first element with the given identifier has been removed
    /// from the collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema Remove(string identifier);

    /// <summary>
    /// Returns a new instance where the last element with the given identifier has been removed
    /// from the collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema RemoveLast(string identifier);

    /// <summary>
    /// Returns a new instance where all the elements with the given identifier have been removed
    /// from the collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    ISchema RemoveAll(string identifier);
}