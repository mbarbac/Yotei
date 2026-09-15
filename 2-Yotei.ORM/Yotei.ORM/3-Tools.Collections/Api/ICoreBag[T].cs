namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a bag-alike collection of elements.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreBag<T> : ICollection<T>, ICollection
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this collection contains the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new bool Contains(T value);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryFind(Predicate<T> predicate, out T value);

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
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

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
    /// Removes from this collection the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    new int Remove(T value);

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