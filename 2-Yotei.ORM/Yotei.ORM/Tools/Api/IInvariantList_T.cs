namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements that provides customizable item
/// validation and duplicates acceptance or denial capabilities.
/// </summary>
/// <typeparam name="TItem"></typeparam>
[Cloneable]
public partial interface IInvariantList<TItem> : IEnumerable<TItem>
{
    /// <summary>
    /// Determines if the given element is valid for this collection, or throws an appropriate
    /// exception otherwise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TItem Validate(TItem item);

    /// <summary>
    /// Determines if the given target element, being a duplicate of the given source one, can be
    /// added or inserted. Returns '<c>true</c>' if it can be added or inserted, '<c>false</c>' if
    /// it shall just be ignored, or throws an appropriate exception if this instance does not
    /// accept that duplicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool CanDuplicate(TItem source, TItem target);

    /// <summary>
    /// Determines if the given target element can be considered equal to the given source one,
    /// or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Compare(TItem source, TItem target);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains an element that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(TItem item);

    /// <summary>
    /// Returns the index of the first element in this instance that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(TItem item);

    /// <summary>
    /// Returns the index of the last element in this instance that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(TItem item);

    /// <summary>
    /// Returns the index of all the elements in this instance that match the given one, or -1
    /// if any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(TItem item);

    /// <summary>
    /// Determines if this instance contains an element that matches the the given predicate.
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
    /// Returns the index of all the element in this instance that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this instace.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instace.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index.
    /// If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<TItem> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TItem> Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If no
    /// changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TItem> Add(TItem item);

    /// <summary>
    /// Returns a new instance where the elements of the given range have been added to the
    /// original one. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<TItem> AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted at the given index. If
    /// no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TItem> Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been inserted at the given index. If
    /// no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<TItem> InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed. If no
    /// changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<TItem> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed. If no changes are detected, then the original instance is returned
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<TItem> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element that matches the given one is removed. If
    /// no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TItem> Remove(TItem item);

    /// <summary>
    /// Returns a new instance where the last element that matches the given one is removed. If
    /// no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TItem> RemoveLast(TItem item);

    /// <summary>
    /// Returns a new instance where all the elements that match the given one have been removed.
    /// If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TItem> RemoveAll(TItem item);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<TItem> Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<TItem> RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<TItem> RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed. If no changes are
    /// detected, then the original instance is returned instead.
    /// </summary>
    /// <returns></returns>
    IInvariantList<TItem> Clear();
}