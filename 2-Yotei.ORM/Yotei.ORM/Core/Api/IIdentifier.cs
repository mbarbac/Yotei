using THost = Yotei.ORM.IIdentifier;
using TItem = Yotei.ORM.IIdentifierPart;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a database identifier.
/// <br/> Duplicate values are allowed.
/// </summary>
public interface IIdentifier : IEnumerable<TItem>, IEquatable<THost>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instance, or null if it represents an empty or missed one. If
    /// not, the value of each part is wrapped with the engine terminators, if they are used.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Determines if the value of this identifier matches the given one, or not. Matching is
    /// determined by comparing the unwrapped values of the parts in this instance against the
    /// target ones, from right to left. Any null, empty, or missed target part is excluded from
    /// the comparison and considered an implicit match.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Match(THost target);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of parts in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the part at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance has a part with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(string? value);

    /// <summary>
    /// Returns the index of the first part in this instance with the given value, or -1 if any
    /// if found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(string? value);

    /// <summary>
    /// Returns the index of the last part in this instance with the given value, or -1 if any
    /// if found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(string? value);

    /// <summary>
    /// Returns the indexes of the parts in this instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? value);

    /// <summary>
    /// Determines if this instance has a part that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first part in this instance that matches the given predicate,
    /// or -1 if any if found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last part in this instance that matches the given predicate,
    /// or -1 if any if found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the indexes of the parts in this instance that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the parts in this instance.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the parts in this instance.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of parts from the given index. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the part at the given index has been replaced by the new
    /// given one. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    THost Replace(int index, TItem part);

    /// <summary>
    /// Returns a new instance where the part at the given index has been replaced by the ones
    /// obtained from the given dotted value. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Replace(int index, string? dotted);

    /// <summary>
    /// Returns a new instance where the given part has been added. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost Add(TItem part);

    /// <summary>
    /// Returns a new instance where the parts obtained from the given dotted value have been
    /// added. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Add(string? dotted);

    /// <summary>
    /// Returns a new instance where the parts from the given range have been added. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the parts obtained from the given range of dotted values
    /// have been added. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<string?> dottedrange);

    /// <summary>
    /// Returns a new instance where the given part has been inserted at the given index. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    THost Insert(int index, TItem part);

    /// <summary>
    /// Returns a new instance where the parts obtained from the given dotted value have been
    /// inserted starting at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Insert(int index, string? dotted);

    /// <summary>
    /// Returns a new instance where the parts obtained from the given range of dotted values have
    /// been inserted starting at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<string?> dottedrange);

    /// <summary>
    /// Returns a new instance where the part at the given index has been removed. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of parts, from the given index, have been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first part with the given value has been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Remove(string? value);

    /// <summary>
    /// Returns a new instance where the last part with the given value has been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    THost RemoveLast(string? value);

    /// <summary>
    /// Returns a new instance where all the parts with the given value have been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    THost RemoveAll(string? value);

    /// <summary>
    /// Returns a new instance where the first part that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last part that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the parts that match the given predicate have been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the parts have been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}