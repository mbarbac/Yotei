namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identifed by their respective keys, with
/// customizable behavior.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<K, T> : IList<T>, IList, ICollection<T>, ICollection
{
    string ToDebugString(int count);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    Func<T, T> ValidateItem { get; set; }

    /// <summary>
    /// Invoked to extract the key associated with the given element.
    /// </summary>
    Func<T, K> GetKey { get; set; }

    /// <summary>
    /// Invoked to validate the given key.
    /// </summary>
    Func<K, K> ValidateKey { get; set; }

    /// <summary>
    /// Invoked to determine equality of keys.
    /// </summary>
    Func<K, K, bool> CompareKeys { get; set; }

    /// <summary>
    /// Invoked to get the indexes of the elements whose keys shall be considered duplicates to
    /// the given one.
    /// </summary>
    Func<ICoreList<K, T>, K, List<int>> Duplicates { get; set; }

    /// <summary>
    /// Invoked to determines if the given element can be included in this collection. Returns
    /// <see langword="true"/> if so or <see langword="false"/> if the include operation shall
    /// be ignored. In addition, it can throw an appropriate exception if duplicates are not
    /// allowed.
    /// </summary>
    Func<T, T, bool> CanInclude { get; set; }

    // ----------------------------------------------------

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
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(K key);

    /// <summary>
    /// Gets the index of the first element in this collection with the given key, or -1 if
    /// it is not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(K key);

    /// <summary>
    /// Gets the index of the last element in this collection with the given key, or -1 if
    /// it is not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(K key);

    /// <summary>
    /// Gets the indexes of the elements in this collection with the given key.
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
    /// Replaces the element at the given index with the new given one. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, T item);

    /// <summary>
    /// Adds to this collection the given element. Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds to this collection the elements from the given range. Returns the number of changes
    /// made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts into this collection the given element at the given index. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first element with the given key. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int Remove(K key);

    /// <summary>
    /// Removes from this collection the last element with the given key. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveLast(K key);

    /// <summary>
    /// Removes from this collection all the elements with the given key. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveAll(K key);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears all the elements in this collection. Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}