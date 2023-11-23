using THost = Yotei.ORM.IIdentifier;
using TItem = Yotei.ORM.IIdentifierPart;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a part in a database identifier.
/// <br/> Duplicate values are allowed.
/// </summary>
[Cloneable]
public partial interface IIdentifier : IEnumerable<TItem>, IEquatable<THost>
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
    /// The unwrapped values carried by this instance, or an empty array if it represents an
    /// empty or missed one.
    /// </summary>
    string?[] UnwrappedValues { get; }

    /// <summary>
    /// Determines if the value of this identifier matches the given one, or not. Matching is
    /// determined by comparing the unwrapped parts of this instance against the target ones,
    /// from right to left, where any null, empty, or missed target part is excluded from the
    /// comparison and considered an implicit match.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Match(THost target);

    // ----------------------------------------------------

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance has at least one element whose value matches the given one.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    bool Contains(string? part);

    /// <summary>
    /// Returns the index of the first element in this instance whose value matches the given
    /// one, or -1 if any is found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int IndexOf(string? part);

    /// <summary>
    /// Returns the index of the last element in this instance whose value matches the given one,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int LastIndexOf(string? part);

    /// <summary>
    /// Returns the indexes of the elements in this instance whose values match the given one.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? part);

    /// <summary>
    /// Determines if this instance has at least one element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of all the elements in this instance that match the given predicate..
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index.
    /// If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// ones obtained from the given dotted value. If no changes are detected, returns the original
    /// instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Replace(int index, string? dotted);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given dotted value have been
    /// added to the original collection. If no changes are detected, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Add(string? dotted);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of dotted values
    /// have been added to the original collection. If no changes are detected, returns the original
    /// instance instead.
    /// </summary>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<string?> dottedrange);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given dotted value have been
    /// inserted starting at the given index into the original collection. If no changes are
    /// detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Insert(int index, string? dotted);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of dotted values
    /// have been inserted into the original collection, starting at the given index. If no changes
    /// are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<string?> dottedrange);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed. If no
    /// changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element whose value matches the given one has been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost Remove(string? part);

    /// <summary>
    /// Returns a new instance where the last element whose value matches the given one has been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost RemoveLast(string? part);

    /// <summary>
    /// Returns a new instance where all the elements whose values match the given one have been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost RemoveAll(string? part);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate. If no
    /// changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original element have been removed. If no changes
    /// are detected, returns the original instance instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}