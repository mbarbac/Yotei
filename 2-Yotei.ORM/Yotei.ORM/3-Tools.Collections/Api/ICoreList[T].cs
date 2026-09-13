namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<T> : IList<T>, IList, ICollection<T>, ICollection
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
    /// Determines if this collection contains the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new bool Contains(T value);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new int IndexOf(T value);

    /// <summary>
    /// Returns the index of the last ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(T value);

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(T value);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryFind(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find the last ocurrence of an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryFindLast(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find all the ocurrences of elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool TryFindAll(Predicate<T> predicate, out List<T> range);

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
    /// Returns a list with the requested number of elements from this collection, starting at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> ToList(int index, int count);

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the value at the given index by the given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    int Replace(int index, T value);

    /// <summary>
    /// Adds to this collection the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    new int Add(T value);

    /// <summary>
    /// Adds to this collection the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>The number of changes made.</returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts into this collection the given value at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    new int Insert(int index, T value);

    /// <summary>
    /// Inserts into this collection the elements of the given range, starting at the given index.
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
    /// Removes from this collection the requested number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    new int Remove(T value);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(T value);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(T value);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate, if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate, if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given predicate,
    /// if any.
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