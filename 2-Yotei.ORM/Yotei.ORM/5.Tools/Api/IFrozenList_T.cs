namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements, identified by their respective
/// keys, with customizable behvior.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IFrozenList<TItem> : IEnumerable<TItem>
{
    /// <summary>
    /// Returns a builder collection of the appropriate type with the elements of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<TItem> ToBuilder();

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
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(TItem item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element, or -1 if not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(TItem item);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element, or -1 if not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(TItem item);

    /// <summary>
    /// Returns the indexes of the ocurrences of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(TItem item);

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
    IFrozenList<TItem> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the new given one.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<TItem> Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the given element added to it. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<TItem> Add(TItem item);

    /// <summary>
    /// Returns a new instance with the elements from the given range add to it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IFrozenList<TItem> AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the given element inserted into it at the given index. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<TItem> Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance with the elements from the given range inserted into it, starting
    /// at the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IFrozenList<TItem> InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance with the element at the given index removed from it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IFrozenList<TItem> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting from the given index,
    /// removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IFrozenList<TItem> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the first ocurrence of the given element removed from it. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<TItem> Remove(TItem item);

    /// <summary>
    /// Returns a new instance with the last ocurrence of the given element removed from it. If
    /// no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<TItem> RemoveLast(TItem item);

    /// <summary>
    /// Returns a new instance with all ocurrences of the given element removed from it. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<TItem> RemoveAll(TItem item);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element that matches the given
    /// predicate removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<TItem> Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element that matches the given
    /// predicate removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<TItem> RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with all the ocurrences of elements that match the given predicate
    /// removed from it. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<TItem> RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance with all the original elements removed. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <returns></returns>
    IFrozenList<TItem> Clear();
}