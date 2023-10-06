using IHost = Yotei.ORM.Core.IIdentifierMultiPart;
using IItem = Yotei.ORM.Core.IIdentifierSinglePart;

namespace Yotei.ORM.Core;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier.
/// </summary>
public interface IIdentifierMultiPart : IIdentifier, IEnumerable<IItem>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IItem this[int index] { get; set; }

    /// <summary>
    /// Determines if this instance contains an element with the given single-part value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(string? value);

    /// <summary>
    /// Returns the index of the first element in this instance with the given single-part value,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(string? value);

    /// <summary>
    /// Returns the index of the last element in this instance with the given single-part value,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(string? value);

    /// <summary>
    /// Returns the indexes of the parts in this instance that carries the given single-part
    /// value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? value);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or or -1 if any
    /// is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or or -1 if any
    /// is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the indexes of all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<IItem> ToList();

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    IItem[] ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance that contains the given number of elements from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been replaced
    /// by a new given one with the given single-part value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost ReplaceItem(int index, string? value);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been replaced
    /// by the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost ReplaceItem(int index, IItem item);

    /// <summary>
    /// Returns a copy of this instance where a new element with the given single-part value
    /// has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Add(string? value);

    /// <summary>
    /// Returns a copy of this instance where the given element has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Add(IItem item);

    /// <summary>
    /// Returns a copy of this instance where new elements from the given range of single-part
    /// values have been added to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<IItem> range);

    /// <summary>
    /// Returns a copy of this instance where the a new element with the given single-part value
    /// has been inserted into it, at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Insert(int index, string? value);

    /// <summary>
    /// Returns a copy of this instance where the given element has been inserted into it, at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Insert(int index, IItem item);

    /// <summary>
    /// Returns a copy of this instance where new elements from the given range of values have
    /// been inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been
    /// inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<IItem> range);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IHost RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element with the given
    /// single-part value has been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Remove(string? value);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of an element with the given
    /// single-part value has been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveLast(string? value);

    /// <summary>
    /// Returns a copy of this instance where all the elements with the given single-part value
    /// has been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveAll(string? value);

    /// <summary>
    /// Returns a copy of this instance where the given number of elements, starting at the given
    /// index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost Remove(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveLast(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of elements that match the given
    /// predicate have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveAll(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IHost Clear();
}