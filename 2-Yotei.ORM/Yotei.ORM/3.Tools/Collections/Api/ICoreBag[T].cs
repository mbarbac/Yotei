namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a bag-alike collection of elements with no ordering guarantees.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreBag<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Invoked to return a validated element before using it in this collection.
    /// </summary>
    Func<T, T> ValidateItem { get; }

    /// <summary>
    /// Invoked to determine if the values that are themselves enumerations of elements of the
    /// type of this collection shall be flattened before using them, or not.
    /// </summary>
    bool FlattenElements { get; }

    /// <summary>
    /// Invoked to determine if, for the purposes of this collection, the two given elements are
    /// equal or not.
    /// </summary>
    Func<T, T, bool> CompareItems { get; }

    /// <summary>
    /// Invoked to find the duplicates of the given element.
    /// </summary>
    Func<T, IEnumerable<T>> GetDuplicates { get; }

    /// <summary>
    /// Invoked to determine if the 2nd argument, which is a duplicate of the 1st one, can be
    /// included in this collection, or not, by returning 'true' or 'false' as appropriate. In
    /// addition, may throw an exception if duplicates are not allowed.
    /// </summary>
    Func<T, T, bool> IncludeDuplicate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Determines if this collection contains at least one ocurrence of the given element, as
    /// determined by the rules in this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate and, if
    /// so and if the given delegate is not null, invokes that delegate with the found element.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, Action<T>? found = null);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate and, if
    /// so, returns the found element in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, out T item);

    /// <summary>
    /// Tries to find all the ocurrences of elements that match the given predicate and, if so
    /// and if the given delegate is not null, invokes that delegate with the found elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    bool FindAll(Predicate<T> predicate, Action<T>? found = null);

    /// <summary>
    /// Tries to find all the ocurrences of elements that match the given predicate and, if so,
    /// returns the found elements in the out argument.
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
    /// Removes from this collection the first ocurrence of the given element, as determined by
    /// the rules in this instance. If it is an empty collection of elements, and if this instance
    /// flattens input elements, then each element of that collection is removed instead. If the
    /// given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int Remove(T item, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element, as determined by
    /// the rules in this instance. If so, returns the removed elements in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    int Remove(T item, out List<T> items);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, as determined by the
    /// rules in this instance. If any is an empty collection of elements, and if this instance
    /// flattens input elements, then each element of that collection is removed instead. If the
    /// given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAll(T item, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, as determined by the
    /// rules in this instance and, if so, returns the removed elements in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    int RemoveAll(T item, out List<T> items);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate. If the
    /// given delegate is not null, it is invoked with the removed element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate. Returns
    /// the removed element, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate, out T removed);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate. If the
    /// given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate and, if so,
    /// returns the removed elements in the out argument.
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