namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identified by their respective keys that
/// provides custom item validation and duplicates acceptance or denial capabilities.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
public interface ICoreList<K, T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>
{
    /// <summary>
    /// The (item, scenario) delegate invoked to return a validated element for this instance and
    /// scenario, it being 'true' if the element is to be added or inserted, or 'false' otherwise.
    /// Throws an exception if the element is not a valid one.
    /// </summary>
    Func<T, bool, T> ValidateItem { get; set; }

    /// <summary>
    /// The (item) delegate invoked to obtain the key associated with the given element.
    /// Throws an exception if the key cannot be obtained.
    /// </summary>
    Func<T, K> GetKey { get; set; }

    /// <summary>
    /// The (key) delegate invoked to return a validated key for this instance.
    /// Throws an exception if the key is not a valid one.
    /// </summary>
    Func<K, K> ValidateKey { get; set; }

    /// <summary>
    /// The (source, target) delegate invoked to compare the key of a source element in this
    /// instance against the given target one.
    /// </summary>
    Func<K, K, bool> Compare { get; set; }

    /// <summary>
    /// The (source, target) delegate invoked to determine if the given source element is the
    /// same as the given target one, to prevent replacing it.
    /// </summary>
    Func<T, T, bool> IsSame { get; set; }

    /// <summary>
    /// The (source, target) delegate invoked to determine if the given duplicated target element
    /// can be added or inserted into this instance. If not, then the element is just ignored. It
    /// is expected that an exception is thrown if duplicates are not allowed.
    /// </summary>
    Func<T, T, bool> ValidDuplicate { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index. An exception may be thrown if the new one
    /// is a duplicate of an existing element.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this instance has at least one element whose key matches the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(K key);

    /// <summary>
    /// Returns the index of the first element in this instance whose key matches the given one,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(K key);

    /// <summary>
    /// Returns the index of the last element in this instance whose key matches the given one,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(K key);

    /// <summary>
    /// Returns the indexes of the elements in this instance whose keys match the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(K key);

    /// <summary>
    /// Determines if this instance has at least one element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of all the elements in this instance that match the given predicate..
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Returns an array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Returns a list with the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> GetRange(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the new given one, unless both are the
    /// same instance. Returns how many changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, T item);

    /// <summary>
    /// Adds the given element to this collection. Returns how many changes have been made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds the elements of the given range to this collection. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts the given element into this collection at the given index. Returns how many
    /// changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts the elements of the given range into this collection, starting at the given index.
    /// Returns how many changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes the element at the given index. Returns how many changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes the given number of elements, starting at the given index. Returns how many
    /// changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes the first element whose key matches the given one. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int Remove(K key);

    /// <summary>
    /// Removes the last element whose key matches the given one. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveLast(K key);

    /// <summary>
    /// Removes all the elements whose keys match the given one. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveAll(K key);

    /// <summary>
    /// Removes the first element that matches the given predicate. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes the last element that matches the given predicate. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes all the elements that match the given predicate. Returns how many changes have
    /// been made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this instance by removing all its elements. Returns how many changes have been
    /// made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}