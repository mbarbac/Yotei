namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a frozen list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IFrozenList<T> : IEnumerable<T>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains the given element or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(T item);

    /// <summary>
    /// Gets the index of the first ocurrence of the given element in this collection, of -1 if
    /// it is not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(T item);

    /// <summary>
    /// Gets the index of the last ocurrence of the given element in this collection, of -1 if
    /// it is not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Gets the indexes of all the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

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

    /// <summary>
    /// Gets a list with the given number of elements, from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> GetRange(int index, int count);

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // -----------------------------------------------------

    /// <summary>
    /// Returns a new copy of this instance where it has been reduced to to the given number
    /// of elements starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Reduce(int index, int count);

    /// <summary>
    /// Returns a new copy of this instance where the original element at the given index has
    /// been replaced by the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Replace(int index, T item);

    /// <summary>
    /// Returns a new copy of this instance where the given element has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Add(T item);

    /// <summary>
    /// Returns a new copy of this instance where the elements from the given range have been
    /// added to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new copy of this instance where the given element has been inserted into it,
    /// at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Insert(int index, T item);

    /// <summary>
    /// Returns a new copy of this instance where the elements from the given range have been
    /// inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a new copy of this instance where the original element at the given index has
    /// been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> RemoveAt(int index);

    /// <summary>
    /// Returns a new copy of this instance where the given number of elements, starting at the
    /// given index, have been removed from it.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new copy of this instance where the first ocurrence of the given element has
    /// been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Remove(T item);

    /// <summary>
    /// Returns a new copy of this instance where the last ocurrence of the given element has
    /// been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> RemoveLast(T item);

    /// <summary>
    /// Returns a new copy of this instance where all the ocurrences of the given element have
    /// been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> RemoveAll(T item);

    /// <summary>
    /// Returns a new copy of this instance where the first element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new copy of this instance where the last element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new copy of this instance where all the elements that match the given predicate
    /// have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new copy of this instance where all its elements have been removed.
    /// </summary>
    /// <returns>If no changes were detected, returns the original instance instead.</returns>
    IFrozenList<T> Clear();
}