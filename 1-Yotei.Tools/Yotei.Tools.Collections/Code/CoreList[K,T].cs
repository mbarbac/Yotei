namespace Yotei.Tools.Collections;

// ========================================================
/// <inheritdoc cref="ICoreList{K, T}"/>
[Cloneable]
[DebuggerDisplay("{ToDebugString(6)}")]
public partial class CoreList<K, T> : ICoreList<K, T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList()
    {
        _ValidateItem = (item) => item;
        _GetKey = (item) => throw new NotImplementedException();
        _ValidateKey = (key) => key;
        _CompareKeys = EqualityComparer<K>.Default.Equals;
        _GetDuplicates = IndexesOf;
        _CanInclude = (item, x) => true;
    }

    /// <summary>
    /// Initializes a new instance with the given element
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) : this() => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
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

    public string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(ItemToDebugString))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToDebugString))}, ...]";
    }

    protected virtual string ItemToDebugString(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <summary>
    /// Reloads the contents of this instance.
    /// </summary>
    protected void Reload()
    {
        if (Items.Count == 0) return;

        var range = Items.ToArray();
        Items.Clear();
        AddRange(range);
    }

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    public Func<T, T> ValidateItem
    {
        get => _ValidateItem;
        set
        {
            if (ReferenceEquals(_ValidateItem, value.ThrowWhenNull())) return;
            _ValidateItem = value;
            Reload();
        }
    }
    Func<T, T> _ValidateItem;

    /// <summary>
    /// Invoked to extract the key associated with the given element.
    /// </summary>
    public Func<T, K> GetKey
    {
        get => _GetKey;
        set
        {
            if (ReferenceEquals(_GetKey, value.ThrowWhenNull())) return;
            _GetKey = value;
            Reload();
        }
    }
    Func<T, K> _GetKey;

    /// <summary>
    /// Invoked to validate the given key before using it in this collection.
    /// </summary>
    public Func<K, K> ValidateKey
    {
        get => _ValidateKey;
        set
        {
            if (ReferenceEquals(_ValidateKey, value.ThrowWhenNull())) return;
            _ValidateKey = value;
            Reload();
        }
    }
    Func<K, K> _ValidateKey;

    /// <summary>
    /// Invoked to determine if the two given keys shall be considered equivalent, or not.
    /// </summary>
    public Func<K, K, bool> CompareKeys
    {
        get => _CompareKeys;
        set
        {
            if (ReferenceEquals(_CompareKeys, value.ThrowWhenNull())) return;
            _CompareKeys = value;
            Reload();
        }
    }
    Func<K, K, bool> _CompareKeys;

    /// <summary>
    /// Invoked to get the indexes of the elements whose keys can be considered equivalent to the
    /// given one.
    /// </summary>
    public Func<K, List<int>> GetDuplicates
    {
        get => _GetDuplicates;
        set
        {
            if (ReferenceEquals(_GetDuplicates, value.ThrowWhenNull())) return;
            _GetDuplicates = value;
            Reload();
        }
    }
    Func<K, List<int>> _GetDuplicates;

    /// <summary>
    /// Invoked to determine if the first given element can be included in this collection, when
    /// compared against the second given one. Returns <c>true</c> is so, or <c>false</c> if the
    /// inclusion operation shall be ignored. It is expected an appropriate exception is thrown
    /// if duplicates are not allowed.
    /// </summary>
    public Func<T, T, bool> CanInclude
    {
        get => _CanInclude;
        set
        {
            if (ReferenceEquals(_CanInclude, value.ThrowWhenNull())) return;
            _CanInclude = value;
            Reload();
        }
    }
    Func<T, T, bool> _CanInclude;

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
    public int IndexOf(K key) => IndexOf(key, validate: true);
    int IndexOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return IndexOf(x => CompareKeys(GetKey(x), key));
    }
    int IList<T>.IndexOf(T item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((T)value!));

    /// <inheritdoc/>
    public int LastIndexOf(K key)
    {
        key = ValidateKey(key);
        return LastIndexOf(x => CompareKeys(GetKey(x), key));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(K key)
    {
        key = ValidateKey(key);
        return IndexesOf(x => CompareKeys(GetKey(x), key));
    }

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

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    // ----------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, bool insert = false)
    {
        if (index < 0) throw new IndexOutOfRangeException("Index is negative.").WithData(index);

        var value = Items.Count + (insert ? 1 : 0);
        if (index >= value) throw new IndexOutOfRangeException("Index greater than or equal the number of elements.").WithData(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, int count, bool insert = false)
    {
        Validate(index, insert);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Items.Count - index);
    }

    protected virtual bool SameItem(T source, T target) => source.EqualsEx(target);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int GetRange(int index, int count)
    {
        Validate(index, count);
        
        if (count == 0 && Count == 0) return 0;
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        var num = Count - range.Count;

        Items.Clear();
        AddRange(range);
        return num;
    }

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        Validate(index);
        item = ValidateItem(item);

        var source = Items[index];
        var same = SameItem(source, item);
        if (same) return 0;

        var done = RemoveAt(index);
        if (done == 0) throw new InvalidOperationException(
            "Cannot remove element at the requested index.")
            .WithData(index)
            .WithData(this);
        
        done = Insert(index, item);
        if (done == 0) Items.Insert(index, source);
        return done;
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        item = ValidateItem(item);

        var prevent = false;
        var key = GetKey(item);
        var dups = GetDuplicates(key);
        foreach (var x in dups) if (!CanInclude(item, Items[x])) prevent = false;
        if (prevent) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <inheritdoc/>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Insert(int index, T item)
    {
        Validate(index, true);
        item = ValidateItem(item);

        var prevent = false;
        var key = GetKey(item);
        var dups = GetDuplicates(key);
        foreach (var x in dups) if (!CanInclude(item, Items[x])) prevent = false;
        if (prevent) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <inheritdoc/>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        Validate(index, true);
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int RemoveAt(int index)
    {
        Validate(index);

        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual int RemoveRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0) return 0;
        if (index == 0 && count == Count) return Clear();

        Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(K key)
    {
        if (Count == 0) return 0;

        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<T>.Remove(T item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? value) => Remove(GetKey((T)value!));

    /// <inheritdoc/>
    public virtual int RemoveLast(K key)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(K key)
    {
        if (Count == 0) return 0;

        key = ValidateKey(key);

        var num = 0; while (true)
        {
            var index = IndexOf(key, validate: false);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Remove(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
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