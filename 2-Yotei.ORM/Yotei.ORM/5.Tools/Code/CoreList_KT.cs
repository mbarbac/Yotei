namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{K, T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial class CoreList<K, T> : ICoreList<K, T>
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
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<K, T> source)
    {
        if (source.Count == 0) return;

        Items.Capacity = source.Count;
        AddRange(source, false);
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
    public override string ToString() => $"Count: {Count}";

    protected virtual string ToDebugString(int num) => Count == 0 ? "0:[]" : (Count <= num
        ? $"[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"[{string.Join(", ", Items.Take(num).Select(ItemToString))}, ...]");

    protected virtual string ItemToString(T item) => item?.ToString() ?? string.Empty;
    protected virtual int DebugCount => 6;

    // ----------------------------------------------------

    /// <summary>
    /// Validates the given element before using it in this collection.
    /// </summary>
    protected virtual T ValidateItem(T item) => item;

    /// <summary>
    /// Obtains the key associated with the given element.
    /// </summary>
    protected virtual K GetKey(T item) => throw new NotImplementedException();

    /// <summary>
    /// Validates the given key before using it in this collection.
    /// </summary>
    protected virtual K ValidateKey(K key) => key;

    /// <summary>
    /// Invoked to compare the given source key with the other given one.
    /// </summary>
    protected virtual bool CompareKeys(K source, K item) =>
        (source is null && item is null) ||
        (source is not null && source.Equals(item));

    /// <summary>
    /// Determines if the given source element is the same as the other given one.
    /// </summary>
    protected virtual bool SameItem(T source, T item) =>
        (source is null && item is null) ||
        (source is not null && source.Equals(item));

    /// <summary>
    /// Obtains the indexes of the source elements whose key is duplicated of the given one.
    /// </summary>
    protected virtual List<int> GetDuplicates(K key) => IndexesOf(key);

    /// <summary>
    /// Determines if the given duplicated item is added, or ignored, or throws an exception.
    /// </summary>
    protected virtual bool AcceptDuplicate(T source, T item) => true;

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
    bool IList.Contains(object? value) => Contains(GetKey((T)value!));
    bool ICollection<T>.Contains(T item) => Contains(GetKey(item));

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

    // ----------------------------------------------------

    void TryIncrease()
    {
        if (Items.Count < Items.Capacity) return;
        else if (Items.Count < 8) Items.Capacity += 4;
        else if (Items.Count < 24) Items.Capacity += 8;
        else Items.Capacity += Items.Capacity / 2;
    }

    void TryIncrease(IEnumerable<T> range)
    {
        var size = TentativeCount(range);
        if (size == 0) return;
        else if ((Items.Capacity - Items.Count) < size) Items.Capacity += size;
    }

    static int TentativeCount(IEnumerable<T> range) =>
        range is ICollection<T> rt ? rt.Count :
        range is ICollection rg ? rg.Count :
        0;

    void TryDecrease()
    {
        if (Items.Count < 2 && Items.Capacity > 2) Items.Capacity = 2;
        else if (Items.Count < Items.Capacity / 2) Items.Capacity /= 2;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

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
        if (SameItem(source, item)) return 0;

        RemoveAt(index, false);
        return Insert(index, item, false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item) => Add(item, true);
    int Add(T item, bool increase)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = GetDuplicates(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        if (increase) TryIncrease();
        Items.Add(item);
        return 1;
    }
    int IList.Add(object? value) { var num = Add((T)value!); return num >= 0 ? Count : -1; }
    void ICollection<T>.Add(T item) => Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range) => AddRange(range, true);
    int AddRange(IEnumerable<T> range, bool increase)
    {
        range.ThrowWhenNull();

        if (increase) TryIncrease(range);
        var num = 0; foreach (var item in range)
        {
            var r = Add(item, !increase);
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
    public virtual int Insert(int index, T item) => Insert(index, item, true);
    int Insert(int index, T item, bool increase)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = GetDuplicates(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        if (increase) TryIncrease();
        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T value) => Insert(index, value);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range, true);
    int InsertRange(int index, IEnumerable<T> range, bool increase)
    {
        range.ThrowWhenNull();

        if (increase) TryIncrease(range);
        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item, !increase);
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index) => RemoveAt(index, true);
    int RemoveAt(int index, bool decrease)
    {
        Items.RemoveAt(index);
        if (decrease) TryDecrease();
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
        TryDecrease();
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
    void IList.Remove(object? value) => Remove(GetKey((T)value!));
    bool ICollection<T>.Remove(T item) => Remove(GetKey(item)) > 0;

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

            if (index >= 0) num += RemoveAt(index, false);
            else break;
        }
        TryDecrease();
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

            if (index >= 0) num += RemoveAt(index, false);
            else break;
        }
        TryDecrease();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear() => Clear(true);
    int Clear(bool decrease)
    {
        var num = Items.Count; if (num > 0)
        {
            Items.Clear();
            if (decrease) TryDecrease();
        }
        return num;
    }
    void IList.Clear() => Clear();
    void ICollection<T>.Clear() => Clear();

    // ----------------------------------------------------

    object ICollection.SyncRoot => Items;
    bool ICollection.IsSynchronized => false;
    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    bool ICollection<T>.IsReadOnly => false;
    void ICollection<T>.CopyTo(T[] array, int index) => Items.CopyTo(array, index);
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((T[])array, index);
}