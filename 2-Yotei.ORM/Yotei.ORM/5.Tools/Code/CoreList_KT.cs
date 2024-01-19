namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc cref="ICoreList{TKey, TItem}"/>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class CoreList<TKey, TItem> : ICoreList<TKey, TItem>
{
    readonly List<TItem> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(TItem item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<TItem> range) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<TKey, TItem> source) => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count) => Count == 0 ? "0:[]" : (
        Count < count
        ? $"{Count}:[{string.Join(", ", this.Select(ItemToString))}]"
        : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToString))}, ...]");

    protected virtual string ItemToString(TItem item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <summary>
    /// Validates the given element before using it in this collection.
    /// </summary>
    protected virtual TItem ValidateItem(TItem item) => item;

    /// <summary>
    /// Obtains the key associated with the given element.
    /// </summary>
    protected virtual TKey GetKey(TItem item) => throw new NotImplementedException();

    /// <summary>
    /// Validates the given key before using it in this collection.
    /// </summary>
    protected virtual TKey ValidateKey(TKey key) => key;

    /// <summary>
    /// Invoked to compare the given source key with the other given one.
    /// </summary>
    protected virtual bool CompareKeys(TKey source, TKey item) =>
        (source is null && item is null) ||
        (source is not null && source.Equals(item));

    /// <summary>
    /// Determines if the given source element is the same as the other given one.
    /// </summary>
    protected virtual bool SameItem(TItem source, TItem item) =>
        (source is null && item is null) ||
        (source is not null && source.Equals(item));

    /// <summary>
    /// Obtains the indexes of the source elements whose key is duplicated of the given one.
    /// </summary>
    protected virtual List<int> GetDuplicates(TKey key) => IndexesOf(key);

    /// <summary>
    /// Determines if the given duplicated item is added, or ignored, or throws an exception.
    /// </summary>
    protected virtual bool AcceptDuplicate(TItem source, TItem item) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool Contains(TKey key) => IndexOf(key) >= 0;
    bool IList.Contains(object? value) => Contains(GetKey((TItem)value!));
    bool ICollection<TItem>.Contains(TItem item) => Contains(GetKey(item));

    /// <inheritdoc/>
    public int IndexOf(TKey key)
    {
        key = ValidateKey(key);
        return IndexOf(x => CompareKeys(GetKey(x), key));
    }
    int IList<TItem>.IndexOf(TItem item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((TItem)value!));

    /// <inheritdoc/>
    public int LastIndexOf(TKey key)
    {
        key = ValidateKey(key);
        return LastIndexOf(x => CompareKeys(GetKey(x), key));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(TKey key)
    {
        key = ValidateKey(key);
        return IndexesOf(x => CompareKeys(GetKey(x), key));
    }

    /// <inheritdoc/>
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <inheritdoc/>
    public int IndexOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <inheritdoc/>
    public TItem[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<TItem> ToList() => new(Items);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Replace(int index, TItem item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if (SameItem(source, item)) return 0;

        RemoveAt(index);
        return Insert(index, item);
    }

    /// <inheritdoc/>
    public virtual int Add(TItem item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = GetDuplicates(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }
    int IList.Add(object? value) { var num = Add((TItem)value!); return num >= 0 ? Count : -1; }
    void ICollection<TItem>.Add(TItem item) => Add(item);

    /// <inheritdoc/>
    public virtual int AddRange(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection irange && irange.Count == 0) return 0;
        if (range is ICollection<TItem> trange && trange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Insert(int index, TItem item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = GetDuplicates(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<TItem>.Insert(int index, TItem value) => Insert(index, value);
    void IList.Insert(int index, object? value) => Insert(index, (TItem)value!);

    /// <inheritdoc/>
    public virtual int InsertRange(int index, IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection irange && irange.Count == 0) return 0;
        if (range is ICollection<TItem> trange && trange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            index += r;
            num += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<TItem>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(TKey key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    void IList.Remove(object? value) => Remove(GetKey((TItem)value!));
    bool ICollection<TItem>.Remove(TItem item) => Remove(GetKey(item)) > 0;

    /// <inheritdoc/>
    public virtual int RemoveLast(TKey key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual int Remove(Predicate<TItem> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveLast(Predicate<TItem> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(Predicate<TItem> predicate)
    {
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
    void IList.Clear() => Clear();
    void ICollection<TItem>.Clear() => Clear();

    // ----------------------------------------------------

    object ICollection.SyncRoot => Items;
    bool ICollection.IsSynchronized => false;
    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    bool ICollection<TItem>.IsReadOnly => false;
    void ICollection<TItem>.CopyTo(TItem[] array, int index) => Items.CopyTo(array, index);
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((TItem[])array, index);
}