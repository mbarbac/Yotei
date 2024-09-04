namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collecion of elements identified by their respective keys, with
/// customizable behavior.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<K, T>
    : IList<T>, IList, ICollection<T>, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
{
    /// <summary>
    /// Invoked to validate the given item before using it in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    T ValidateItem(T item);

    /// <summary>
    /// Invoked to obtain the key associated with the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    K GetKey(T item);

    /// <summary>
    /// Invoked to validate the given key before using it in this collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    K ValidateKey(K key);

    /// <summary>
    /// Invoke to determine if the two given keys shall be considered the same, or not.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool CompareKeys(K x, K y);

    /// <summary>
    /// Invoked to obtain the indexes of the existing elements whose keys shall be considered
    /// duplicates of the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> GetDuplicates(K key);

    /// <summary>
    /// Invoked to determine if the given element (item) can be added or inserted into this
    /// collection, even if its key has been considered a duplicate of the key of the given
    /// existing element. This method returns:
    /// <br/>- <c>true</c> if the item can be added or inserted.
    /// <br/>- <c>false</c> if the add or insert operation shall be ignored.
    /// <br/>- Or throws an appropriate exception if duplicated keys are not allowed.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    bool CanInclude(T item, T source);

    /// <summary>
    /// Invoked to determine if, when a given element is itself a collection of elements of the
    /// type this instance is built for, that element shall be expanded before using it in this
    /// collection, and then its own ones used instead, or not.
    /// </summary>
    /// <returns></returns>
    bool ExpandItems();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the current number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

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

    /// <summary>
    /// Reverses the order of the elements in this collection.
    /// </summary>
    void Reverse();

    /// <summary>
    /// Sorts the elements of this collection, by their respective keys, using the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    void Sort(IComparer<K> comparer);

    /// <summary>
    /// Gets or sets the total number of elements the internal data structures can hold without
    /// resizing.
    /// </summary>
    int Capacity { get; set; }

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    /// <summary>
    /// Returns a list with a shallow copy of the given number of elements, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> GetRange(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    int Replace(int index, T item);

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    new int Add(T item);

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>The number of changes made.</returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of changes made.</returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>The number of changes made.</returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The number of changes made.</returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of changes made.</returns>
    int Remove(K key);

    /// <summary>
    /// Removes from this collection the last element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(K key);

    /// <summary>
    /// Removes from this collection all the elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(K key);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns>The number of changes made.</returns>
    new int Clear();
}