namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="ICoreList{T}"/>
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
    /// Initializes a new instance with the elements from the given range.
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
    /// <param name="count"></param>
    /// <param name="item2debug"></param>
    /// <returns></returns>
    protected virtual string ToDebugString(int count, Func<T, string>? item2debug)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[]";

        item2debug ??= (item) => item?.ToString() ?? "-";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(item2debug))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(item2debug))}, ...]";
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The default implementation of this method just returns the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual T ValidateItem(T item) => item;

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The default implementation of this method just uses the default comparer for the
    /// type of the elements.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public virtual bool CompareItems(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The default implementation of this method just uses the <see cref="IndexesOf(T)"/>
    /// method.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual List<int> GetDuplicates(T item) => IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The default implementation of this method just returns <c>true</c>.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual bool CanInclude(T item, T source) => true;

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The default implementation of this method just returns <c>true</c>.
    /// </summary>
    /// <returns></returns>
    public virtual bool ExpandItems() => true;

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
        return IndexOf(x => CompareItems(x, item));
    }

    /// <inheritdoc/>
    public int LastIndexOf(T item) => LastIndexOf(item, validate: true);

    int LastIndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return LastIndexOf(x => CompareItems(x, item));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(T item) => IndexesOf(item, validate: true);

    List<int> IndexesOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return IndexesOf(x => CompareItems(x, item));
    }

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    /// <inheritdoc/>
    public void Reverse() => Items.Reverse();

    /// <inheritdoc/>
    public void Sort() => Sort(new KeysComparer(this));
    struct KeysComparer(CoreList<T> Master) : IComparer<T>
    {
        public readonly int Compare(T? x, T? y)
        {
            if (x is null && y is null) return 0;
            if (x is not null) return -1;
            if (y is not null) return +1;

            if (ReferenceEquals(x, y)) return 0;
            return Master.CompareItems(x!, y!) ? 0 : -1;
        }
    }

    /// <inheritdoc/>
    public void Sort(IComparer<T> comparer) => Items.Sort(comparer);

    /// <inheritdoc/>
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <inheritdoc/>
    public void Trim() => Items.TrimExcess();

    /// <inheritdoc/>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        var same = Same(source, item);
        if (same) return 0;

        var removed = RemoveAt(index);
        var inserted = Insert(index, item);
        return inserted == 0 ? removed : inserted;
    }
    bool Same(T source, T item)
    {
        if (!typeof(T).IsValueType)
        {
            if (source is null && item is null) return true;
            if (source is not null) return false;
            if (item is not null) return false;

            if (ReferenceEquals(source, item)) return true;
        }
        return CompareItems(source, item);
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems()) return AddRange(range);

        item = ValidateItem(item);

        var dups = GetDuplicates(item);
        foreach (var dup in dups) if (!CanInclude(item, Items[dup])) return 0;

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
        if (item is IEnumerable<T> range && ExpandItems()) return InsertRange(index, range);

        item = ValidateItem(item);

        var dups = GetDuplicates(item);
        foreach (var dup in dups) if (!CanInclude(item, Items[dup])) return 0;

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
        if (item is IEnumerable<T> range && ExpandItems())
        {
            var num = 0; foreach (var temp in range) num += Remove(temp);
            return num;
        }
        else
        {
            var index = IndexOf(item, validate: false);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;
    void IList.Remove(object? value) => Remove((T)value!);

    /// <inheritdoc/>
    public virtual int RemoveLast(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems())
        {
            var num = 0; foreach (var temp in range) num += RemoveLast(temp);
            return num;
        }
        else
        {
            var index = LastIndexOf(item, validate: false);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(T item)
    {
        if (item is IEnumerable<T> range && ExpandItems())
        {
            var num = 0; foreach (var temp in range) num += RemoveAll(temp);
            return num;
        }
        else
        {
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
        var temp = Count; if (temp > 0) Items.Clear();
        return temp;
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