using T = Yotei.ORM.IMetadataTag;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of tags that describes the maximal structure of identifiers
/// in an underlying database.
/// </summary>
[Cloneable]
public partial interface IIdentifierTags : IFrozenList<T>
{
    /// <summary>
    /// Determines if the names in this instance are considered case sensitive or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets the collection of all the names carried by this instance, despite the concrete entry
    /// in this collection that holds them.
    /// </summary>
    IEnumerable<string> Names { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains any name from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Gets the index of the element in this collection that carries the given name, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOf(string name);

    /// <summary>
    /// Gets the index of the first element in this instance that carries any of the given names
    /// or -1 if such element is not found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int IndexOfAny(IEnumerable<string> range);

    /// <summary>
    /// Gets the index of the last element in this instance that carries any of the given names
    /// or -1 if such element is not found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int LastIndexOfAny(IEnumerable<string> range);

    /// <summary>
    /// Gets the indexes of the elements in this instance that carries any of the given names.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    List<int> IndexesOfAny(IEnumerable<string> range);

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{T}.GetRange(int, int)"/>
    new IIdentifierTags GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{T}.Replace(int, T)"/>
    new IIdentifierTags Replace(int index, T item);

    /// <inheritdoc cref="IFrozenList{T}.Add(T)"/>
    new IIdentifierTags Add(T item);

    /// <inheritdoc cref="IFrozenList{T}.AddRange(IEnumerable{T})"/>
    new IIdentifierTags AddRange(IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{T}.Insert(int, T)"/>
    new IIdentifierTags Insert(int index, T item);

    /// <inheritdoc cref="IFrozenList{T}.InsertRange(int, IEnumerable{T})"/>
    new IIdentifierTags InsertRange(int index, IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{T}.RemoveAt(int)"/>
    new IIdentifierTags RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{T}.RemoveRange(int, int)"/>
    new IIdentifierTags RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{T}.Remove(T)"/>
    new IIdentifierTags Remove(T item);

    /// <inheritdoc cref="IFrozenList{T}.RemoveLast(T)"/>
    new IIdentifierTags RemoveLast(T item);

    /// <inheritdoc cref="IFrozenList{T}.RemoveAll(T)"/>
    new IIdentifierTags RemoveAll(T item);

    /// <inheritdoc cref="IFrozenList{T}.Remove(Predicate{T})"/>
    new IIdentifierTags Remove(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{T}.RemoveLast(Predicate{T})"/>
    new IIdentifierTags RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{T}.RemoveAll(Predicate{T})"/>
    new IIdentifierTags RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{T}.Clear"/>
    new IIdentifierTags Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element that carries the given name has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IIdentifierTags Remove(string name);

    /// <summary>
    /// Returns a new instance where the first element that carries any name from the given range
    /// has been removed from the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierTags RemoveAny(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the last element that carries any name from the given range
    /// has been removed from the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierTags RemoveLastAny(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where all the elements that carry any name from the given range
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifierTags RemoveAllAny(IEnumerable<string> range);
}