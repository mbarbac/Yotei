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
    /// Invoked to return a validated element before using it in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    T Validate(T item);

    /// <summary>
    /// Determines whether elements that are themselves collections of elements of the type of
    /// this instance will be flattened when included in this collection, or not
    /// </summary>
    bool FlattenElements { get; }

    /// <summary>
    /// Determines whether the given item, which has been found to be a duplicate of the existing
    /// source element, can be included in this collection or not. This method shall:
    /// <br/>- Return '<c>true</c>' to include the given duplicated element.
    /// <br/>- Return '<c>false</c>' to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool IsValidDuplicate(T source, T item);

    /// <summary>
    /// The comparer used by this instance to determine equality of elements.
    /// </summary>
    IEqualityComparer<T> Comparer { get; }

    // ----------------------------------------------------

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
    /// Determines if this collection contains elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Determines if this collection contains elements that match the given predicate and,
    /// if so, returns that elements in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate, out List<T> items);

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
    /// Removes one ocurrence of the given element from this collection, if any.
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
    /// Removes from this collection one ocurrence of an element that matches the given predicate,
    /// and returns the removed one, if any, in the out argument.
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