namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public CoreList(int capacity) => Items = new(capacity);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source) : this() => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Invoked to produce a debug string.
    /// </summary>
    public virtual string ToDebugString(int count)
    {
        if (Count == 0) return $"0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ToDebugItem))}, ...]";
    }

    string ToDebugItem(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given item before using it in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual T ValidateItem(T item) => item;

    /// <summary>
    /// The comparer to use to determine equality between two given elements.
    /// </summary>
    public virtual IEqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

    /// <summary>
    /// Determines if when, including in this collection elements that are themselves collections
    /// of the elements this one is built for, then their elements shall be used instead of the
    /// original ones.
    /// </summary>
    public virtual bool ExpandItems { get; } = true;

    /// <summary>
    /// Invoked to determine if the given item can be included in this collection when its key
    /// is considered a duplicate of the one of the given existing source element. This method
    /// shall:
    /// <br/>- Return <c>true</c> if the item shall be added or inserted.
    /// <br/>- Return <c>false</c> if the include operation shall be just ignored.
    /// <br/>- Throw an appropriate exception if needed.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual bool IncludeDuplicated(T item, T source) => true;

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
    public bool Contains(T item) => IndexOf(item) >= 0;
    bool IList.Contains(object? value) => Contains((T)value!);

    /// <inheritdoc/>
    public int IndexOf(T item) => IndexOf(item, validate: true);
    int IList.IndexOf(object? value) => IndexOf((T)value!);

    int IndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return IndexOf(x => Comparer.Equals(item, x));
    }

    /// <inheritdoc/>
    public int LastIndexOf(T item) => LastIndexOf(item, validate: true);

    int LastIndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return LastIndexOf(x => Comparer.Equals(item, x));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(T item) => IndexesOf(item, validate: true);

    List<int> IndexesOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return IndexesOf(x => Comparer.Equals(item, x));
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

    /// <summary>
    /// Invoked to determine if the given items are the same or not.
    /// </summary>
    protected virtual bool SameItem(T item, T source)
    {
        return typeof(T).IsValueType
            ? Comparer.Equals(item, source)
            : ReferenceEquals(item, source);
    }

    /// <summary>
    /// Invoked to find the indexes of the elements in this collection that can be considered
    /// as duplicates of the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual List<int> FindDuplicates(T item) => IndexesOf(item);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        Replace(index, item, remove1st: true, out var removed, out var inserted);
        return inserted == 0 ? removed : inserted;
    }

    void Replace(int index, T item, bool remove1st, out int removed, out int inserted)
    {
        removed = 0;
        inserted = 0;

        if (item is IEnumerable<T> range && ExpandItems) // Enumerable expansion...
        {
            removed = RemoveAt(index);
            if (removed == 0) return;

            foreach (var temp in range)
            {
                Replace(index, temp, remove1st: false, out var xremoved, out var xinserted);
                inserted += xinserted;
                index += xinserted;
            }
        }

        else // Standard case...
        {
            item = ValidateItem(item);

            var source = Items[index];
            var same = SameItem(item, source);
            if (same) return;

            removed = remove1st ? RemoveAt(index) : 0;
            if (removed == 0 && remove1st) return;

            inserted = Insert(index, item);
        }
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems) return AddRange(range);

        item = ValidateItem(item);

        var dups = FindDuplicates(item);
        foreach (var dup in dups) if (!IncludeDuplicated(item, Items[dup])) return 0;

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
        if (item is IEnumerable<T> range && ExpandItems) return InsertRange(index, range);

        item = ValidateItem(item);

        var dups = FindDuplicates(item);
        foreach (var dup in dups) if (!IncludeDuplicated(item, Items[dup])) return 0;

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
            num += r;
            index += r;
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
        Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = Remove(temp);
                num += r;
            }
            return num;
        }
        else
        {
            var index = IndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(ValidateItem(item)) > 0;
    void IList.Remove(object? value) => Remove(ValidateItem((T)value!));

    /// <inheritdoc/>
    public virtual int RemoveLast(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveLast(temp);
                num += r;
            }
            return num;
        }
        else
        {
            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveAll(temp);
                num += r;
            }
            return num;
        }
        else
        {
            item = ValidateItem(item);

            var num = 0; while (true)
            {
                var index = IndexOf(item, validate: false);

                if (index >= 0) num += RemoveAt(index);
                else break;
            }
            return num;
        }
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