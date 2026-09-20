namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable bag-alike collection of elements.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantBag<T> : IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Returns a mutable builder based upon the current contents of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreBag<T> ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this collection contains the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(T value);

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
    /// Returns a copy of this instance where the given value has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> Add(T value);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of the given value, if any, has
    /// been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> Remove(T value);

    /// <summary>
    /// Returns a copy of this instance where the all the ocurrences of the given value,  if any,
    /// have been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> RemoveAll(T value);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate, if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of elements that match the given
    /// predicate, if any, have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance that has been cleared.
    /// </summary>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IInvariantBag<T> Clear();
}