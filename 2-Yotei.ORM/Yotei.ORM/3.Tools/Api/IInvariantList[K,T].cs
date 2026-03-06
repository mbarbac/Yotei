namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements, identified by their respective
/// keys, with customizable behavior.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantList<K, T> : IReadOnlyList<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Returns a new builder based upon the contents in this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<K, T> ToBuilder();

    /// <summary>
    /// Get the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains an ocurrence of the given key, or not.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(K key);

    /// <summary>
    /// Returns the index of the first ocurrence of an element with the given key, or -1 if it
    /// cannot be found using the rules in this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(K key);

    /// <summary>
    /// Returns the index of the last ocurrence of an element with the given key, or -1 if it
    /// cannot be found using the rules in this instance.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(K key);

    /// <summary>
    /// Returns the indexes of all the ocurrences of elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(K key);

    /// <summary>
    /// Determines if this instance contains a element that matches the given predicate, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first value in this collection that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last value in this collection that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of all the values in this collection that matche the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the first ocurrence of a value that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find the last ocurrence of a value that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool FindLast(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find all the ocurrences of values that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool FindAll(Predicate<T> predicate, out List<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Returns a list with the requested number of elements, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> ToList(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance with the requested number of elements starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been replaced
    /// by the other given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="other"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> Replace(int index, T other);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> Add(T value);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> Insert(int index, T value);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been inserted
    /// into it starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> InsertRange(int index, IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the requested number of elements, starting at the
    /// given index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first element with the given key has been
    /// removed, if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> Remove(K key);

    /// <summary>
    /// Returns a copy of this instance where the last element with the given key has been
    /// removed, if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> RemoveLast(K key);

    /// <summary>
    /// Returns a copy of this instance where all the elements with the given key have been
    /// removed, if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> RemoveAll(K key);

    /// <summary>
    /// Returns a copy of this instance where the first element that matches the given predicate,
    /// if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last element that matches the given predicate,
    /// if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the elements that matches the given predicate,
    /// if any, have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns an empty copy of this instance, but keeping all configurations.
    /// </summary>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<K, T> Clear();
}