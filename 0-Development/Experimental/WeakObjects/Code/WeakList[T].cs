namespace Experimental.WeakObjects;

// ========================================================
/// <summary>
/// Represents a list-alike collection of weak holders that can be weakened so that they can be
/// garbage collected when not used any longer.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WeakList<T> : IWeakObject, IList<T>, IList, ICollection<T>, ICollection
    where T : class
{
    readonly List<WeakHolder<T>> Holders;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public WeakList() => Holders = [];

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public WeakList(int capacity) => Holders = new(capacity);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public WeakList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// <br/> This method is NOT synchronized.
    /// <br/> This method DOES hydrate the elements returned.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
        var i = 0; while (i < Holders.Count)
        {
            var item = Holders[i];
            var target = item.Target;
            if (target != null) yield return target;

            i++;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        var count = 0;
        var items = 0;

        lock (SyncRoot)
        {
            for (int i = 0; i < Holders.Count; i++)
            {
                var item = Holders[i];
                count++;
                items += item.IsValid ? 1 : 0;
            }

            return $"Count: {count}/{items}";
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool IsValid => throw null;

    /// <inheritdoc/>
    public DateTime LastUsed => throw null;

    /// <inheritdoc/>
    public void Hydrate() => throw null;

    /// <inheritdoc/>
    public void Weaken() => throw null;

    /// <inheritdoc/>
    public void Weaken(TimeSpan timeout) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public object SyncRoot { get; } = new();

    /// <summary>
    /// Gets the number of valid elements in this collection.
    /// <br/> This property IS synchronized.
    /// </summary>
    public int Count { get { lock (SyncRoot) return Holders.Count(x => x.IsValid); } }

    /// <summary>
    /// Gets the total number of holders in this collection, despite of if they are valid or not.
    /// <br/> This property IS synchronized.
    /// </summary>
    public int CountHolders { get { lock (SyncRoot) return Holders.Count; } }

    /// <inheritdoc/>
    public T this[int index]
    {
        get => throw null;
        set => throw null;
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <inheritdoc/>
    public bool Contains(T item) => throw null;
    bool IList.Contains(object? value) => Contains((T)value!);

    /// <inheritdoc/>
    public int IndexOf(T item) => throw null;
    int IList.IndexOf(object? value) => IndexOf((T)value!);

    /// <summary>
    /// Gets the index of the last ocurrence of the given element in this collection, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item) => throw null;

    /// <summary>
    /// Gets the indexes of the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item) => throw null;

    /// <summary>
    /// Determines if this collection contains an element that satisfies the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Gets the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Gets the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Gets the indexes of all the elements in this collection that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Gets an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => throw null;

    /// <summary>
    /// Gets a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => throw null;

    /// <summary>
    /// Returns a list with a shallow copy of the given number of elements, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> ToList(int index, int count) => throw null;

    /// <inheritdoc/>
    public void CopyTo(Array array, int index) => throw null;

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => throw null;

    /// <summary>
    /// Gets or sets the total number of elements the internal data structures can hold without
    /// resizing.
    /// </summary>
    public int Capacity
    {
        get => throw null;
        set => throw null;
    }

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    public void Trim() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before adding or inserting it into this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool Validate(T item) => true;

    /// <summary>
    /// Replaces the element at the given index with the given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public void Replace(int index, T item) => throw null;

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public void Add(T item) => throw null;
    int IList.Add(object? value) => throw null; // Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range) => throw null;

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public void Insert(int index, T item) => throw null;
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<T> range) => throw null;

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public void RemoveAt(int index) => throw null;

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public void RemoveRange(int index, int count) => throw null;

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item) => throw null;
    void IList.Remove(object? value) => Remove((T)value!);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool RemoveLast(T item) => throw null;

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveAll(T item) => throw null;

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Remove(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool RemoveLast(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveAll(Predicate<T> predicate) => throw null;

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns></returns>
    public void Clear() => throw null;

    // ----------------------------------------------------

    bool ICollection.IsSynchronized => false;
    bool IList.IsFixedSize => false;
    bool ICollection<T>.IsReadOnly => false;
    bool IList.IsReadOnly => false;
}