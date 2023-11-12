namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{TKey, TItem}"/>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
[DebuggerDisplay("{ToDebugString()}")]
public class CoreList<TKey, TItem> : ICoreList<TKey, TItem>
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
    /// Initializes a new instance with the element from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<TItem> range) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<TKey, TItem> source)
    {
        source.ThrowWhenNull();
        AddRange(source.Items);
    }

    /// <summary>
    /// <inheritdoc cref="ICoreList{TKey, TItem}.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual CoreList<TKey, TItem> Clone() => new(this);
    ICoreList<TKey, TItem> ICoreList<TKey, TItem>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

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
    /// Invoked to obtain a string representation of this instance for DEBUG purposes.
    /// </summary>
    /// <returns></returns>
    protected string ToDebugString()
    {
        var items = Items.Count < DEBUGCOUNT ? Items : Items.Take(DEBUGCOUNT);
        return $"({Count}):[{string.Join(", ", items)}]";
    }
    static int DEBUGCOUNT = 8;

    // ----------------------------------------------------

    /// <summary>
    /// Validates and returns the given element.
    /// <br/> This method must throw an appropriate exception if the element is not valid for
    /// this instance.
    /// <br/> The default implementation of this method just returns the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual TItem ValidateItem(TItem item) => item;

    /// <summary>
    /// Extracts the appropriate key for the given element.
    /// <br/> This method must throw an appropriate exception if the key cannot be extracted
    /// from the given element as, for instance, when it is a container element.
    /// <br/> The default implementation of this method just throws an exception.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual TKey GetKey(TItem item) => throw new NotImplementedException();

    /// <summary>
    /// Validates and return the given key.
    /// <br/> The default implementation of this method just returns the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual TKey ValidateKey(TKey key) => key;

    /// <summary>
    /// Compares the two given keys to determine if they can be considered as the same one for
    /// the purposes of this instance, or not.
    /// <br/> The default implementation of this method just invokes the default comparer of the
    /// type of the keys.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareKeys(TKey source, TKey target) => EqualityComparer<TKey>.Default.Equals(source, target);

    /// <summary>
    /// Determines if the given element, which is considered a duplicated one, can be added or
    /// or inserted into this instance, or not.
    /// <br/> The default implementation of this method just returns <c>true</c>.
    /// <br/> It is expected that this method will throw an appropriate exception if duplicates
    /// are not allowed in this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool AcceptDuplicate(TItem item) => true;

    /// <summary>
    /// Determines if the given element, which is an enumeration of the base elements of this
    /// instance, shall be expanded and its own child elements used, or rather to use the given
    /// element instead.
    /// <br/> The default implementation of this method just returns <c>false</c>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool ExpandNested(TItem item) => false;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual int Count => Items.Count;

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
    public bool Contains(TKey key) => IndexOf(key) >= 0;
    bool ICollection<TItem>.Contains(TItem item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(TKey key)
    {
        key = ValidateKey(key);

        for (int i = 0; i < Items.Count; i++)
        {
            var same = CompareKeys(GetKey(Items[i]), key);
            if (same) return i;
        }
        return -1;
    }
    int IList<TItem>.IndexOf(TItem item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(TKey key)
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
    public List<int> IndexesOf(TKey key)
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
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate)
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
    public int LastIndexOf(Predicate<TItem> predicate)
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
    public List<int> IndexesOf(Predicate<TItem> predicate)
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
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => new(Items);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<TItem> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first element that can be considered as a duplicate of the
    /// given item, or -1 if any.
    /// </summary>
    protected virtual int FindDuplicate(TItem item)
    {
        var key = GetKey(item);
        var num = IndexOf(key);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns><inheriteddoc/></returns>
    public virtual int Replace(int index, TItem item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        var same = EqualityComparer<TItem>.Default.Equals(source, item);
        if (same) return 0;

        var range = ToArray();
        RemoveAt(index);

        var count = Insert(index, item); if (count == 0)
        {
            Items.Clear();
            Items.AddRange(range);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheriteddoc/></returns>
    public virtual int Add(TItem item)
    {
        if (item is IEnumerable<TItem> range && ExpandNested(item))
        {
            return AddRange(range);
        }

        item = ValidateItem(item);
        var num = FindDuplicate(item);
        if (num >= 0 && !AcceptDuplicate(item)) return 0;

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
    /// <returns><inheriteddoc/></returns>
    public virtual int AddRange(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range)
        {
            var num = Add(item);
            count += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns><inheriteddoc/></returns>
    public virtual int Insert(int index, TItem item)
    {
        if (item is IEnumerable<TItem> range && ExpandNested(item))
        {
            return InsertRange(index, range);
        }

        item = ValidateItem(item);
        var num = FindDuplicate(item);
        if (num >= 0 && !AcceptDuplicate(item)) return 0;

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
    /// <returns><inheritdoc/></returns>
    public virtual int InsertRange(int index, IEnumerable<TItem> range)
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
    /// <returns><inheriteddoc/></returns>
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
    /// <returns><inheriteddoc/></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns><inheriteddoc/></returns>
    public virtual int Remove(TKey key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<TItem>.Remove(TItem item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? value) => Remove(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns><inheriteddoc/></returns>
    public virtual int RemoveLast(TKey key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns><inheriteddoc/></returns>
    public virtual int RemoveAll(TKey key)
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
    /// <returns><inheriteddoc/></returns>
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
    /// <returns><inheriteddoc/></returns>
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
    /// <returns><inheriteddoc/></returns>
    public virtual int RemoveAll(Predicate<TItem> predicate)
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
    /// <returns><inheritdoc/></returns>
    public virtual int Clear()
    {
        var count = Items.Count; if (count > 0) Items.Clear();
        return count;
    }
    void ICollection<TItem>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;

    bool ICollection<TItem>.IsReadOnly => false;
    void ICollection<TItem>.CopyTo(TItem[] array, int arrayIndex) => ((ICollection<TItem>)Items).CopyTo(array, arrayIndex);

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    void ICollection.CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj);
    public override int GetHashCode() => Items.GetHashCode();
}