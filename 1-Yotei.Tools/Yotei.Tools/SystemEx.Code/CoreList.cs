namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{IItem, IKey}"/>
/// </summary>
/// <typeparam name="IItem"></typeparam>
/// <typeparam name="IKey"></typeparam>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class CoreList<IItem, IKey> : ICoreList<IItem, IKey>
{
    readonly List<IItem> Items = new(0);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(IItem item) => Add(item);

    /// <summary>
    /// Initializes a new instance with elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<IItem> range) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<IItem, IKey> source)
    {
        source.ThrowWhenNull();
        AddRange(source);
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public abstract CoreList<IItem, IKey> Clone();
    ICoreList<IItem, IKey> ICoreList<IItem, IKey>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    string ToDebugString()
    {
        return Items.Count < DEBUGCOUNT
            ? $"[{string.Join(", ", Items)}]"
            : $"[{string.Join(", ", Items.Take(DEBUGCOUNT))}, ...]";
    }
    static int DEBUGCOUNT = 8;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract IItem ValidateItem(IItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract IKey GetKey(IItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract IKey ValidateKey(IKey key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public abstract bool CompareKeys(IKey inner, IKey other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool AcceptDuplicated(IItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool ExpandNested(IItem item);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IItem this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (IItem)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(IKey key) => IndexOf(key) >= 0;
    bool ICollection<IItem>.Contains(IItem item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((IItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(IKey key)
    {
        key = ValidateKey(key);

        for (int i = 0; i < Items.Count; i++)
        {
            var same = CompareKeys(GetKey(Items[i]), key);
            if (same) return i;
        }
        return -1;
    }
    int IList<IItem>.IndexOf(IItem item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((IItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(IKey key)
    {
        key = ValidateKey(key);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = CompareKeys(GetKey(Items[i]), key);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IKey key)
    {
        key = ValidateKey(key);

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = CompareKeys(GetKey(Items[i]), key);
            if (same) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++)
        {
            var same = predicate(Items[i]);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<IItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = predicate(Items[i]);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<IItem> predicate)
    {
        predicate.ThrowWhenNull();

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = predicate(Items[i]);
            if (same) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IItem> ToList() => new(Items);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<IItem> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, IItem item)
    {
        item = ValidateItem(item);

        var nested = item is IEnumerable<IItem> && ExpandNested(item);
        if (!nested)
        {
            var temp = Items[index];
            if ((temp is null && item is null) ||
                (temp is not null && item is not null && temp.Equals(item))) return 0;
        }

        RemoveAt(index);
        return Insert(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(IItem item)
    {
        if (item is IEnumerable<IItem> range && ExpandNested(item)) return AddRange(range);

        item = ValidateItem(item);

        var num = IndexOf(GetKey(item));
        if (num >= 0 && !AcceptDuplicated(item)) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<IItem>.Add(IItem item) => Add(item);
    int IList.Add(object? value)
    {
        var num = Add((IItem)value!);
        return num > 0 ? Count : -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<IItem> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range) count += Add(item);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, IItem item)
    {
        if (item is IEnumerable<IItem> range && ExpandNested(item)) return InsertRange(index, range);

        item = ValidateItem(item);

        var num = IndexOf(GetKey(item));
        if (num >= 0 && !AcceptDuplicated(item)) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<IItem>.Insert(int index, IItem item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (IItem)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<IItem> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range)
        {
            var num = Insert(index, item);
            count += num;
            index += num;
        }
        return count;
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
    void IList<IItem>.RemoveAt(int index) => RemoveAt(index);
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
    public virtual int Remove(IKey key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<IItem>.Remove(IItem item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? value) => Remove(GetKey((IItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveLast(IKey key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveAll(IKey key)
    {
        var count = 0; while (true)
        {
            var index = IndexOf(key);

            if (index >= 0) count += RemoveAt(index);
            else break;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<IItem> predicate)
    {
        predicate.ThrowWhenNull();

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<IItem> predicate)
    {
        predicate.ThrowWhenNull();

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<IItem> predicate)
    {
        predicate.ThrowWhenNull();

        var count = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) count += RemoveAt(index);
            else break;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var count = Items.Count; if (count > 0) Items.Clear();
        return count;
    }
    void ICollection<IItem>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;

    bool ICollection<IItem>.IsReadOnly => false;
    void ICollection<IItem>.CopyTo(IItem[] array, int arrayIndex) => ((ICollection<IItem>)Items).CopyTo(array, arrayIndex);

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    void ICollection.CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj);
    public override int GetHashCode() => Items.GetHashCode();
}