namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements.
/// <br/> The semantics are that two given elements are considered equal only if the equality
/// rules in this instance determine so.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<T>
    : IList<T>, IReadOnlyList<T>, IList
    , ICollection<T>, IReadOnlyCollection<T>, ICollection
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains at least one ocurrence of the given element, as
    /// determined by the rules in this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element, as determined by the rules
    /// in this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(T item);

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

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts into this collection the elements of the given range starting at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the given one. If it is an empty collection
    /// of elements, and if this instance flattens input elements, then no replacement is made.
    /// If the given delegate is not null, it is invoked with the removed element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int Replace(int index, T item, Action<T>? removed = null);

    /// <summary>
    /// Replaces the element at the given index with the given one. If it is an empty collection
    /// of elements, and if this instance flattens input elements, then no replacement is made.
    /// Returns the removed element, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int Replace(int index, T item, out T removed);

    /// <summary>
    /// Removes from this collection the element at the given index. If the given delegate is not
    /// null, it is invoked with the removed element.
    /// <br/> Returns whether the element has been removed or not.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    bool RemoveAt(int index, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the removed element,
    /// if any, in the out argument.
    /// <br/> Returns whether the element has been removed or not.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    bool RemoveAt(int index, out T removed);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// If the given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// Returns the removed elements, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count, out List<T> removed);

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
    /// <param name="removed"></param>
    /// <returns></returns>
    int Remove(T item, out List<T> removed);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element, as determined by
    /// the rules in this instance. If it is an empty collection of elements, and if this instance
    /// flattens input elements, then each element of that collection is removed instead. If the
    /// given delegate is not null, it is invoked with the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLast(T item, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element, as determined by
    /// the rules in this instance. If so, returns the removed elements in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLast(T item, out List<T> removed);

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
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAll(T item, out List<T> removed);

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
    /// Removes from this collection the last element that matches the given predicate. If the
    /// given delegate is not null, it is invoked with the removed element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate, Action<T>? removed = null);

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate. Returns
    /// the removed element, if any, in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate, out T removed);

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
    /// <param name="removed"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate, out List<T> removed);

    /// <summary>
    /// Clears this collection.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}