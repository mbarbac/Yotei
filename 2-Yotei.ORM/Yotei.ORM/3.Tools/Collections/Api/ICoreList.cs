namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements.
/// <br/> Each instance define its own behavior related to the equality comparison of elements,
/// to whether null (for nullable types) and duplicated values are allowed or not, if elements
/// that are themselves collections of elements shall be flattened before using them, as well as
/// any other rules or behaviors appropriate for their context.
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
    /// <br/> Values may be intercepted as determined by this the rules in this instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains at least one ocurrence of the given value, as
    /// determined by the rules in this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value, as determined by the rules
    /// in this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(T item);

    /// <summary>
    /// Returns the index of the last ocurrence of the given value, as determined by the rules
    /// in this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value, as determined by the rules in
    /// this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> AllIndexesOf(T item);

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
    List<int> AllIndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate and,
    /// if so, returns it in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, out T item);

    /// <summary>
    /// Tries to find the last ocurrence of an element that matches the given predicate and,
    /// if so, returns it in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool FindLast(Predicate<T> predicate, out T item);

    /// <summary>
    /// Tries to find all the ocurrences of elements that match the given predicate and returns
    /// them in the out argument.
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
    /// Replaces the element at the given index with the new given value. If a remove delegate is
    /// provided, it is invoked with each of the removed elements, if any. If the given value is
    /// itself an empty enumeration, and if this instance flattens the input values, then no
    /// changes are made.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int Replace(int index, T item, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection the element at the given index. If a remove delegate is
    /// provided, it is invoked with each of the removed elements, if any.
    /// <br/> Returns whether changes have been made or not.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    bool RemoveAt(int index, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// If a remove delegate is provided, it is invoked with each of the removed elements, if
    /// any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given value, as determined by the
    /// rules in this instance. If a remove delegate is provided, it is invoked with each of the
    /// removed elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int Remove(T item, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given value, as determined by the
    /// rules in this instance. If a remove delegate is provided, it is invoked with each of the
    /// removed elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int RemoveLast(T item, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given value, as determined by the
    /// rules in this instance. If a remove delegate is provided, it is invoked with each of the
    /// removed elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int RemoveAll(T item, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate. If a remove delegate is provided, it is invoked with each of the removed
    /// elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate. If a remove delegate is provided, it is invoked with each of the removed
    /// elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate, Action<T>? onremoved = null);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that matches the given
    /// predicate. If a remove delegate is provided, it is invoked with each of the removed
    /// elements, if any.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate, Action<T>? onremoved = null);

    /// <summary>
    /// Clears this collection.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}