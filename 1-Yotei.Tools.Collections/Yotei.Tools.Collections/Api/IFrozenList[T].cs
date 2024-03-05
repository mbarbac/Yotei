namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IFrozenList<T> : IEnumerable<T>
{
    string ToDebugString(int count);

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
    T this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(T item);

    /// <summary>
    /// Gets the index of the first ocurrence of the given element in this collection, or -1 if
    /// it is not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(T item);

    /// <summary>
    /// Gets the index of the last ocurrence of the given element in this collection, or -1 if
    /// it is not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Gets the index of the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Gets the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Gets the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Gets the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Gets an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Gets a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the rquested number of elements, starting from the
    /// given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IFrozenList<T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the new given one.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<T> Replace(int index, T item);

    /// <summary>
    /// Returns a new instance with the given element added to it. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<T> Add(T item);

    /// <summary>
    /// Returns a new instance with the elements from the given range added to it. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IFrozenList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the given element inserted to it, at the given index. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance with the elements from the given range inserted to it, starting at
    /// the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IFrozenList<T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the element at the given index removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IFrozenList<T> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the requested number or elements, starting from the given
    /// index, removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IFrozenList<T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the first ocurrence of the given element removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<T> Remove(T item);

    /// <summary>
    /// Returns a new instance with the last ocurrence of the given element removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<T> RemoveLast(T item);

    /// <summary>
    /// Returns a new instance with all the ocurrences of the given element removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<T> RemoveAll(T item);

    /// <summary>
    /// Returns a new instance with the first element that matches the given predicate removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with the last element that matches the given predicate removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the elements that match the given predicate removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the original elements removed. If no changes are detected,
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    IFrozenList<T> Clear();
}