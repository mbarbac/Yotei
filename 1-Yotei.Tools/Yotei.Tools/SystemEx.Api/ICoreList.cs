namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICoreList<T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>, ICloneable
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    new ICoreList<T> Clone();

    /// <summary>
    /// The (item, add) delegate invoked to determine if a given element is valid for the given
    /// scenario, it being 'true' if the element is to be added or inserted into this collection,
    /// or 'false' otherwise. By default this delegate just returns the item under consideration.
    /// Inheritors are expected to throw and appropriate exception if needed.
    /// </summary>
    Func<T, bool, T> Validate { get; set; }

    /// <summary>
    /// The (inner, other) delegate invoked to determine if any (inner) existing element in this
    /// collection shall be considered equivalent to the (other) given one. Many methods is this
    /// class have the ability of overriding this comparison by selecting an 'strict' mode. By
    /// default this delegate just invokes the default comparer for the type of the elements in
    /// this collection.
    /// </summary>
    Func<T, T, bool> Compare { get; set; }

    /// <summary>
    /// The (item) delegate invoked to determine if the given duplicated element can added or
    /// inserted into this collection, or not. By default this delegate just returns 'true' to
    /// add or insert duplicated elements. Inheritors are expected to return the appropriate
    /// value, or to throw and appropriate exception if needed.
    /// </summary>
    Func<T, bool> AcceptDuplicate { get; set; }

    /// <summary>
    /// The (item) delegate invoked to determine if the given element, which is by itself an
    /// enumeration of elements, shall be expanded and its own elements added or inserted into
    /// this collection, instead of the original one, or not. By default this delegate returns
    /// 'false' meaning that those nested elements are not flattened. Inheritors are expected to
    /// return the appropriate value, or to throw and appropriate exception if needed.
    /// </summary>
    Func<T, bool> ExpandNested { get; set; }

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Minimizes the memory footprint of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// <br/> The setter performs an 'strict' replacement at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// <br/> If 'strict' mode is requested, comparison is made by value or reference, instead of
    /// using the <see cref="Compare"/> delegate.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(T item, bool strict = false);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or -1
    /// if it cannot be found.
    /// <br/> If 'strict' mode is requested, comparison is made by value or reference, instead of
    /// using the '<see cref="Compare"/>' delegate.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int IndexOf(T item, bool strict = false);

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or -1
    /// if it cannot be found.
    /// <br/> If 'strict' mode is requested, comparison is made by value or reference, instead of
    /// using the '<see cref="Compare"/>' delegate.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int LastIndexOf(T item, bool strict = false);

    /// <summary>
    /// Returns a list with the indexes of the ocurrences of the given element in this collection.
    /// <br/> If 'strict' mode is requested, comparison is made by value or reference, instead of
    /// using the '<see cref="Compare"/>' delegate.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item, bool strict = false);

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate, or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns a list containing the indexes of all the elements in this collection that match
    /// the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

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
    /// Returns a list with the given number of elements starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> GetRange(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element stored at the given index with the new given one.
    /// If they are the same then no replacement is performed.
    /// If 'strict' mode is requested, comparison is made by value or reference, instead of using
    /// the '<see cref="Compare"/>' delegate.
    /// Returns the number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int Replace(int index, T item, bool strict = false);

    /// <summary>
    /// Adds to this collection the given element.
    /// If it is a duplicated one, then this method invokes the '<see cref="AcceptDuplicate"/>
    /// delegate to determine the appropriate behavior.
    /// Returns the number of elements added.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// If any is a duplicated one, then this method invokes the '<see cref="AcceptDuplicate"/>
    /// delegate to determine the appropriate behavior.
    /// Returns the number of elements added.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts into this collection the given element, at the given index.
    /// If it is a duplicated one, then this method invokes the '<see cref="AcceptDuplicate"/>
    /// delegate to determine the appropriate behavior.
    /// Returns the number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting from the given
    /// index.
    /// If any is a duplicated one, then this method invokes the '<see cref="AcceptDuplicate"/>
    /// delegate to determine the appropriate behavior.
    /// Returns the number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting from the given index.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element.
    /// If 'strict' mode is requested, comparison is made by value or reference, instead of using
    /// the '<see cref="Compare"/>' delegate.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int Remove(T item, bool strict = false);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element.
    /// If 'strict' mode is requested, comparison is made by value or reference, instead of using
    /// the '<see cref="Compare"/>' delegate.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int RemoveLast(T item, bool strict = false);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element.
    /// If 'strict' mode is requested, comparison is made by value or reference, instead of using
    /// the '<see cref="Compare"/>' delegate.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    int RemoveAll(T item, bool strict = false);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Clears this collection.
    /// Returns the number of elements removed.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}