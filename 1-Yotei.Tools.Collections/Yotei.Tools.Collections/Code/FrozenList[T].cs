namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="IFrozenList{T}"/>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class FrozenList<T> : IFrozenList<T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public FrozenList() { }

    /// <summary>
    /// Initializes a new instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public FrozenList(int capacity) => Items.Capacity = capacity;

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public FrozenList(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenList(FrozenList<T> source)
    {
        Items.Capacity = source.ThrowWhenNull().Items.Capacity;
        Items.AddRange(source.Items);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Invoked to produce a debug string.
    /// </summary>
    protected virtual string ToDebugString(int count, Func<T, string>? item2debug = null)
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
    /// The actual storage of the items in this collection.
    /// </summary>
    protected virtual ICoreList<T> Items { get; } = new CoreList<T>();

    /// <inheritdoc/>
    public virtual ICoreList<T> ToBuilder() => Items.Clone();

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IFrozenList<T> Reverse()
    {
        var clone = Clone();
        clone.Items.Reverse();
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Sort(IComparer<T> comparer)
    {
        var clone = Clone();
        clone.Items.Sort(comparer);
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;

        var items = Items.GetRange(index, count);
        var clone = Clone();
        clone.Items.Clear();
        clone.Items.AddRange(items);
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Replace(int index, T item)
    {
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
        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Insert(int index, T item)
    {
        var clone = Clone();
        var done = clone.Items.Insert(index, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.InsertRange(index, range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAt(index);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveRange(int index, int count)
    {
        if (count == 0 && index >= 0 && index < Count) return this;

        var clone = Clone();
        var done = clone.Items.RemoveRange(index, count);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Remove(T item)
    {
        var clone = Clone();
        var done = clone.Items.Remove(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveLast(T item)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveAll(T item)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<T> RemoveAll(Predicate<T> predicate)
    {
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