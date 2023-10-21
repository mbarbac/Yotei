using IHost = Yotei.ORM.IIdentifierMultiPart;
using IItem = Yotei.ORM.IIdentifierSinglePart;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier. Two elements are considered the same if their
/// values are both null, or if they match according to the case sensitivity settings of the
/// underlying engine. Duplicated elements are allowed.
/// </summary>
[Cloneable]
public partial interface IIdentifierMultiPart : IIdentifier, IEnumerable<IItem>
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
    /// <param name="strict"></param>
    /// <returns></returns>
    bool Contains(IItem item, bool strict = false);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this instance, or -1
    /// if it cannot be found. If strict mode is requested, comparison is made by value or
    /// reference, instead of using the comparison criteria.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int IndexOf(IItem item, bool strict = false);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this instance, or -1
    /// if it cannot be found. If strict mode is requested, comparison is made by value or
    /// reference, instead of using the comparison criteria.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int LastIndexOf(IItem item, bool strict = false);

    /// <summary>
    /// Returns a list with the indexes of the ocurrences of the given element in this instance.
    /// If strict mode is requested, comparison is made by value or reference, instead of using
    /// the comparison criteria.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    List<int> IndexesOf(IItem item, bool strict = false);

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate, or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this instance that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this instance that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a list containing the indexes of all the elements in this instance that match
    /// the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    IItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<IItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains an element that matches the given criteria.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(string? value);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this instance that matches
    /// the given criteria, or -1 if any can be found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(string? value);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this instance that matches
    /// the given criteria, or -1 if any can be found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(string? value);

    /// <summary>
    /// Returns a list containing the indexes of all the elements in this instance that match
    /// the given criteria.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the given number of elements, starting from the
    /// given index, from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the item at the given index has been replaced by the new
    /// given one. If strict mode is requested, comparison is made by value or reference, instead
    /// of using the comparison criteria.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    IHost Replace(int index, IItem item, bool strict = false);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Add(IItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<IItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted, at the given index,
    /// into the original one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Insert(int index, IItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted, at
    /// the given index, into the original one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<IItem> range);

    /// <summary>
    /// Returns a new instance where element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IHost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the given element has been removed. If strict mode is
    /// requested, comparison is made by value or reference, instead of using the comparison
    /// criteria.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    IHost Remove(IItem item, bool strict = false);

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given element has been removed.
    /// If strict mode is requested, comparison is made by value or reference, instead of using
    /// the comparison criteria.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    IHost RemoveLast(IItem item, bool strict = false);

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given element have been removed.
    /// If strict mode is requested, comparison is made by value or reference, instead of using
    /// the comparison criteria.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    IHost RemoveAll(IItem item, bool strict = false);

    /// <summary>
    /// Returns a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost Remove(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveLast(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveAll(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed.
    /// </summary>
    /// <returns></returns>
    IHost Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the item at the given index has been replaced by the new
    /// one(s) obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where the element(s) obtained from the given value has(ve) been
    /// added to the original one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Add(string? value);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of values have
    /// been added to the original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the element(s) obtained from the given value has(ve) been
    /// inserted into the original one, starting at the given index.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of values have
    /// been inserted into the original one, starting at the given index.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the first ocurrence(s) of the element(s) obtained from the
    /// given value has(ve) been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Remove(string? value);

    /// <summary>
    /// Returns a new instance where the last ocurrence(s) of the element(s) obtained from the
    /// given value has(ve) been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveLast(string? value);

    /// <summary>
    /// Returns a new instance where all the ocurrences of the element(s) obtained from the
    /// given value have been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost RemoveAll(string? value);
}