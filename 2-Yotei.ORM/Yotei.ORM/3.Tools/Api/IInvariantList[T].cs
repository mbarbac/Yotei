namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Returns a new builder based upon the contents in this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<T> ToBuilder();

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
    /// Determines if this instance contains an ocurrence of the given value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(T value);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value, or -1 if it cannot be found
    /// using the rules in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(T value);

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
    /// Returns the index of all the values in this collection that match the given predicate.
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
    /// <param name="other"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> GetRange(int index, T other);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been replaced
    /// by the other given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="other"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> Replace(int index, T other);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> Add(T value);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> Insert(int index, T value);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been inserted
    /// into it starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> InsertRange(int index, IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the requested number of elements, starting at the
    /// given index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of the given element has been
    /// removed, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> Remove(T value);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of the given element has been
    /// removed, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> RemoveLast(T value);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of the given element have been
    /// removed, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> RemoveAll(T value);

    /// <summary>
    /// Returns a copy of this instance where the first element that matches the given predicate,
    /// if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last element that matches the given predicate,
    /// if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the elements that matches the given predicate,
    /// if any, have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns an empty copy of this instance, but keeping all configurations.
    /// </summary>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantList<T> Clear();
}