namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="IFrozenList{K, T}"/>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class FrozenList<K, T> : IFrozenList<K, T>
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
    protected FrozenList(FrozenList<K, T> source)
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
    protected virtual ICoreList<K, T> Items { get; } = new CoreList<K, T>();

    /// <inheritdoc/>
    public virtual ICoreList<K, T> ToBuilder() => Items.Clone();

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Reverse()
    {
        var clone = Clone();
        clone.Items.Reverse();
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Sort(IComparer<K> comparer)
    {
        var clone = Clone();
        clone.Items.Sort(comparer);
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> GetRange(int index, int count)
    {
        if (count == 0 && index >= 0 && index < Count) return this;

        var items = Items.GetRange(index, count);
        var clone = Clone();
        clone.Items.Clear();
        clone.Items.AddRange(items);
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Replace(int index, T item)
    {
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
        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Insert(int index, T item)
    {
        var clone = Clone();
        var done = clone.Items.Insert(index, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.InsertRange(index, range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAt(index);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveRange(int index, int count)
    {
        if (count == 0 && index >= 0 && index < Count) return this;

        var clone = Clone();
        var done = clone.Items.RemoveRange(index, count);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Remove(K key)
    {
        var clone = Clone();
        var done = clone.Items.Remove(key);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveLast(K key)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(key);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAll(K key)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(key);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAll(Predicate<T> predicate)
    {
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