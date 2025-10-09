namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements, identified by their respective keys, with
/// customizable behavior.
/// </summary>
/// <typeparam name="K">The type of the keys used to identify elements.</typeparam>
/// <typeparam name="T">The type of the elements in this collection.</typeparam>
[DebuggerDisplay("{ToDebugString(3)}")]
public abstract class CoreList<K, T>
    : IList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, ICollection<T>
    , IList, ICollection
    , ICloneable
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public CoreList(int capacity) => Items = new(capacity);

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

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract CoreList<K, T> Clone();
    object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a string representation of this instance, suitable for debug purposes, with at
    /// most the given number of elements.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int max)
    {
        if (Count == 0) return "0:[]";
        if (max == 0) return $"{Count}:[...]";

        return Count <= max
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(max).Select(ToDebugItem))}]";
    }

    /// <summary>
    /// Invoked to obtain a debug string representation of the given element.
    /// </summary>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before adding or inserting it into this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract T ValidateItem(T item);

    /// <summary>
    /// Invoked to obtain the value of the key by which the given element is known.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract K GetKey(T item);

    /// <summary>
    /// Invoked to validate the key by which some element is known.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected abstract K ValidateKey(K key);

    /// <summary>
    /// Used to enforce flatten collections.
    /// Determines if elements that are themselves collections of elements shall be expanded, and
    /// its own elements used instead of the original one, or not. 
    /// </summary>
    protected abstract bool ExpandElements { get; }

    /// <summary>
    /// Invoked to determine if the given 'item', which is a duplicate of the existing 'source'
    /// element, can can be added or inserted into this collection, or not. This method shall:
    /// <br/>- Returns '<c>true</c>' to include the duplicated element.
    /// <br/>- Returns '<c>false</c>' to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract bool IsValidDuplicate(T source, T item);

    /// <summary>
    /// The comparer used by this instance to determine equality of keys.
    /// </summary>
    protected abstract IEqualityComparer<K> Comparer { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets or sets the element at the given index.
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
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(K key) => IndexOf(key) >= 0;
    bool ICollection<T>.Contains(T item) => Contains(GetKey(item));
    bool IList.Contains(object? value) => Contains(GetKey((T)value!));

    /// <summary>
    /// Returns the index of the first element in this collection with the given key, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(K key) => IndexOf(key, validate: true);
    int IndexOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return IndexOf(x => Comparer.Equals(key, GetKey(x)));
    }
    int IList<T>.IndexOf(T item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((T)value!));

    /// <summary>
    /// Returns the index of the last element in this collection with the given key, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(K key) => LastIndexOf(key, validate: true);
    int LastIndexOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return LastIndexOf(x => Comparer.Equals(key, GetKey(x)));
    }

    /// <summary>
    /// Returns the indexes of all the element in this collection with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(K key) => IndexesOf(key, validate: true);
    List<int> IndexesOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return IndexesOf(x => Comparer.Equals(key, GetKey(x)));
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
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
    /// Returns the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
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
    /// Returns the indexes of all the element in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <summary>
    /// Returns a new array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => [.. Items];

    /// <summary>
    /// Returns a new list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => [.. Items];

    /// <summary>
    /// Returns a new list with the given number of elements from this collection, starting from
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> ToList(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// Gets or sets the total number of elements the internal data structures can hold without
    /// resizing.
    /// </summary>
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the indexes of the source elements that carry the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected virtual List<int> FindDuplicates(K key) => IndexesOf(key);

    /// <summary>
    /// Invoked to determine if the 'source' element shall be considered the same as the given
    /// 'item' one, or not. If they can be considered the same, the source element will never be
    /// replaced by the given item.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool SameItem(T source, T item)
    {
        return typeof(T).IsValueType
            ? Comparer.Equals(GetKey(source), GetKey(item))
            : ReferenceEquals(source, item);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the given one. If an empty enumeration is
    /// given, then an exception is thrown.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        // Saving original element...
        var source = Items[index];

        // Replacing an existing element with a range...
        if (ExpandElements && item is IEnumerable<T> range)
        {
            var num = RemoveAt(index);
            if (num == 1)
            {
                num = InsertRange(index, range);
                if (num > 0) return num;
                if (!range.Any())
                    throw new EmptyException("Replacement element is an empty range.");
            }
        }

        // Standard replacement...
        else
        {
            if (SameItem(source, item)) return 0;

            var num = RemoveAt(index);
            if (num == 1)
            {
                num = Insert(index, item);
                if (num > 0) return num;
            }
        }

        // Restoring original element...
        if (Insert(index, source) == 0) throw new InvalidOperationException(
                "Cannot restore removed source element after failed replacement.")
                .WithData(index)
                .WithData(source)
                .WithData(this);

        return 0;
    }

    /// <summary>
    /// Adds to this collection the given element.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        if (ExpandElements && item is IEnumerable<T> range) return AddRange(range);

        item = ValidateItem(item);
        var key = ValidateKey(GetKey(item));
        var dups = FindDuplicates(key); foreach (var dup in dups)
        {
            var source = Items[dup];
            if (!IsValidDuplicate(source, item)) return 0;
        }

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// <br/> Returns the number of changes made.
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
    /// Inserts into this collection the given element at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        if (ExpandElements && item is IEnumerable<T> range) return InsertRange(index, range);

        item = ValidateItem(item);
        var key = ValidateKey(GetKey(item));
        var dups = FindDuplicates(key); foreach (var dup in dups)
        {
            var source = Items[dup];
            if (!IsValidDuplicate(source, item)) return 0;
        }

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// <br/> Returns the number of changes made.
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
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// <br/> Returns the number of changes made.
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
    /// Removes from this collection the given number of elements, starting at the given index.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count == 0 &&
            index >= 0 && index < Count) return 0;

        Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Removes from this collection the first element with the given key.
    /// <br/> Returns the number of changes made.
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
    /// Removes from this collection the last element with the given key.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveLast(K key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the elements with the given key.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual int RemoveAll(K key)
    {
        var nums = IndexesOf(key);

        for (int i = nums.Count - 1; i >= 0; i--)
        {
            var index = nums[i];
            Items.RemoveAt(index);
        }
        return nums.Count;
    }

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        var nums = IndexesOf(predicate);

        for (int i = nums.Count - 1; i >= 0; i--)
        {
            var index = nums[i];
            Items.RemoveAt(index);
        }
        return nums.Count;
    }

    /// <summary>
    /// Clears this collection.
    /// <br/> Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var num = Count; if (num > 0) Items.Clear();
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