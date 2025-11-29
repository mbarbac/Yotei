namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{K, T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreList<,>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class CoreList<K, T> : ICoreList<K, T>
{
    /// <summary>
    /// Internal repository.
    /// </summary>
    class Repo : CoreList<T>
    {
        readonly CoreList<K, T> Master;
        public Repo(CoreList<K, T> master)
        {
            Master = master;

            GetItemDuplicates = x => Master.FindAll(
                y => Master.CompareKeys(Master.GetKey(x), Master.GetKey(y)),
                out var items)
                ? items
                : [];
        }
        new public Func<T, T> ValidateItem => Master.ValidateItem;
        new public bool FlattenElements => Master.FlattenElements;
        new public Func<T, T, bool> CompareItems => Master.CompareItems;
        new public Func<T, T, bool> IncludeDuplicate => Master.IncludeDuplicate;

        protected override string ToDebugItem(T item) => Master.ToDebugItem(item);
    }
    readonly Repo Items;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="getkey"></param>
    public CoreList(Func<T, K> getkey)
    {
        ValidateItem = static x => x;
        CompareItems = (x, y) => x.EqualsEx(y);
        GetKey = getkey;
        ValidateKey = static x => x;
        CompareKeys = static (x, y) => EqualityComparer<K>.Default.Equals(x, y);
        GetKeyDuplicates = x => FindAll(y => CompareKeys(x, GetKey(y)), out var items) ? items : [];
        IncludeDuplicate = static (_, _) => true;
        Items = new(this);
    }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="getkey"></param>
    /// <param name="range"></param>
    public CoreList(
        Func<T, K> getkey, IEnumerable<T> range) : this(getkey) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<K, T> source)
    {
        source.ThrowWhenNull();

        ValidateItem = source.ValidateItem;
        CompareItems = source.CompareItems;
        GetKey = source.GetKey;
        ValidateKey = source.ValidateKey;
        CompareKeys = source.CompareKeys;
        GetKeyDuplicates = source.GetKeyDuplicates;
        IncludeDuplicate = source.IncludeDuplicate;

        Items = new(this);
        Items.AddRange(source);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes with at most
    /// the requested number of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int count) => Items.ToDebugString(count);

    /// <summary>
    /// Invoked to obtain a string representation of the given element suitable for debug
    /// purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T> ValidateItem
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> CompareItems
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, K> GetKey
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<K, K> ValidateKey
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<K, K, bool> CompareKeys
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool FlattenElements
    {
        get;
        set
        {
            if (field == value) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, IEnumerable<T>> GetItemDuplicates => Items.GetItemDuplicates;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<K, IEnumerable<T>> GetKeyDuplicates
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> IncludeDuplicate
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => Items.Contains(item);
    bool IList.Contains(object? item) => Contains((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(K key) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item) => Items.IndexOf(item);
    int IList.IndexOf(object? item) => IndexOf((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item) => Items.IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOfKey(K key) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOfKey(K key) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOfKey(K key) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(
        Predicate<T> predicate, Action<T>? found = null) => Items.Find(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, out T found) => Items.Find(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindLast(
        Predicate<T> predicate, Action<T>? found = null) => Items.FindLast(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindLast(
        Predicate<T> predicate, out T found) => Items.FindLast(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, Action<T>? found = null) => Items.FindAll(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, out List<T> found) => Items.FindAll(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(T[] array, int index) => Items.CopyTo(array, index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index) => Items.CopyTo(array, index);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item) => throw null;
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item) => throw null;
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? item) => Insert(index, (T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item, out T removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual bool RemoveAt(int index, Action<T>? removed = null) => throw null;
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual bool RemoveAt(int index, out T removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count, out List<T> removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(T item, Action<T>? removed = null) => throw null;
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;
    void IList.Remove(object? item) => Remove((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(T item, out List<T> removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item, out List<T> removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, out List<T> removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveKey(K key, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveKey(K key, out T removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLastKey(K key, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLastKey(K key, out T removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAllKey(K key, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAllKey(K key, out List<T> removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate, out T removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate, out T removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate, Action<T>? removed = null) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate, out List<T> removed) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear() => throw null;
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;

    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
}