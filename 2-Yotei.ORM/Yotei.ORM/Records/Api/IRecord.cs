using THost = Yotei.ORM.Records.IRecord;
using TItem = Yotei.ORM.Records.IRecordEntry;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the contents retrieved from, or to be persisted into,
/// an underlying database.
/// <br/> Duplicated elements are allowed as far as the are the same instance.
/// </summary>
public interface IRecord : IEnumerable<TItem>
{
    /// <summary>
    /// The object that describes the structure and contents of this instance.
    /// </summary>
    ISchema Schema { get; }

    // ----------------------------------------------------

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
    /// Determines if this collection contains an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Returns the index of the first element in this collection with the given identifier,
    /// or -1 it cannot be found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Returns the index of the last element in this collection with the given identifier,
    /// or -1 it cannot be found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Returns the indexes of the elements in this collection with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or -1 if no one
    /// can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or -1 if no one
    /// can be found.
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
    /// An array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// A list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    /// <summary>
    /// Returns the indexes of the elements in this instance whose identifiers match the given
    /// specifications. Matching is determined by comparing the  unwrapped values of the parts
    /// of the identifiers in this instance with the target ones, from right to left, where any
    /// missed or empty target part is excluded from the comparison and considered an implicit
    /// match.
    /// <br/> Matching is not conmutative, 'a' matching 'b' does not mean that 'b' matches 'a'.
    /// </summary>
    /// <param name="specifications"></param>
    /// <param name="unique"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    List<int> Match(string? specifications, out TItem? unique, out int index);

    /// <summary>
    /// Compares the elements of this instance against the given target ones, and returns a new
    /// record with the changes detected in the values, along with the metadata that describes
    /// them. If no changes were detected, returns null. Orphan source and target entries are
    /// returned only if such is requested.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    TItem? CompareTo(TItem target, bool orphanSources = false, bool orphanTargets = false);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements from the original one, starting
    /// at the given index. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by 
    /// the new given one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the value of the original element at the given index has
    /// been replaced by  the new given one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost ReplaceValue(int index, object? value);

    /// <summary>
    /// Returns a new instance where the metadata of the original element at the given index has
    /// been replaced by  the new given one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    THost ReplaceMetadata(int index, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If no
    /// changes were detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Add(TItem item);

    /// <summary>
    /// Returns a new instance where the given range of elements have been added to the original
    /// one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted, at the given index, into
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given range of elements have been inserted, starting at
    /// the given index, into the original one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed from
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed from the original one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given identifier has been removed
    /// from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    THost Remove(string identifier);

    /// <summary>
    /// Returns a new instance where the last element with the given identifier has been removed
    /// from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    THost RemoveLast(string identifier);

    /// <summary>
    /// Returns a new instance where all the elements with the given identifier have been removed
    /// from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    THost RemoveAll(string identifier);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed. If no changes
    /// were detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}