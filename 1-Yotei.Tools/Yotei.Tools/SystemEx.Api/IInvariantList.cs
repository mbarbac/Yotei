namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IInvariantList<T> : IEnumerable<T>, ICloneable
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    new IInvariantList<T> Clone();

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory footprint of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or
    /// -1 if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(T item);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or
    /// -1 if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Returns a list containing the indexes of all the ocurrences of the given element in this
    /// collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate, or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns a list containing the indexes of all the elements in this collection that match
    /// the given predicate.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the given number of elements from the given index,
    /// from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced
    /// by the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Replace(int index, T item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Add(T item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted, at the given index,
    /// into the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the original collection, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given element has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(T item);

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given element has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(T item);

    /// <summary>
    /// Returns a new instance where all the ocurrence of the given element have been removed
    /// from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(T item);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where all the ocurrences of elements that match the given
    /// predicate have been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IInvariantList<T> Clear();
}