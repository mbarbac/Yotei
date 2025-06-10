namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable and customizable list-alike collection of elements that are identified
/// by their respective keys.
/// <para>Instances of this class are intended to be immutable ones.</para>
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
    /// Returns the index of the first element in this collection with the given key, or -1 if
    /// no one is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(K key);

    /// <summary>
    /// Returns the index of the last element in this collection with the given key, or -1 if
    /// no one is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(K key);

    /// <summary>
    /// Returns the indexes of all the elements in this collection with the given key.
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
    /// Returns the index of the first element in this collection that matches the given predicate,
    /// or -1 if no one is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that matches the given predicate,
    /// or -1 if no one is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the indexes of all the elements in this collection that match the given predicate.
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

    /// <summary>
    /// Trims the internal structures of this collection.
    /// <br/> Note that this method does not break the logical immutability of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<K, T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one.
    /// <br/> If an empty enumeration is given, then nothing is replaced (so this scenario cannot
    /// be used instead of the '<see cref="RemoveAt(int)"/>' method).
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<K, T> Replace(int index, T item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<K, T> Add(T item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<K, T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection, at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<K, T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<K, T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given key has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<K, T> Remove(K key);

    /// <summary>
    /// Returns a new instance where the last element with the given key has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveLast(K key);

    /// <summary>
    /// Returns a new instance where all the elements with the given key have been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveAll(K key);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<K, T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<K, T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IInvariantList<K, T> Clear();
}