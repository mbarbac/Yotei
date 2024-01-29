namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc cref="ICoreList{T}"/>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class CoreList<T> : ICoreList<T>
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
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source) => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count) => Count == 0 ? "0:[]" : (
        Count < count
        ? $"{Count}:[{string.Join(", ", this.Select(ItemToString))}]"
        : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToString))}, ...]");

    protected virtual string ItemToString(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    void Reload()
    {
        if (Items.Count == 0) return;

        var range = Items.ToArray();
        Items.Clear();
        AddRange(range);
    }

    /// <inheritdoc/>
    public Func<T, T> Validate
    {
        get => _Validate;
        set
        {
            if (ReferenceEquals(_Validate, value.ThrowWhenNull())) return;
            _Validate = value;
            Reload();
        }
    }
    Func<T, T> _Validate = (item) => item;

    /// <inheritdoc/>
    public Func<T, T, bool> Compare
    {
        get => _Comparer;
        set
        {
            if (ReferenceEquals(_Comparer, value.ThrowWhenNull())) return;
            _Comparer = value;
            Reload();
        }
    }
    Func<T, T, bool> _Comparer = EqualityComparer<T>.Default.Equals;

    /// <inheritdoc/>
    public Func<ICoreList<T>, T, List<int>> Duplicates
    {
        get => _Duplicates;
        set
        {
            if (ReferenceEquals(_Duplicates, value.ThrowWhenNull())) return;
            _Duplicates = value;
            Reload();
        }
    }
    Func<ICoreList<T>, T, List<int>> _Duplicates = (@this, item) => @this.IndexesOf(item);

    /// <inheritdoc/>
    public Func<T, T, bool> CanInclude
    {
        get => _CanInclude;
        set
        {
            if (ReferenceEquals(_CanInclude, value.ThrowWhenNull())) return;
            _CanInclude = value;
            Reload();
        }
    }
    Func<T, T, bool> _CanInclude = (_, _) => true;

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
    bool IList.Contains(object? value) => Contains(Validate((T)value!));

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        item = Validate(item);
        return IndexOf(x => Compare(x, item));
    }
    int IList.IndexOf(object? value) => IndexOf(Validate((T)value!));

    /// <inheritdoc/>
    public int LastIndexOf(T item)
    {
        item = Validate(item);
        return LastIndexOf(x => Compare(x, item));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item);
        return IndexesOf(x => Compare(x, item));
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

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    // ----------------------------------------------------

    static bool SameItem(T source, T item) => typeof(T).IsValueType
        ? source!.Equals(item)
        : ReferenceEquals(source, item);

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        item = Validate(item);

        var source = Items[index];
        if (SameItem(source, item)) return 0;

        RemoveAt(index);
        return Insert(index, item);
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        Console.WriteLine($"Entering add...count: {Count}, item: {item}");

        item = Validate(item);

        var prevent = false;
        var range = Duplicates(this, item);
        foreach (var i in range) if (!CanInclude(Items[i], item)) prevent = true;
        if (prevent) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <inheritdoc/>
    public virtual int AddRange(IEnumerable<T> range)
    {
        Console.WriteLine($"Entering addrange...count: {Count}");

        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

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
        item = Validate(item);

        var prevent = false;
        var range = Duplicates(this, item);
        foreach (var i in range) if (!CanInclude(Items[i], item)) prevent = true;
        if (prevent) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <inheritdoc/>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

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
        if (index < 0 || index >= Items.Count) throw new IndexOutOfRangeException("Index out of range.").WithData(index);

        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual int RemoveRange(int index, int count)
    {
        if (count < 0) throw new ArgumentException("Count is less than cero.").WithData(count);
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index less than cero or bigger then count.")
            .WithData(index);

        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(T item)
    {
        if (Count == 0) return 0;

        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<T>.Remove(T item) => Remove(Validate(item)) >= 0;
    void IList.Remove(object? value) => Remove(Validate((T)value!));

    /// <inheritdoc/>
    public virtual int RemoveLast(T item)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(T item)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int Remove(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

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