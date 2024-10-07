namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collecion of elements identified by their respective keys.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
// Note: Changes must be kept in sync with the 'FrozenGenerator' package.
[Cloneable]
public partial interface IFrozenList<K, T> : IReadOnlyList<T>
{
    /// <summary>
    /// Returns a new builder using the contents of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<K, T> ToBuilder();

    /// <summary>
    /// Gets the current number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given key, or not.
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
    /// Gets the indexes of all the elements in this collection with the given key.
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
    /// Gets the indexes of all the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    // ----------------------------------------------------

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
    /// Returns a new instance where the order of the original elements has been reversed.
    /// </summary>
    /// <returns></returns>
    IFrozenList<K, T> Reverse();

    /// <summary>
    /// Returns a new instance where the original elements have been sorted using the given
    /// comparer.
    /// </summary>
    /// <param name="comparer"></param>
    /// <returns></returns>
    IFrozenList<K, T> Sort(IComparer<K> comparer);

    /// <summary>
    /// Returns a new instance that contains a shallow copy of the given number of elements,
    /// starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced
    /// by the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> Replace(int index, T item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original ones.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> Add(T item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original ones.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted at the given index into
    /// the original ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance where where the elements from the given range have been inserted
    /// starting at the given index into the original ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed from the original ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given key has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> Remove(K key);

    /// <summary>
    /// Returns a new instance where the last element with the given key has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> RemoveLast(K key);

    /// <summary>
    /// Returns a new instance where all the elements with the given key have been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> RemoveAll(K key);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns>A new instance, or the original one if no changes are made.</returns>
    IFrozenList<K, T> Clear();
}