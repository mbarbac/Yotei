namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICustomList{K, T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(6)}")]
public class CustomList<K, T> : ICustomList<K, T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CustomList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"Count: {Count}";

    protected virtual string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ItemToString))}]...";

    protected virtual string ItemToString(T item) => item?.ToString() ?? string.Empty;

    // ---------------------------------------------------

    readonly List<T> Items = [];

    /// <summary>
    /// Invoked to validate the given element before using it for adding or inserting purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual T ValidateItem(T item) => OnValidateItem(item);
    public Func<T, T> OnValidateItem
    {
        get => _OnValidateItem;
        set
        {
            value.ThrowWhenNull();

            if (_OnValidateItem == value) return;
            _OnValidateItem = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, T> _OnValidateItem = (item) => item;

    /// <summary>
    /// Invoked to obtain the key associated with the given item, which might not be a valid one.
    /// <br/> Override this method or the associated delegate before using this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual K GetKey(T item) => OnGetKey(item);
    public Func<T, K> OnGetKey
    {
        get => _OnGetKey;
        set
        {
            value.ThrowWhenNull();

            if (_OnGetKey == value) return;
            _OnGetKey = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, K> _OnGetKey = (item) => throw new NotImplementedException();

    /// <summary>
    /// Invoked to validate the given key before using it for comparison purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual K ValidateKey(K key) => OnValidateKey(key);
    public Func<K, K> OnValidateKey
    {
        get => _OnValidateKey;
        set
        {
            value.ThrowWhenNull();

            if (_OnValidateKey == value) return;
            _OnValidateKey = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<K, K> _OnValidateKey = (key) => key;

    /// <summary>
    /// Invoked to determine if the two given keys shall be considered equal, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareKeys(K source, K target) => OnCompareKeys(source, target);
    public Func<K, K, bool> OnCompareKeys
    {
        get => _OnCompareKeys;
        set
        {
            value.ThrowWhenNull();

            if (_OnCompareKeys == value) return;
            _OnCompareKeys = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<K, K, bool> _OnCompareKeys = (source, target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered the same, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool SameElement(T source, T target) => OnSameElement(source, target);
    public Func<T, T, bool> OnSameElement
    {
        get => _OnSameElement;
        set
        {
            value.ThrowWhenNull();

            if (_OnSameElement == value) return;
            _OnSameElement = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, T, bool> _OnSameElement = (source, target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// Invoked to determine if the given duplicate or the existing one can be accepted, or if
    /// it just shall be ignored. In addition, an exception can be thrown if duplicates are not
    /// accepted.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool AcceptDuplicate(T source, T target) => OnAcceptDuplicate(source, target);
    public Func<T, T, bool> OnAcceptDuplicate
    {
        get => _OnAcceptDuplicate;
        set
        {
            value.ThrowWhenNull();

            if (_OnAcceptDuplicate == value) return;
            _OnAcceptDuplicate = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, T, bool> _OnAcceptDuplicate = (source, target) => true;

    /// <summary>
    /// Invoked to obtain the indexes of the elements whose keys can be considered duplicates of
    /// the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual List<int> FindDuplicates(K key) => OnFindDuplicates(this, key);
    public Func<CustomList<K, T>, K, List<int>> OnFindDuplicates
    {
        get => _OnFindDuplicates;
        set
        {
            value.ThrowWhenNull();

            if (_OnFindDuplicates == value) return;
            _OnFindDuplicates = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<CustomList<K, T>, K, List<int>> _OnFindDuplicates =
        (master, key) => master.IndexesOf(key);

    /// <summary>
    /// Invoked to reload the contents of this instance.
    /// </summary>
    protected void ReLoad()
    {
        var range = ToArray();
        Clear();
        AddRange(range);
    }

    // ---------------------------------------------------

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
        set => Replace(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(K key) => IndexOf(key) >= 0;
    bool ICollection<T>.Contains(T item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((T)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(K key)
    {
        key = ValidateKey(key);
        return IndexOf(x => CompareKeys(GetKey(x), key));
    }
    int IList<T>.IndexOf(T item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((T)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(K key)
    {
        key = ValidateKey(key);
        return LastIndexOf(x => CompareKeys(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(K key)
    {
        key = ValidateKey(key);
        return IndexesOf(x => CompareKeys(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="comparer"></param>
    public void Sort(IComparer<T> comparer)
        => Items.Sort(comparer ?? throw new ArgumentNullException(nameof(comparer)));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Reverse() => Items.Reverse();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if (SameElement(source, item)) return 0;

        RemoveAt(index);
        return Insert(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = FindDuplicates(key);
        var valid = true;

        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? Count : -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = FindDuplicates(key);
        var valid = true;

        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int Remove(K key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<T>.Remove(T item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? value) => Remove(GetKey((T)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveLast(K key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveAll(K key)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(key);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    object ICollection.SyncRoot => Items;
    bool ICollection.IsSynchronized => false;
    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    bool ICollection<T>.IsReadOnly => false;
    void ICollection<T>.CopyTo(T[] array, int index) => Items.CopyTo(array, index);
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((T[])array, index);
}