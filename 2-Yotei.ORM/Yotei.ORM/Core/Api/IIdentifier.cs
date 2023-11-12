using THost = Yotei.ORM.IIdentifier;
using TItem = Yotei.ORM.IIdentifierPart;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a database identifier.
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
    /// not, each part is wrapped with the engine terminators, if they are used.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Returns an array with the unwrapped values carried by this instance separated by dots,
    /// or an empty one if this instance represents an empty or missed identifier.
    /// </summary>
    /// <returns></returns>
    string?[] ToUnwrappedValues();

    /// <summary>
    /// Determines if the value of this identifier matches the given one, or not. Matching is
    /// determined by comparing the unwrapped parts of this instance against the target ones,
    /// from right to left. Any null, empty, or missed target part is excluded from the
    /// comparison and considered an implicit match.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Match(THost target);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains any elements with the given single part value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    bool Contains(string? part);

    /// <summary>
    /// Returns the index of the first element in this collection with the given single part
    /// value, or -1 if any can be found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int IndexOf(string? part);

    /// <summary>
    /// Returns the index of the last element in this collection with the given single part
    /// value, or -1 if any can be found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int LastIndexOf(string? part);

    /// <summary>
    /// Returns the indexes of all the elements in this collection with the given single part
    /// value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? part);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first element in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that match the given predicate,
    /// or -1 if any can be found.
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
    /// Obtains a new instance that contains the given number of elements starting from the given
    /// index.
    /// <br/> If the index is zero and the requested number of elements is the same as the size
    /// of the original collection, then it is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Obtains a new instance where the element at the given index has been replaced with the
    /// ones obtained from the given value.
    /// <br/> If the given element can be considered as equal to the existing one at that index,
    /// then the original collection is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Replace(int index, string? value);

    /// <summary>
    /// Obtains a new instance where the elements obtained from the given value have been added
    /// to the original one.
    /// <br/> If the range of values was an empty one, then the original collection is returned
    /// instead.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Add(string? value);

    /// <summary>
    /// Obtains a new instance where the elements obtained from the given range of values have
    /// been added to the original one.
    /// <br/> If the range was an empty one, then the original collection is returned instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Obtains a new instance where the elements obtained from the given value have been
    /// inserted into the original one, starting at the given index.
    /// <br/> If the range of values was an empty one, then the original collection is returned
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Insert(int index, string? value);

    /// <summary>
    /// Obtains a new instance where the elements obtained from the given range of values have
    /// been inserted into the original one, starting at the given index.
    /// <br/> If the range was an empty one, then the original collection is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Obtains a new instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Obtains a new instance where the given number of elements have been removed from the
    /// original one, starting from the given index.
    /// <br/> If the number of elements to remove is zero, then the original collection is
    /// returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Obtains a new instance where the first element with the given single part value has been
    /// removed.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost Remove(string? part);

    /// <summary>
    /// Obtains a new instance where the last element with the given single part value has been
    /// removed.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost RemoveLast(string? part);

    /// <summary>
    /// Obtains a new instance where all the elements with the given single part value have been
    /// removed.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    THost RemoveAll(string? part);

    /// <summary>
    /// Obtains a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Obtains a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Obtains a new instance where all the ocurrences of elements that match the given
    /// predicate have been removed.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Obtains a new instance where all the original elements have been removed.
    /// <br/> If the original collection was an empty one, it is returned instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}