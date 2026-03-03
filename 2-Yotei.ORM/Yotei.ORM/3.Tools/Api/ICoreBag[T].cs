namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a bag-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreBag<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Get the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this instance contains an ocurrence of the given value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new bool Contains(T value);

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

    // ----------------------------------------------------

    /// <summary>
    /// Removes from this instance a ocurrence of the given value, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new int Remove(T value);

    /// <summary>
    /// Removes from this instance all the ocurrences of the given value, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int RemoveAll(T value);

    /// <summary>
    /// Removes from this instance a value that matches the given predicate, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

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