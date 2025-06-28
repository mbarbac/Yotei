namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a customizable list-alike collection of elements that are identified by their
/// respective keys.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<K, T>
    : IList<T>, IList, IReadOnlyList<T>
    , ICollection<T>, ICollection, IReadOnlyCollection<T>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

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
    /// Gets or sets the total number of elements the internal data structures can hold without
    /// resizing.
    /// </summary>
    int Capacity { get; set; }

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the given one.
    /// <br/> If an empty enumeration is given, then nothing is replaced (so this scenario cannot
    /// be used instead of the '<see cref="RemoveAt(int)"/>' method).
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, T item);

    /// <summary>
    /// Adds to this collection the given element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first element with the given key.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int Remove(K key);

    /// <summary>
    /// Removes from this collection the last element with the given key.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveLast(K key);

    /// <summary>
    /// Removes from this collection all the elements with the given key.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveAll(K key);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this collection.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}