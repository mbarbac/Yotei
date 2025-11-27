namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface ICoreList<T>
    : IList<T>, IReadOnlyList<T>, IList
    , ICollection<T>, IReadOnlyCollection<T> , ICollection
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
    /// determined by the rules of this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element, as determined by the rules
    /// of this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(T item);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element, as determined by the rules
    /// of this instance, or -1 if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Returns the indexes of the ocurrences of the given element in this collection,
    /// as determined by the rules of this instance.
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
    /// Returns the indexes of the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> AllIndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate and,
    /// if so, returns the first one in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, [MaybeNull] out T item);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate and,
    /// if so, returns the last one in the out argument.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool FindLast(Predicate<T> predicate, [MaybeNull] out T item);

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

    /// <summary>
    /// Trims the internal structures used by this instance.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the new given one.
    /// <br/> If the given element is an empty enumeration, and it happens that this instance
    /// flattens the input elements that are collections of the type, then no changes are made.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, T item);

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
    /// Removes from this collection the element at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the element at the given index, and returns it in the out
    /// argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveAt(int index, [MaybeNull] out T item);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index,
    /// and returns the removed ones in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count, out List<T> items);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element, as determined by
    /// the rules of this instance.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Remove(T item);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element, as determined by
    /// the rules of this instance.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveLast(T item);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, as determined by
    /// the rules of this instance.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveAll(T item);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, as determined by
    /// the rules of this instance, and returns the removed elements.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    int RemoveAll(T item, out List<T> items);

    /// <summary>
    /// Removes the first ocurrence of the given element from this collection and returns the
    /// removed one, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate, [MaybeNull] out T item);

    /// <summary>
    /// Removes the last ocurrence of the given element from this collection and returns the
    /// removed one, if any, in the out argument.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate, [MaybeNull] out T item);

    /// <summary>
    /// Removes all the ocurrences of the given element from this collection, and returns the
    /// removed ones in the out argument.
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