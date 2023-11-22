namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{K, T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToStringEx(7)}")]
public class CoreList<K, T> : ICoreList<K, T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    public virtual string ToStringEx(int count)
    {
        var items = count <= Items.Count ? Items : Items.Take(count);
        var temp = string.Join(", ", items.Select(x => ItemToString(x)));

        return $"({Count})[{temp}]";
    }

    public virtual string ItemToString(T item) => item?.ToString() ?? string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, bool, T> ValidateItem
    {
        get => _ValidateItem;
        set
        {
            if (ReferenceEquals(_ValidateItem, value)) return;
            _ValidateItem = value.ThrowWhenNull();

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, bool, T> _ValidateItem = (item, add) => item;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, K> GetKey
    {
        get => _GetKey;
        set
        {
            if (ReferenceEquals(_GetKey, value)) return;
            _GetKey = value.ThrowWhenNull();

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, K> _GetKey = (item) => item.ConvertTo<K>()!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<K, K> ValidateKey
    {
        get => _ValidateKey;
        set
        {
            if (ReferenceEquals(_ValidateKey, value)) return;
            _ValidateKey = value.ThrowWhenNull();

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<K, K> _ValidateKey = (item) => item;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<K, K, bool> Compare
    {
        get => _Compare;
        set
        {
            if (ReferenceEquals(_Compare, value)) return;
            _Compare = value.ThrowWhenNull();

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<K, K, bool> _Compare = (source, target)
        => (source is null && target is null)
        || (typeof(K).IsValueType ? source!.Equals(target) : ReferenceEquals(source, target));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> IsSame
    {
        get => _IsSame;
        set
        {
            if (ReferenceEquals(_IsSame, value)) return;
            _IsSame = value.ThrowWhenNull();

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, T, bool> _IsSame = (source, target)
        => (source is null && target is null)
        || (typeof(T).IsValueType ? source!.Equals(target) : ReferenceEquals(source, target));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> ValidDuplicate
    {
        get => _ValidDuplicate;
        set
        {
            if (ReferenceEquals(_ValidDuplicate, value)) return;
            _ValidDuplicate = value.ThrowWhenNull();

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, T, bool> _ValidDuplicate = (source, target) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

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
    /// <param name="predicate"></param>
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
        return IndexOf(x => Compare(GetKey(x), key));
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
        return LastIndexOf(x => Compare(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(K key)
    {
        key = ValidateKey(key);
        return IndexesOf(x => Compare(GetKey(x), key));
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
        predicate.ThrowWhenNull();

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
        predicate.ThrowWhenNull();

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
        predicate.ThrowWhenNull();

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
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
    public List<T> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = ValidateItem(item, true);

        var source = Items[index];
        if (IsSame(source, item)) return 0;

        var key = GetKey(item);
        var nums = IndexesOf(key);
        foreach (var num in nums)
        {
            if (num == index) continue;
            if (!ValidDuplicate(Items[num], item)) return 0;
        }

        Items[index] = item;
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        item = ValidateItem(item, true);

        var key = GetKey(item);
        var nums = IndexesOf(key);
        foreach (var num in nums) if (!ValidDuplicate(Items[num], item)) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value)
    {
        var num = Add((T)value!);
        return num > 0 ? Count : -1;
    }

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
        item = ValidateItem(item, true);

        var key = GetKey(item);
        var nums = IndexesOf(key);
        foreach (var num in nums) if (!ValidDuplicate(Items[num], item)) return 0;

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
    bool ICollection<T>.Remove(T item) => Remove(GetKey(item)) >= 0;
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

    bool ICollection<T>.IsReadOnly => false;
    void ICollection<T>.CopyTo(
        T[] array, int arrayIndex) => ((ICollection<T>)Items).CopyTo(array, arrayIndex);

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    void ICollection.CopyTo(
        Array array, int index) => ((ICollection)Items).CopyTo(array, index);
}