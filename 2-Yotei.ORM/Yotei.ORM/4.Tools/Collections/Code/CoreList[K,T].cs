namespace Yotei.ORM.Tools;

// ========================================================
/// <inheritdoc cref="ICoreList{K, T}"/>
[Cloneable(ReturnType = typeof(ICoreList<,>))]
[DebuggerDisplay("{ToDebugString(5)}")]
public abstract partial class CoreList<K, T> : ICoreList<K, T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => throw null;

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    public CoreList(int capacity) => throw null;

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<K, T> source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => throw null;

    /// <summary>
    /// Returns a debug string for this instance with at most the given number of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int count) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before adding or inserting it into this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract T ValidateItem(T item);

    /// <summary>
    /// Invoked to obtain the key by which the given element is known.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract K GetKey(T item);

    /// <summary>
    /// Invoked to validate the given key before using it in this collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract K ValidateKey(K key);

    /// <summary>
    /// Determines if elements that are themselves collections of elements shall be expanded, and
    /// those elements used instead of the original one, or not.
    /// </summary>
    public abstract bool ExpandItems { get; }

    /// <summary>
    /// Invoked to determine if the given '<paramref name="item"/>', that is a duplicate of an
    /// existing '<paramref name="source"/>' element, can be added or inserted into this collection
    /// or not. This method shall:
    /// <br/>- Return <c>true</c> to include the duplicated element.
    /// <br/>- Return <c>false</c> to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool IsValidDuplicate(T source, T item);

    /// <summary>
    /// The comparer used to determine the equality of two given keys.
    /// </summary>
    public abstract IEqualityComparer<K> Comparer { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the indexes of all the elements that carry duplicated keys.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected virtual List<int> FindDuplicates(K key) => IndexesOf(key);

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered the same or not.
    /// <br/> This method is used to determine if an existing element shall be replaced by the
    /// given item, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool SameItem(T source, T item) => typeof(T).IsValueType
        ? Comparer.Equals(GetKey(source), GetKey(item))
        : ReferenceEquals(source, item);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count { get => throw null; }

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
    public bool Contains(K key) => throw null;
    bool ICollection<T>.Contains(T item) => throw null;
    bool IList.Contains(object? value) => throw null;

    /// <inheritdoc/>
    public int IndexOf(K key) => throw null;
    int IList<T>.IndexOf(T item) => throw null;
    int IList.IndexOf(object? value) => throw null;

    /// <inheritdoc/>
    public int LastIndexOf(K key) => throw null;

    /// <inheritdoc/>
    public List<int> IndexesOf(K key) => throw null;

    /// <inheritdoc/>
    public bool Contains(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public int IndexOf(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public T[] ToArray() => throw null;

    /// <inheritdoc/>
    public List<T> ToList() => throw null;

    /// <inheritdoc/>
    public List<T> ToList(int index, int count) => throw null;

    /// <inheritdoc/>
    public int Capacity
    {
        get => throw null;
        set => throw null;
    }

    /// <inheritdoc/>
    public void Trim() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Replace(int index, T item) => throw null;

    /// <inheritdoc/>
    public virtual int Add(T item) => throw null;
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <inheritdoc/>
    public virtual int AddRange(IEnumerable<T> range) => throw null;

    /// <inheritdoc/>
    public virtual int Insert(int index, T item) => throw null;
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <inheritdoc/>
    public virtual int InsertRange(int index, IEnumerable<T> range) => throw null;

    /// <inheritdoc/>
    public virtual int RemoveAt(int index) => throw null;
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual int RemoveRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public virtual int Remove(K key) => throw null;
    bool ICollection<T>.Remove(T item) => throw null;
    void IList.Remove(object? value) => throw null;

    /// <inheritdoc/>
    public virtual int RemoveLast(K key) => throw null;

    /// <inheritdoc/>
    public virtual int RemoveAll(K key) => throw null;

    /// <inheritdoc/>
    public virtual int Remove(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public virtual int RemoveLast(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public virtual int RemoveAll(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public virtual int Clear() => throw null;
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => throw null;
    void ICollection<T>.CopyTo(T[] array, int index) => throw null;
    void ICollection.CopyTo(Array array, int index) => throw null;
}