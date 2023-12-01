using THost = Yotei.ORM.Records.ISchema;
using TItem = Yotei.ORM.Records.ISchemaEntry;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that describes the structure and contents of the associated records.
/// <br/> Duplicated elements are allowed as far as the are the same instance.
/// </summary>
public interface ISchema : IEnumerable<TItem>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance has an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Returns the index of the first element in this instance with the given identifier, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Returns the index of the last element in this instance with the given identifier, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Returns the indexes of the elements in this instance with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

    /// <summary>
    /// Determines if this instance has an element that matches the given predicate.
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
    /// Returns the indexes of the elements in this instance that match the given predicate.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements from the original instance,
    /// starting from the given index. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced
    /// by the new given one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If no
    /// changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Add(TItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the original one,
    /// at the given index. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance the original element at the given index has been removed from the
    /// original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number elements, starting from the given index,
    /// have been removed from the original one. If no changes were needed, returns the original
    /// instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given identifier has been removed
    /// from the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    THost Remove(string identifier);

    /// <summary>
    /// Returns a new instance where the last element with the given identifier has been removed
    /// from the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    THost RemoveLast(string identifier);

    /// <summary>
    /// Returns a new instance where all the elements with the given identifier have been 
    /// from the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    THost RemoveAll(string identifier);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all last elements that match the given predicate have been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed. If no changes
    /// were needed, returns the original instance instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}