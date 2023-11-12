namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identified by their respective keys.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
[DebuggerDisplay("{ToString(6)}")]
public abstract class CoreList<TKey, TItem>
    : IList<TItem>, IList, ICollection<TItem>, ICollection, IEnumerable<TItem>
{
    /// <summary>
    /// Determines if the given element is an appropriate one for this collection, or throws an
    /// appropriate exception otherwise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TItem ValidateItem(TItem item);

    /// <summary>
    /// Determines if the given item, which can be considered as a duplicate of the existing
    /// one at the given index, can be added or inserted into this collection, or not. It is
    /// expected that this method throws an appropriate exception if duplicated are not allowed
    /// in this collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool AcceptDuplicate(int index, TItem item);

    /// <summary>
    /// Returns the key associated with the given element. It is expected that this method throws
    /// an appropriate exception if the key cannot be obtained.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TKey GetKey(TItem item);

    /// <summary>
    /// Returns a validated key that can be used for comparison purposes. It is expected this
    /// method throws an appropriate exception if the key is not a valid one, but only for that
    /// purposes.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract TKey ValidateKey(TKey key);

    /// <summary>
    /// Determines if the given target key matches any of the source ones, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool CompareKeys(TKey source, TKey target);

    // ----------------------------------------------------

    /// <summary>
    /// The actual list of elements carried by this instance.
    /// </summary>
    protected List<TItem> Items { get; } = [];

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
    /// Returns a string representation of this instance with at most the given number of
    /// elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string ToString(int count)
    {
        var items = Items.Count <= count ? Items : Items.Take(count);
        return $"({Count}):[{string.Join(", ", items)}]";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// Gets or sets the element stored at the given index.
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
    /// Determines if this collection contains any elements that match the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TKey key) => IndexOf(key) >= 0;
    bool ICollection<TItem>.Contains(TItem item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((TItem)value!));

    /// <summary>
    /// Returns the index of the first element in this collection with a key that matches the
    /// given one, or -1 if any can be found.
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
    /// Returns the index of the last element in this collection with a key that matches the
    /// given one, or -1 if any can be found.
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
    /// Returns the indexes of all the elements in this collection keys that match the given one.
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
    /// Determines if this collection contains any elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first element in this collection that match the given predicate,
    /// or -1 if any can be found.
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
    /// Returns the index of the last element in this collection that match the given predicate,
    /// or -1 if any can be found.
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
    /// Returns the indexes of the elements in this collection that match the given predicate.
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
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the given number of elements starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<TItem> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// Sets the element at the given index with the new given one. If the given element can
    /// be considered as equal to the existing one at that index, then no replacement is made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, TItem item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if (CompareKeys(GetKey(source), GetKey(item))) return 0;

        var range = ToArray();
        RemoveAt(index);

        var num = Insert(index, item); if (num == 0)
        {
            Items.Clear();
            Items.AddRange(range);
        }
        return num;
    }

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(TItem item)
    {
        item = ValidateItem(item);

        var num = IndexOf(GetKey(item));
        if (num >= 0 && !AcceptDuplicate(num, item)) return 0;

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
    /// Adds to this collection the elements from the given range.
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
    /// Inserts into this collection the given element.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, TItem item)
    {
        item = ValidateItem(item);

        var num = IndexOf(GetKey(item));
        if (num >= 0 && !AcceptDuplicate(num, item)) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<TItem>.Insert(int index, TItem item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (TItem)value!);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
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
    /// Removes from this collection the element at the given index.
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
    /// Removes from this collection the given number of elements, starting from the given index.
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
    /// Removes from this collection the first element that matches the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int Remove(TKey key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<TItem>.Remove(TItem item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? value) => Remove(GetKey((TItem)value!));

    /// <summary>
    /// Removes from this collection the last element that matches the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveLast(TKey key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the elements that match the given key.
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
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate.
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
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate.
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
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate.
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
    /// Clears this collection.
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