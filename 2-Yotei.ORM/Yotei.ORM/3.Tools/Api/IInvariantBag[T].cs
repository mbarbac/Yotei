namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable bag-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantBag<T> : IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Returns a new builder based upon the contents in this instance.
    /// </summary>
    /// <returns></returns>
    ICoreBag<T> ToBuilder();

    /// <summary>
    /// Get the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this instance contains an ocurrence of the given value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(T value);

    /// <summary>
    /// Determines if this instance contains a element that matches the given predicate, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Tries to find a value in this collection that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find all the values in this collection that match the given predicate.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> Add(T value);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> AddRange(IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where an ocurrence of the given element has been removed,
    /// if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> Remove(T value);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of the given value have been
    /// removed, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> RemoveAll(T value);

    /// <summary>
    /// Returns a copy of this instance where an element that matches the given predicate, if
    /// any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all elements that match the given predicate, if
    /// any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns an empty copy of this instance, but keeping all configurations.
    /// </summary>
    /// <returns>This instance if no changes were made.</returns>
    IInvariantBag<T> Clear();
}