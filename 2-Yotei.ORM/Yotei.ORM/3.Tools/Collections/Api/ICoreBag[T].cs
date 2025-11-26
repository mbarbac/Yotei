namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a bag-alike collection of elements with customizable behavior, but with no
/// ordering guarantees.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreBag<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate and,
    /// if so, returns the first found one in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, [MaybeNull] out T item);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate and,
    /// if so, returns them in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    bool FindAll(Predicate<T> predicate, out List<T> items);

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

    /// <summary>
    /// Removes from this collection the first ocurrence found of the given element, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Remove(T item);

    /// <summary>
    /// Removes all the ocurrences of the given element from this collection, if any, and returns
    /// the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    int RemoveAll(T item, out List<T> items);

    /// <summary>
    /// Removes from this collection the first ocurrence found of an element that matches the
    /// given predicate, and returns the removed one, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate, [MaybeNull] out T item);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given predicate
    /// and returns the removed ones, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate, out List<T> items);

    /// <summary>
    /// Clears this collection.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}