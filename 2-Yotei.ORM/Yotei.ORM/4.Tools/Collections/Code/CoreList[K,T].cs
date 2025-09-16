namespace Yotei.ORM.Tools;

// ========================================================
/// <inheritdoc cref="ICoreList{K, T}"/>
[DebuggerDisplay("{ToDebugString(5)}")]
public abstract class CoreList<K, T> : ICoreList<K, T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
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

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a debug string for this instance with at most the given number of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ToDebugItem))}, ...]";
    }

    /// <summary>
    /// Invoked to obtain the debug string of the given element.
    /// </summary>
    protected virtual string ToDebugItem(T item) => item?.ToString() ?? "-";

    /// <inheritdoc/>
    public abstract ICoreList<K, T> Clone();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before adding or inserting it into this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract T ValidateItem(T item);

    /// <summary>
    /// Invoked to obtain the key by which the given element is known.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract K GetKey(T item);

    /// <summary>
    /// Invoked to validate the given key before using it in this collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract K ValidateKey(K key);

    /// <summary>
    /// Determines if elements that are themselves collections of elements shall be expanded, and
    /// those elements used instead of the original one, or not.
    /// </summary>
    public abstract bool ExpandItems { get; }

    /// <summary>
    /// Invoked to determine if the given '<paramref name="item"/>', that is a duplicate of an
    /// existing '<paramref name="source"/>' element, can be added or inserted into this collection
    /// or not. This method shall:
    /// <br/>- Return <c>true</c> to include the duplicated element.
    /// <br/>- Return <c>false</c> to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool IsValidDuplicate(T source, T item);

    /// <summary>
    /// The comparer used to determine the equality of two given keys.
    /// </summary>
    public abstract IEqualityComparer<K> Comparer { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the indexes of all the elements that carry duplicated keys.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected virtual List<int> FindDuplicates(K key) => IndexesOf(key);

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered the same or not.
    /// <br/> This method is used to determine if an existing element shall be replaced by the
    /// given item, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool SameItem(T source, T item) => typeof(T).IsValueType
        ? Comparer.Equals(GetKey(source), GetKey(item))
        : ReferenceEquals(source, item);

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
    int IList<T>.IndexOf(T item) => IndexOf(GetKey(item));
    int IList.IndexOf(object? value) => IndexOf(GetKey((T)value!));

    int IndexOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return IndexOf(x => Comparer.Equals(key, GetKey(x)));
    }

    /// <inheritdoc/>
    public int LastIndexOf(K key) => LastIndexOf(key, validate: true);

    int LastIndexOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return LastIndexOf(x => Comparer.Equals(key, GetKey(x)));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(K key) => IndexesOf(key, validate: true);

    List<int> IndexesOf(K key, bool validate)
    {
        if (validate) key = ValidateKey(key);
        return IndexesOf(x => Comparer.Equals(key, GetKey(x)));
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

        List<int> list = [];
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    /// <inheritdoc/>
    public List<T> ToList(int index, int count) => Items.GetRange(index, count);

    /// <inheritdoc/>
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <inheritdoc/>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        var source  = Items[index];
        if (SameItem(source, item)) return 0;

        var given = index;
        var num = RemoveAt(index);
        if (num == 0) return 0;

        // Expansion...
        if (ExpandItems && item is IEnumerable<T> range)
        {
            // If range is empty then num will be cero, which in turn drives restoring of the
            // removed source...
            num = 0;
            foreach (var temp in range)
            {
                var r = Insert(index, temp);
                num += r;
                index += r;
            }
        }

        // Standard case...
        else
        {
            item = ValidateItem(item);
            num = Insert(index, item);
        }

        // Restoring removed element when no insertions happened...
        if (num == 0)
        {
            if (Insert(given, source) == 0) throw new InvalidOperationException(
                "Cannot restore removed source item when replacing failed.")
                .WithData(source)
                .WithData(index)
                .WithData(item)
                .WithData(this);
        }

        // Finishing...
        return num;
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        if (ExpandItems && item is IEnumerable<T> range) return AddRange(range);

        item = ValidateItem(item);

        var key = GetKey(item);
        var dups = FindDuplicates(key);
        foreach (var dup in dups) if (!IsValidDuplicate(Items[dup], item)) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual int Insert(int index, T item)
    {
        if (ExpandItems && item is IEnumerable<T> range) return InsertRange(index, range);

        item = ValidateItem(item);

        var key = GetKey(item);
        var dups = FindDuplicates(key);
        foreach (var dup in dups) if (!IsValidDuplicate(Items[dup], item)) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual int RemoveRange(int index, int count)
    {
        if (count == 0 &&
            index >= 0 && index < Items.Count) return 0;

        Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(K key)
    {
        var index = IndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<T>.Remove(T item) => Remove(GetKey(item)) > 0;
    void IList.Remove(object? value) => Remove(GetKey((T)value!));

    /// <inheritdoc/>
    public virtual int RemoveLast(K key)
    {
        var index = LastIndexOf(key);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(K key)
    {
        key = ValidateKey(key);

        var num = 0; while (true)
        {
            var index = IndexOf(key, validate: false);

            if (index >= 0)
            {
                var r = RemoveAt(index);
                if (r > 0) num += r;
                else break;
            }
            else break;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0)
            {
                var r = RemoveAt(index);
                if (r > 0) num += r;
                else break;
            }
            else break;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Clear()
    {
        var num = Count;

        if (num > 0) Items.Clear();
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