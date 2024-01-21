using TItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database identifier.
/// </summary>
public interface IIdentifier : IFrozenList<TKey?, TItem>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// If not, the values of its parts are wrapped with the appropriate engine terminators.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Determines if this instance matches the given specifications. Matching is performed by
    /// comparing parts from right to left, where any null or empty specification is excluded
    /// from the comparison and considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    bool Match(string? specs);

    // ----------------------------------------------------

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.GetRange(int, int)"/>
    new IIdentifier GetRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Replace(int, TItem)"/>
    new IIdentifier Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the ones obtained
    /// from the given value. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, string? value);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Add(TItem)"/>
    new IIdentifier Add(TItem item);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value added to it. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Add(string? value);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.AddRange(IEnumerable{TItem})"/>
    new IIdentifier AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values add to
    /// it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Insert(int, TItem)"/>
    new IIdentifier Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value inserted into it
    /// at the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? value);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.InsertRange(int, IEnumerable{TItem})"/>
    new IIdentifier InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the the given range of value
    /// inserted into it, starting at the given index. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAt(int)"/>
    new IIdentifier RemoveAt(int index);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveRange(int, int)"/>
    new IIdentifier RemoveRange(int index, int count);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Remove(TKey)"/>
    new IIdentifier Remove(TKey? key);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveLast(TKey)"/>
    new IIdentifier RemoveLast(TKey? key);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAll(TKey)"/>
    new IIdentifier RemoveAll(TKey? key);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Remove(Predicate{TItem})"/>
    new IIdentifier Remove(Predicate<TItem> predicate);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveLast(Predicate{TItem})"/>
    new IIdentifier RemoveLast(Predicate<TItem> predicate);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.RemoveAll(Predicate{TItem})"/>
    new IIdentifier RemoveAll(Predicate<TItem> predicate);

    /// <inheritdoc cref="IFrozenList{TKey, TItem}.Clear"/>
    new IIdentifier Clear();
}