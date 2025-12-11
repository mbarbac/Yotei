namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements.
/// <br/> The semantics are that two given elements are considered equal only if the equality
/// rules in this instance determine so.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<T> ToBuilder();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains at least one ocurrence of the given element, as
    /// determined by the rules in this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element, as determined by the rules
    /// in this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(T item);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element, as determined by the rules
    /// in this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Returns the indexes of all ocurrences of the given element, as determined by the rules in
    /// this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the indexes of all the ocurrences of elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

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
    /// Tries to find the last ocurrence of an element that matches the given predicate and, if
    /// so and if the given delegate is not null, invokes that delegate with the found element.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    bool FindLast(Predicate<T> predicate, Action<T>? found = null);

    /// <summary>
    /// Tries to find the last ocurrence of an element that matches the given predicate and, if
    /// so, returns the found element in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    bool FindLast(Predicate<T> predicate, out T found);

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
    /// Returns a list with the given number of elements of this collection, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> ToList(int index, int count);

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
    IInvariantList<T> Add(T item);

    /// <summary>
    /// Returns a new instance with the elements of the given range added to it.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the given element inserted into it at the given index.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<T> Insert(int index, T item);

    /// <summary>
    /// Returns a new instance with the elements from the given range inserted into it, starting
    /// at the given index.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> InsertRange(int index, IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the given one. If
    /// it is an empty collection of elements, and if this instance flattens input elements, then
    /// no replacement is made. If the given delegate is not null, it is invoked with the removed
    /// element.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> Replace(int index, T item, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the element at the given index replaced by the given one. If
    /// it is an empty collection of elements, and if this instance flattens input elements, then
    /// no replacement is made. Returns the removed element, if any, in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> Replace(int index, T item, out T removed);

    /// <summary>
    /// Returns a new instance with the element at the given index removed. f the given delegate
    /// is not null, it is invoked with the removed element.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAt(int index, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the element at the given index removed. Returns the removed
    /// element, if any, in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAt(int index, out T removed);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index,
    /// removed. If the given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveRange(int index, int count, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the given number of elements, starting at the given index,
    /// removed. Returns the removed elements, if any, in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveRange(int index, int count, out List<T> removed);

    /// <summary>
    /// Returns a new instance with the first ocurrence of the given element removed. If it is
    /// itself a collection of elements, and this instance flattens input elements, then its own
    /// elements are removed instead. If the given delegate is not null, it is invoked with the
    /// removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(T item, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the first ocurrence of the given element removed. If it is
    /// itself a collection of elements, and this instance flattens input elements, then its own
    /// elements are removed instead. Returns the removed elements in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(T item, out List<T> removed);

    /// <summary>
    /// Returns a new instance with the last ocurrence of the given element removed. If it is
    /// itself a collection of elements, and this instance flattens input elements, then its own
    /// elements are removed instead. If the given delegate is not null, it is invoked with the
    /// removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(T item, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the last ocurrence of the given element removed. If it is
    /// itself a collection of elements, and this instance flattens input elements, then its own
    /// elements are removed instead. Returns the removed elements in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(T item, out List<T> removed);

    /// <summary>
    /// Returns a new instance with all the ocurrences of the given element removed. If it is
    /// itself a collection of elements, and this instance flattens input elements, then its own
    /// elements are removed instead. If the given delegate is not null, it is invoked with the
    /// removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(T item, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with all the ocurrences of the given element removed. If it is
    /// itself a collection of elements, and this instance flattens input elements, then its own
    /// elements are removed instead. Returns the removed elements in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(T item, out List<T> removed);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element that matches the given
    /// predicate removed. If the given delegate is not null, it is invoked with the removed
    /// element.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the first ocurrence of an element that matches the given
    /// predicate removed. Returns the removed element in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(Predicate<T> predicate, out T removed);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element that matches the given
    /// predicate removed. If the given delegate is not null, it is invoked with the removed
    /// element.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with the last ocurrence of an element that matches the given
    /// predicate removed. Returns the removed element in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(Predicate<T> predicate, out T removed);

    /// <summary>
    /// Returns a new instance with all the ocurrences of elements that match the given predicate
    /// removed. If the given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Returns a new instance with all the ocurrences of elements that match the given predicate
    /// removed. Returns the removed elements in the out argument.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(Predicate<T> predicate, out List<T> removed);

    /// <summary>
    /// Returns a new instance with all the original elements removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    IInvariantList<T> Clear();
}