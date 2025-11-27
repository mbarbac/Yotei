namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a bag-alike collection of elements.
/// <br/> Each instance define its own behavior related to the equality comparison of elements,
/// to whether null (for nullable types) and duplicated values are allowed or not, if elements
/// that are themselves collections of elements shall be flattened before using them, as well as
/// any other rules or behaviors appropriate for their context.
/// </summary>
/// <typeparam name="T"></typeparam>
/// NOTE: By default it is expected that the default behaviors mimics the ones of the standard
/// collections, unless explicit overrides are used.
[Cloneable]
public partial interface ICoreBag<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this collection contains at least one ocurrence of the given value, as
    /// determined by the rules in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    new bool Contains(T value);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate and,
    /// if so, returns it in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find all the ocurrences of elements that match the given predicate and returns
    /// them in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool FindAll(Predicate<T> predicate, out List<T> values);

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
    /// Trims the internal structures used by this instance.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this collection the given element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds to this collection the elements of the given range.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from this collection the first ocurrence of the given value, as determined by the
    /// rules in this instance. If a remove delegate is provided, it is invoked with each of the
    /// removed elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="onremove"></param>
    /// <returns></returns>
    int Remove(T value, Action<T>? onremove = null);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given value, as determined by the
    /// rules in this instance. If a remove delegate is provided, it is invoked with each of the
    /// removed elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="onremove"></param>
    /// <returns></returns>
    int RemoveAll(T value, Action<T>? onremove = null);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate. If a remove delegate is provided, it is invoked with each of the removed
    /// elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremove"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate, Action<T>? onremove = null);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that matches the given
    /// predicate. If a remove delegate is provided, it is invoked with each of the removed
    /// elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremove"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate, Action<T>? onremove = null);

    /// <summary>
    /// Clears this collection.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}