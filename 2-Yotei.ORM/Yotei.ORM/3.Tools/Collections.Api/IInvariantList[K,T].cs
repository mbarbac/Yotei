namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable collection of elements identified by their respective keys, with
/// customizable behavior.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantList<K, T> : IReadOnlyList<T>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(K key);

    /// <summary>
    /// Returns the index of the first element whose key matches the given one, or -1 if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(K key);

    /// <summary>
    /// Returns the index of the last element whose key matches the given one, or -1 if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(K key);

    /// <summary>
    /// Returns the indexes of all the elements whose key match the given one.
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
    /// Returns the index of the first element that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the indexes of all the elements that match the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Returns an array with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Returns a list with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Returns a list with the given number of elements from this collection, starting at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> ToList(int index, int count);

    /// <summary>
    /// Trims the excess of capacity in this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting from the given index. 
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<K, T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the new one.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<K, T> Replace(int index, T item);

    /// <summary>
    /// Returns a new instance with the given element added to the collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<K, T> Add(T item);

    /// <summary>
    /// Returns a new instance with the elements of the given range added to the collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<K, T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the given element inserted into the collection at the given
    /// index. <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<K, T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance with the elements of the given range inserted into the collection,
    /// starting at the given index. <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<K, T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the element at the given index removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index,
    /// removed. <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the first element whose key matches the given one removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<K, T> Remove(K key);

    /// <summary>
    /// Returns a new instance with the last element whose key matches the given one removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveLast(K key);

    /// <summary>
    /// Returns a new instance with all the elements whose key matches the given one removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveAll(K key);

    /// <summary>
    /// Returns a new instance with the first element that matches the given predicate removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<K, T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with the last element that matches the given predicate removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the elements that match the given predicate removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the elements removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    IInvariantList<K, T> Clear();
}