namespace Yotei.Tools.Collections;

// ========================================================
/// <inheritdoc cref="IFrozenList{T}"/>
[Cloneable]
[DebuggerDisplay("{ToDebugString(6)}")]
public partial class FrozenList<T> : IFrozenList<T>
{
    protected virtual ICoreList<T> Items { get; } = new CoreList<T>();

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
    protected FrozenList(FrozenList<T> source) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
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
    public bool Contains(T item) => Items.Contains(item);

    /// <inheritdoc/>
    public int IndexOf(T item) => Items.IndexOf(item);

    /// <inheritdoc/>
    public int LastIndexOf(T item) => Items.LastIndexOf(item);

    /// <inheritdoc/>
    public List<int> IndexesOf(T item) => Items.IndexesOf(item);

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

    protected virtual bool SameItem(T source, T target) => source.EqualsEx(target);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IFrozenList<T> GetRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0 && Count == 0) return this;
        if (index == 0 && count == Count) return this;

        var clone = Clone();
        var done = clone.Items.GetRange(index, count);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Replace(int index, T item)
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
    public virtual IFrozenList<T> Add(T item)
    {
        var clone = Clone();
        var done = clone.Items.Add(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Insert(int index, T item)
    {
        Validate(index, true);

        var clone = Clone();
        var done = clone.Items.Insert(index, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> InsertRange(int index, IEnumerable<T> range)
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
    public virtual IFrozenList<T> RemoveAt(int index)
    {
        Validate(index);

        var clone = Clone();
        var done = clone.Items.RemoveAt(index);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0) return this;
        if (index == 0 && count == Count) return Clear();

        var clone = Clone();
        var done = clone.Items.RemoveRange(index, count);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Remove(T item)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.Remove(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveLast(T item)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveLast(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveAll(T item)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveAll(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Remove(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.RemoveAll(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Clear()
    {
        if (Count == 0) return this;

        var clone = Clone();
        var done = clone.Items.Clear();
        return done > 0 ? clone : this;
    }
}