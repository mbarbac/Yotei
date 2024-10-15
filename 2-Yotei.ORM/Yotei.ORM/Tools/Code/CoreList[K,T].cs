namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc cref="ICoreList{K, T}"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public abstract partial class CoreList<K, T> : ICoreList<K, T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public CoreList(int capacity) => Items = new(capacity);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<K, T> source) : this() => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    // <summary>
    /// Invoked to produce a debug string.
    /// </summary>
    protected virtual string ToDebugString(int count)
    {
        if (Count == 0) return $"0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(ItemToDebug))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToDebug))}, ...]";
    }

    /// <summary>
    /// Invoked to obtain a debug string of the given item.
    /// </summary>
    protected virtual string ItemToDebug(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given item before using it in this collection.
    /// </summary>
    public Func<T, T> ValidateItem
    {
        get => _ValidateItem;
        init
        {
            if (_ValidateItem != value.ThrowWhenNull())
            {
                _ValidateItem = value;
                Reload();
            }
        }
    }
    Func<T, T> _ValidateItem = (item) => item;

    /// <summary>
    /// Invoked to obtain the key by which the given item will be known.
    /// <br/> The value of this delegate property MUST be set before using this collection.
    /// </summary>
    public Func<T, K> GetKey
    {
        get => _GetKey;
        init
        {
            if (_GetKey != value.ThrowWhenNull())
            {
                _GetKey = value;
                Reload();
            }
        }
    }
    Func<T, K> _GetKey = (item) => throw new NotImplementedException();

    /// <summary>
    /// The comparer to use to determine equality of two given keys.
    /// </summary>
    public IEqualityComparer<K> Comparer
    {
        get => _Comparer;
        init
        {
            if (_Comparer != value.ThrowWhenNull())
            {
                _Comparer = value;
                Reload();
            }
        }
    }
    IEqualityComparer<K> _Comparer = EqualityComparer<K>.Default;

    /// <summary>
    /// Reloads the contents of this instance after a change in a init property.
    /// </summary>
    void Reload()
    {
        if (Count != 0)
        {
            var range = new List<T>(Items);
            Items.Clear();
            AddRange(range);
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public T this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <inheritdoc/>
    public bool Contains(K key) => IndexOf(key) >= 0;
    bool ICollection<T>.Contains(T item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((T)value!));

    /// <inheritdoc/>
    public int IndexOf(K key) => IndexOf(x => Comparer.Equals(key, GetKey(x)));
    int IList<T>.IndexOf(T item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((T)value!));

    /// <inheritdoc/>
    public int LastIndexOf(K key) => LastIndexOf(x => Comparer.Equals(key, GetKey(x)));

    /// <inheritdoc/>
    public List<int> IndexesOf(K key) => IndexesOf(x => Comparer.Equals(key, GetKey(x)));

    /// <inheritdoc/>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <inheritdoc/>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    /// <inheritdoc/>
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <inheritdoc/>
    public void Trim() => Items.TrimExcess();

    /// <inheritdoc/>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Replace(int index, T item) => throw null;

    /// <inheritdoc/>
    public int Add(T item) => throw null;
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <inheritdoc/>
    public int AddRange(IEnumerable<T> range) => throw null;

    /// <inheritdoc/>
    public int Insert(int index, T item) => throw null;
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <inheritdoc/>
    public int InsertRange(int index, IEnumerable<T> range) => throw null;

    /// <inheritdoc/>
    public int RemoveAt(int index) => throw null;
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public int RemoveRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public int Remove(K key) => throw null;
    bool ICollection<T>.Remove(T item) => throw null;
    void IList.Remove(object? value) => throw null;

    /// <inheritdoc/>
    public int RemoveLast(K key) => throw null;

    /// <inheritdoc/>
    public int RemoveAll(K key) => throw null;

    /// <inheritdoc/>
    public int Remove(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public int RemoveLast(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public int RemoveAll(Predicate<T> predicate) => throw null;

    /// <inheritdoc/>
    public int Clear() => throw null;
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;

    void ICollection<T>.CopyTo(T[] array, int index) => Items.CopyTo(array, index);
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((T[])array, index);
}