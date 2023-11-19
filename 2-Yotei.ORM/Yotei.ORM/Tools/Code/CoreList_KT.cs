namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{TKey, TItem}"/>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
[DebuggerDisplay("{ToString(7)}")]
public abstract class CoreList<TKey, TItem> : ICoreList<TKey, TItem>
{
    readonly List<TItem> Items = [];

    /// <summary>
    /// Initializes a new empty list.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(TItem item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<TItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a string representation of this collection containing at most the given number
    /// of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected string ToString(int count)
    {
        var items = count <= Items.Count ? Items : Items.Take(count);
        var temp = string.Join(", ", items);
        return $"({Count})[{temp}]";
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TItem ValidateItem(TItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool CanDuplicate(TItem source, TItem target);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TKey GetKey(TItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TKey ValidateKey(TKey key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool Compare(TKey source, TKey target);

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
    public TItem this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (TItem)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TKey key) => IndexOf(ValidateKey(key)) >= 0;
    bool ICollection<TItem>.Contains(TItem item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(TKey key) => IndexOf(x => Compare(GetKey(x), ValidateKey(key)));
    int IList<TItem>.IndexOf(TItem item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(TKey key) => LastIndexOf(x => Compare(GetKey(x), ValidateKey(key)));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TKey key) => IndexesOf(x => Compare(GetKey(x), ValidateKey(key)));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate)
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
    public int LastIndexOf(Predicate<TItem> predicate)
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
    public List<int> IndexesOf(Predicate<TItem> predicate)
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
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<TItem> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, TItem item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if ((source is null && item is null) ||
            (source is not null && source.Equals(item))) return 0;

        var nums = IndexesOf(GetKey(item)); foreach (var num in nums)
        {
            if (num == index) continue;
            if (!CanDuplicate(Items[num], item)) return 0;
        }

        Items[index] = item;
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(TItem item)
    {
        item = ValidateItem(item);

        var nums = IndexesOf(GetKey(item));
        foreach (var num in nums) if (!CanDuplicate(Items[num], item)) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<TItem>.Add(TItem item) => Add(item);
    int IList.Add(object? value)
    {
        var num = Add((TItem)value!);
        return num > 0 ? Count : -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<TItem> range)
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
    public virtual int Insert(int index, TItem item)
    {
        item = ValidateItem(item);

        var nums = IndexesOf(GetKey(item));
        foreach (var num in nums) if (!CanDuplicate(Items[num], item)) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<TItem>.Insert(int index, TItem item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (TItem)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<TItem> range)
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
    void IList<TItem>.RemoveAt(int index) => RemoveAt(index);
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
    public virtual int Remove(TKey key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<TItem>.Remove(TItem item) => Remove(GetKey(item)) >= 0;
    void IList.Remove(object? value) => Remove(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveLast(TKey key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveAll(TKey key)
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
    public virtual int Remove(Predicate<TItem> predicate)
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
    public virtual int RemoveLast(Predicate<TItem> predicate)
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
    public virtual int RemoveAll(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

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
    void ICollection<TItem>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<TItem>.IsReadOnly => false;
    void ICollection<TItem>.CopyTo(
        TItem[] array, int arrayIndex) => ((ICollection<TItem>)Items).CopyTo(array, arrayIndex);

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    void ICollection.CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);
}