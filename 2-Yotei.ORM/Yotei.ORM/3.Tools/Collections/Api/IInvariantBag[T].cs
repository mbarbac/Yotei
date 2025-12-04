namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable bag-alike collection of elements with no ordering guarantees.
/// <br/> The semantics are that two given elements are considered equal only if the equality
/// rules in this instance determine so.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantBag<T> : IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Represents a builder for <see cref="IInvariantBag{T}"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface ICoreBuilder : ICoreBag<T>
    {
        /// <summary>
        /// Returns a new invariant instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IInvariantBag<T> ToInvariant();
    }

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreBuilder ToBuilder();

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
    bool Contains(T item);

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
    bool Find(Predicate<T> predicate, out T found);

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
    /// <param name="found"></param>
    /// <returns></returns>
    bool FindAll(Predicate<T> predicate, out List<T> found);

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
    /// Returns a new instance with the given element added to it.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantBag<T> Add(T item);

    /// <summary>
    /// Returns a new instance with the elements of the given range added to it.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantBag<T> AddRange(IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the first ocurrence of the given element, as determined by
    /// the rules in this instance, removed. If it is itself a collection of elements, and this
    /// instance flattens input elements, then its own elements are removed instead. If the given
    /// delegate is not null, it is invoked with the removed element.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> Remove(T item, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the first ocurrence of the given element, as determined by
    /// the rules in this instance, removed. If it is itself a collection of elements, and this
    /// instance flattens input elements, then its own elements are removed instead. Returns the
    /// removed element in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> Remove(T item, out List<T> removed);

    /// <summary>
    /// Returns a new instance with all the ocurrences of the given element, as determined by
    /// the rules in this instance, removed. If it is itself a collection of elements, and this
    /// instance flattens input elements, then its own elements are removed instead. If the given
    /// delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> RemoveAll(T item, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with all the ocurrences of the given element, as determined by
    /// the rules in this instance, removed. If it is itself a collection of elements, and this
    /// instance flattens input elements, then its own elements are removed instead. Returns the
    /// removed elements in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> RemoveAll(T item, out List<T> removed);

    /// <summary>
    /// Returns a new instance with the first element that matches the given predicate removed.
    /// If the given delegate is not null, it is invoked with the removed element.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> Remove(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the first element that matches the given predicate removed.
    /// Returns the removed element in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> Remove(Predicate<T> predicate, out T removed);

    /// <summary>
    /// Returns a new instance with all the element that matches the given predicate removed.
    /// If the given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with all the elements that matches the given predicate removed.
    /// Returns the removed elements in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantBag<T> RemoveAll(Predicate<T> predicate, out List<T> removed);

    /// <summary>
    /// Returns a new instance with all the original elements removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    IInvariantBag<T> Clear();
}