namespace Yotei.Tools.Collections;

// ========================================================
/// <inheritdoc cref="IFrozenList{K, T}"/>
[Cloneable]
public partial class FrozenList<K, T> : IFrozenList<K, T>
{
    protected virtual ICoreList<K, T> Items { get; } = new CoreList<K, T>();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public FrozenList() { }

    /// <summary>
    /// Initializes a new instance with the given element
    /// </summary>
    /// <param name="item"></param>
    public FrozenList(T item) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public FrozenList(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenList(FrozenList<K, T> source) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return ToString();

        return Count < count
            ? $"{Count}:[{string.Join(", ", this.Select(ItemToDebugString))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToDebugString))}, ...]";
    }

    protected virtual string ItemToDebugString(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public T this[int index] => Items[index];

    /// <inheritdoc/>
    public bool Contains(K key) => Items.Contains(key);

    /// <inheritdoc/>
    public int IndexOf(K key) => Items.IndexOf(key);

    /// <inheritdoc/>
    public int LastIndexOf(K key) => Items.LastIndexOf(key);

    /// <inheritdoc/>
    public List<int> IndexesOf(K key) => Items.IndexesOf(key);

    /// <inheritdoc/>
    public bool Contains(Predicate<T> predicate) => Items.Contains(predicate);

    /// <inheritdoc/>
    public int IndexOf(Predicate<T> predicate) => Items.IndexOf(predicate);

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<T> predicate) => Items.LastIndexOf(predicate);

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<T> predicate) => Items.IndexesOf(predicate);

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => Items.ToList();

    // ----------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, bool insert = false)
    {
        if (index < 0) throw new IndexOutOfRangeException("Index is negative.").WithData(index);

        var value = Items.Count + (insert ? 1 : 0);
        if (index >= value) throw new IndexOutOfRangeException("Index greater than or equal the number of elements.").WithData(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, int count, bool insert = false)
    {
        Validate(index, insert);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Items.Count - index);
    }

    protected virtual bool SameItem(T source, T target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> GetRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0 && Count == 0) return this;
        if (index == 0 && count == Count) return this;

        var clone = Clone();
        var done = clone.Items.GetRange(index, count);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Replace(int index, T item)
    {
        Validate(index);

        var source = Items[index];
        var same = SameItem(source, item);
        if (same) return this;

        var clone = Clone();
        var done = clone.Items.Replace(index, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Add(T item)
    {
        var clone = Clone();
        var done = clone.Items.Add(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Insert(int index, T item)
    {
        Validate(index, true);

        var clone = Clone();
        var done = clone.Items.Insert(index, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        Validate(index, true);
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.InsertRange(index, range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAt(int index)
    {
        Validate(index);

        var clone = Clone();
        var done = clone.Items.RemoveAt(index);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0) return this;
        if (index == 0 && count == Count) return Clear();

        var clone = Clone();
        var done = clone.Items.RemoveRange(index, count);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Remove(K key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.Remove(key);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveLast(K key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveLast(key);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAll(K key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveAll(key);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Remove(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveAll(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Clear()
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.Clear();
        return done > 0 ? clone : this;
    }
}