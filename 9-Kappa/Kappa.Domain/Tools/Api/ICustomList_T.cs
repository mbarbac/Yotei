namespace Kappa.Tools;

// ========================================================
/// <summary>
/// Represents a custom list-alike collection of elements.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICustomList<T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(T item);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Returns the indexes of the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if such cannot be found.
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
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    void Trim();

    /// <summary>
    /// Sorts this collection using the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    void Sort(IComparer<T> comparer);

    /// <summary>
    /// Reverses the order of the elements in this collection.
    /// </summary>
    void Reverse();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the given number of elements from this collection, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> GetRange(int index, int count);

    /// <summary>
    /// Replaces the element at the given index.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, T item);

    /// <summary>
    /// Adds the given element to this collection.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds the elements from the given range to this collection.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts the given element into this collection at the given index.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts the elements from the given range into this collection starting at the given index.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements starting from the given index.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Remove(T item);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveLast(T item);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveAll(T item);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this collection.
    /// Returns the number of changes made, or cero if any.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}