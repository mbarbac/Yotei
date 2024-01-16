using TItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an arbitrary database identifier.
/// </summary>
public interface IIdentifier : IEnumerable<TItem>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier part, or null if it represents an empty or missed
    /// one. If not, the values of its parts are wrapped with the appropriate engine terminators.
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

    /// <summary>
    /// Returns a builder of the appropriate type, with the elements of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<TKey?, TItem> ToBuilder();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(TKey? key);

    /// <summary>
    /// Returns the index of the first ocurrence of an element with the given key, or -1 if not
    /// found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey? key);

    /// <summary>
    /// Returns the index of the last ocurrence of element with the given key, or -1 if not
    /// found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(TKey? key);

    /// <summary>
    /// Returns the indexes of the ocurrences of elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(TKey? key);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the given number of elements starting from the given
    /// index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the new given one.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the ones obtained
    /// from the given value. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance with the given element added to it. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Add(TItem item);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value added to it. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Add(string? value);

    /// <summary>
    /// Returns a new instance with the elements from the given range add to it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values add to
    /// it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance with the given element inserted into it at the given index. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value inserted into it
    /// at the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance with the elements from the given range inserted into it, starting
    /// at the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the elements obtained from the the given range of value
    /// inserted into it, starting at the given index. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance with the element at the given index removed from it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifier RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting from the given index,
    /// removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element with the given key removed
    /// from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IIdentifier Remove(TKey? key);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element with the given key removed
    /// from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(TKey? key);

    /// <summary>
    /// Returns a new instance with all ocurrences of elements with the givne key removed from
    /// it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(TKey? key);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element that matches the given
    /// predicate removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element that matches the given
    /// predicate removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with all the ocurrences of elements that match the given predicate
    /// removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with all the original elements removed. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <returns></returns>
    IIdentifier Clear();
}