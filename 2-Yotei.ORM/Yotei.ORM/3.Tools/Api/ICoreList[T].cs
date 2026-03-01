namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICoreList<T>
    : IList<T>, IReadOnlyList<T>, IList
    , ICollection<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Get the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this instance contains an ocurrence of the given value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new bool Contains(T value);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be found
    /// using the rules in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new int IndexOf(T value);

    /// <summary>
    /// Returns the index of the last ocurrence of the given value, or -1 if it cannot be found
    /// using the rules in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(T value);

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(T value);

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

    /// <summary>
    /// Returns a copy of this instance with the requested number of elements starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    ICoreList<T> GetRange(int index, T other);

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given value to this instance.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new int Add(T value);

    /// <summary>
    /// Adds the elements from the given range to this instance.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts the given value into this instance at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    new int Insert(int index, T value);

    /// <summary>
    /// Inserts the elements from the given range into this instance, starting at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from this instance the value at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this instance the requested number of elements, starting at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this instance the first ocurrence of the given value, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new int Remove(T value);

    /// <summary>
    /// Removes from this instance the last ocurrence of the given value, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int RemoveLast(T value);

    /// <summary>
    /// Removes from this instance all the ocurrences of the given value, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int RemoveAll(T value);

    /// <summary>
    /// Removes from this instance the first value that matches the given predicate, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this instance the last value that matches the given predicate, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this instance all the values that match the given predicate, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this instance, but keeps all its configurations.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}