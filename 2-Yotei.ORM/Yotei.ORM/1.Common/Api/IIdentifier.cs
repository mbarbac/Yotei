using T = Yotei.ORM.IIdentifierPart;
using K = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
[Cloneable]
public partial interface IIdentifier : IFrozenList<K?, T>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instace, or null if it represents an empty or missed one. If
    /// not null, its parts may be wrapped with the terminator pairs of the associated engine.
    /// </summary>
    string? Value { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance matches the given specifications. Matching is performed by
    /// comparing parts from right to left, where any null or empty part in the specifications
    /// is considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    bool Match(string? specs);

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

    /// <inheritdoc cref="IFrozenList{K, T}.GetRange(int, int)"/>
    new IIdentifier GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Replace(int, T)"/>
    new IIdentifier Replace(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.Add(T)"/>
    new IIdentifier Add(T item);

    /// <inheritdoc cref="IFrozenList{K, T}.AddRange(IEnumerable{T})"/>
    new IIdentifier AddRange(IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.Insert(int, T)"/>
    new IIdentifier Insert(int index, T item);

    /// <inheritdoc cref="IFrozenList{K, T}.InsertRange(int, IEnumerable{T})"/>
    new IIdentifier InsertRange(int index, IEnumerable<T> range);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAt(int)"/>
    new IIdentifier RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveRange(int, int)"/>
    new IIdentifier RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(K)"/>
    new IIdentifier Remove(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(K)"/>
    new IIdentifier RemoveLast(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(K)"/>
    new IIdentifier RemoveAll(K? key);

    /// <inheritdoc cref="IFrozenList{K, T}.Remove(Predicate{T})"/>
    new IIdentifier Remove(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveLast(Predicate{T})"/>
    new IIdentifier RemoveLast(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.RemoveAll(Predicate{T})"/>
    new IIdentifier RemoveAll(Predicate<T> predicate);

    /// <inheritdoc cref="IFrozenList{K, T}.Clear"/>
    new IIdentifier Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the ones obtained
    /// from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance with the added elements obtained from the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Add(string? value);

    /// <summary>
    /// Returns a new instance with the added elements obtained from the given range of values.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value inserted starting
    /// at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values inserted
    /// starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);
}