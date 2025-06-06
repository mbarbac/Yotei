namespace Yotei.ORM.Tools;

// ========================================================
/// <inheritdoc cref="ICoreList{T}"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public abstract partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new instance with the given initial capacity.
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
    public CoreList(CoreList<T> source) : this() => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a debug string for this instance.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract T ValidateItem(T item);

    /// <summary>
    /// The comparer to use to determine equality between two elements.
    /// </summary>
    public virtual IEqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

    /// <summary>
    /// Determines if when including in this collection elements that are themselves collections
    /// of the elements of this instance, then that elements shall be used instead, or otherwise
    /// the given one without expansion.
    /// </summary>
    public virtual bool ExpandItems { get; } = true;

    /// <summary>
    /// Invoked to determine if the given duplicated element can be included in this collection.
    /// This method shall:
    /// <br/>- Return '<c>true</c>' if item is a valid duplicate of the existing source.
    /// <br/>- Return '<c>false</c>' if the including operation shall just be ignored.
    /// <br/>- Throw an appropriate exception if needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool IsValidDuplicate(T source, T item) => true;

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
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <inheritdoc/>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to find the indexes of all the elements that shall be considered duplicates
    /// because carrying the given key.
    /// </summary>
    protected virtual List<int> FindDuplicates(T item) => IndexesOf(item);

    /// <summary>
    /// Determines if the given element is the same as the existing source one, or not. By default,
    /// value types are compared by the equality of their respective keys. Otherwise, reference
    /// equality is used.
    /// </summary>
    protected virtual bool SameItem(T source, T item)
    {
        return typeof(T).IsValueType
            ? Comparer.Equals(source, item)
            : ReferenceEquals(source, item);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        var clone = Clone();
        int num = 0;

        clone.RemoveAt(index);

        if (item is IEnumerable<T> range && ExpandItems) // Enumerable expansion...
        {
            foreach (var temp in range)
            {
                var r = clone.Insert(index, temp);
                num += r;
                index += r;
            }
        }
        else // Standard case...
        {
            item = ValidateItem(item);

            var source = Items[index];
            var same = SameItem(source, item);
            if (same) return 0;

            num = clone.Insert(index, item);
        }

        // Finishing...
        if (num > 0)
        {
            Items.Clear();
            Items.AddRange(clone.Items);
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems) return AddRange(range);

        item = ValidateItem(item);

        var dups = FindDuplicates(item);
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
        if (item is IEnumerable<T> range && ExpandItems) return InsertRange(index, range);

        item = ValidateItem(item);

        var dups = FindDuplicates(item);
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
        if (count == 0 && index >= 0 && index < Items.Count) return 0;

        Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems) // Enumerable expansion...
        {
            var num = 0; foreach (var temp in range)
            {
                var r = Remove(temp);
                num += r;
            }
            return num;
        }
        else // Standard case...
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
        if (item is IEnumerable<T> range && ExpandItems) // Enumerable expansion...
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveLast(temp);
                num += r;
            }
            return num;
        }
        else // Standard case...
        {
            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems) // Enumerable expansion...
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveAll(temp);
                num += r;
            }
            return num;
        }
        else // Standard case...
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
        predicate.ThrowWhenNull();

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
        var num = Count;

        if (num > 0 ) Items.Clear();
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