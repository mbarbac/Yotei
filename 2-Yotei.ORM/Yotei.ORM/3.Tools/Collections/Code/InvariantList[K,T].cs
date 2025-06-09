namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc cref="IInvariantList{K, T}"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public abstract partial class InvariantList<K, T> : IInvariantList<K, T>
{
    /// <summary>
    /// The actual repository of contents of this instance.
    /// </summary>
    protected abstract ICoreList<K, T> Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() { }

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range)
    {
        Items.AddRange(range);
        Items.Trim();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public InvariantList(InvariantList<K, T> source)
    {
        Items.Capacity = source.Items.Count;
        Items.AddRange(source);
        Items.Trim();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString() ?? string.Empty;

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

    /// <inheritdoc/>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual InvariantList<K, T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (index == 0 && count == 0) return Clear();

        var range = Items.ToList().GetRange(index, count);
        var clone = Clone();

        clone.Items.Clear();
        clone.Items.AddRange(range);
        return clone;
    }
    IInvariantList<K, T> IInvariantList<K, T>.GetRange(int index, int count) => GetRange(index, count);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> Replace(int index, T item)
    {
        var clone = Clone();
        var done = clone.Items.Replace(index, item);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Replace(int index, T item) => Replace(index, item);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> Add(T item)
    {
        var clone = Clone();
        var done = clone.Items.Add(item);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Add(T item) => Add(item);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.AddRange(IEnumerable<T> range) => AddRange(range);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> Insert(int index, T item)
    {
        var clone = Clone();
        var done = clone.Items.Insert(index, item);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Insert(int index, T item) => Insert(index, item);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.InsertRange(index, range);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAt(index);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.Items.RemoveRange(index, count);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveRange(int index, int count) => RemoveRange(index, count);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> Remove(K key)
    {
        var clone = Clone();
        var done = clone.Items.Remove(key);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Remove(K key) => Remove(key);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> RemoveLast(K key)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(key);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveLast(K key) => RemoveLast(key);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> RemoveAll(K key)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(key);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveAll(K key) => RemoveAll(key);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Remove(Predicate<T> predicate) => Remove(predicate);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveLast(Predicate<T> predicate) => RemoveLast(predicate);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> RemoveAll(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(predicate);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveAll(Predicate<T> predicate) => RemoveAll(predicate);

    /// <inheritdoc/>
    public virtual InvariantList<K, T> Clear()
    {
        var clone = Clone();
        var done = clone.Items.Clear();
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Clear() => Clear();
}