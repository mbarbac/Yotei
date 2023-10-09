using IHost = Yotei.ORM.Core.IIdentifierMultiPart;
using IItem = Yotei.ORM.Core.IIdentifierSinglePart;
namespace Yotei.ORM.Core;

// ========================================================
/// <summary>
/// The immutable object that represents a multi-part database identifier.
/// </summary>
public interface IIdentifierMultiPart : IIdentifier, IEnumerable<IItem>
{
    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory footprint of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(IItem item);

    /// <summary>
    /// Determines if this instance contains a part with the given value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(string? value);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or
    /// -1 if no equivalent can be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(IItem item);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection with the given
    /// value, or -1 if any can be found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(string? value);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or
    /// -1 if no equivalent can be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(IItem item);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection with the given
    /// value, or -1 if any can be found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(string? value);

    /// <summary>
    /// Returns a list containing the indexes of all the ocurrences of the given element, or
    /// equivalent ones, in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(IItem item);

    /// <summary>
    /// /// Returns a list containing the indexes of all the the elements in this collection
    /// that carries the given value, or an equivalent one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? value);

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a list containing the indexes of all the elements in this collection that match
    /// the given predicate.
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
    /// Returns a new instance that contains the given number of elements from the original
    /// collection, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost ReplaceItem(int index, IItem item);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// a new element with the given value, or a chain if it was a multipart value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost ReplaceValue(int index, string? value);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Add(IItem item);

    /// <summary>
    /// Returns a new instance where a new element with the given value, or a chain of elements
    /// it it was a multipart value, have been added to the original collection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost AddValue(string? value);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<IItem> range);

    /// <summary>
    /// Returns a new instance where new elements with the values from the given range, or a chain
    /// of elements per each if any of them was a multipart value, have been added to the original
    /// collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddValueRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the original
    /// collection at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Insert(int index, IItem item);

    /// <summary>
    /// Returns a new instance where a new element with the given value, or a chain of elements
    /// it it was a multipart value, have been inserted into the original collection at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost InsertValue(int index, string? value);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the original collection, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<IItem> range);

    /// <summary>
    /// Returns a new instance where new elements with the values from the given range, or a chain
    /// of elements per each if any of them was a multipart value, have been inserted into the
    /// original collection, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertValueRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IHost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given element, or an equivalent
    /// one, has been removed from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Remove(IItem item);

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given value, or an equivalent
    /// one, has been removed from the original collection. If the value was a multipart one,
    /// then all its parts are removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveValue(string? value);

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given element, or an equivalent
    /// one, has been removed from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost RemoveLast(IItem item);

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given value, or an equivalent
    /// one, has been removed from the original collection. If the value was a multipart one,
    /// then all its parts are removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveLastValue(string? value);

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given element, or equivalent ones,
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost RemoveAll(IItem item);

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given value, or equivalent ones,
    /// have been removed from the original collection. If the value was a multipart one, then
    /// all its parts are removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveAllValues(string? value);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost Remove(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveLast(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where all the ocurrences of elements matching the given predicate
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveAll(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IHost Clear();
}