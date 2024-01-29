namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements identified by their respective
/// keys with customizable behavior.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IFrozenList<K, T> : IEnumerable<T>
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
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(K key);

    /// <summary>
    /// Gets the index of the first element in this collection with the given key, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(K key);

    /// <summary>
    /// Gets the index of the last element in this collection with the given key, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(K key);

    /// <summary>
    /// Gets the indexes of elements in this collection with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(K key);

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
    /// Returns a new instance that contains the given number of elements starting at the given
    /// index. If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IFrozenList<K, T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the new given one.
    /// If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<K, T> Replace(int index, T item);

    /// <summary>
    /// Returns a new instance with the given element added to it. If no changes are detected,
    /// returns the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<K, T> Add(T item);

    /// <summary>
    /// Returns a new instance with the elements of the given range added to it. If no changes
    /// are detected, returns the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IFrozenList<K, T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the given element inserted into it, at the given index. If
    /// no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IFrozenList<K, T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance with the element of the given range inserted into it, starting at
    /// the given index. If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IFrozenList<K, T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the element at the given index removed from it. If no changes
    /// are detected, returns the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IFrozenList<K, T> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index,
    /// removed from it. If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IFrozenList<K, T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the first element with the given key removed from it. If no
    /// changes are detected, returns the original collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IFrozenList<K, T> Remove(K key);

    /// <summary>
    /// Returns a new instance with the last element with the given key removed from it. If no
    /// changes are detected, returns the original collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IFrozenList<K, T> RemoveLast(K key);

    /// <summary>
    /// Returns a new instance with all the elements with the given key removed from it. If no
    /// changes are detected, returns the original collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IFrozenList<K, T> RemoveAll(K key);

    /// <summary>
    /// Returns a new instance with the first element that matches the given predicate removed
    /// from it. If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<K, T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with the last element that matches the given predicate removed
    /// from it. If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<K, T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the elements that match the given predicate removed from
    /// it. If no changes are detected, returns the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IFrozenList<K, T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the original elements removed. If no changes are detected,
    /// returns the original collection.
    /// </summary>
    /// <returns></returns>
    IFrozenList<K, T> Clear();
}