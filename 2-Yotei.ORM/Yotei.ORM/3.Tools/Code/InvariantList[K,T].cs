namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc cref="IInvariantList{K, T}"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[Cloneable]
public abstract partial class InvariantList<K, T> : IInvariantList<K, T>
{
    protected abstract ICoreList<K, T> Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() { }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
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
    protected InvariantList(InvariantList<K, T> source)
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

    /// <inheritdoc/>
    public virtual ICoreList<K, T> GetBuilder() => Items.Clone();

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
    public List<T> ToList(int index, int count) => Items.ToList(index, count);

    /// <inheritdoc/>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <inheritdoc cref="IInvariantList{K, T}.GetRange(int, int)"/>
    public virtual InvariantList<K, T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;

        var range = Items.ToList(index, count);
        var clone = Clone();

        clone.Items.Clear();
        clone.Items.AddRange(range);
        return clone;
    }
    IInvariantList<K, T> IInvariantList<K, T>.GetRange(int index, int count) => GetRange(index, count);

    /// <inheritdoc cref="IInvariantList{K, T}.Replace(int, T)"/>
    public virtual InvariantList<K, T> Replace(int index, T item)
    {
        var clone = Clone();
        var done = clone.Items.Replace(index, item);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Replace(int index, T item) => Replace(index, item);

    /// <inheritdoc cref="IInvariantList{K, T}.Add(T)"/>
    public virtual InvariantList<K, T> Add(T item)
    {
        var clone = Clone();
        var done = clone.Items.Add(item);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Add(T item) => Add(item);

    /// <inheritdoc cref="IInvariantList{K, T}.AddRange(IEnumerable{T})"/>
    public virtual InvariantList<K, T> AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.AddRange(IEnumerable<T> range) => AddRange(range);

    /// <inheritdoc cref="IInvariantList{K, T}.Insert(int, T)"/>
    public virtual InvariantList<K, T> Insert(int index, T item)
    {
        var clone = Clone();
        var done = clone.Items.Insert(index, item);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Insert(int index, T item) => Insert(index, item);

    /// <inheritdoc cref="IInvariantList{K, T}.InsertRange(int, IEnumerable{T})"/>
    public virtual InvariantList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.InsertRange(index, range);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveAt(int)"/>
    public virtual InvariantList<K, T> RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAt(index);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveRange(int, int)"/>
    public virtual InvariantList<K, T> RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.Items.RemoveRange(index, count);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveRange(int index, int count) => RemoveRange(index, count);

    /// <inheritdoc cref="IInvariantList{K, T}.Remove(K)"/>
    public virtual InvariantList<K, T> Remove(K key)
    {
        var clone = Clone();
        var done = clone.Items.Remove(key);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Remove(K key) => Remove(key);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveLast(K)"/>
    public virtual InvariantList<K, T> RemoveLast(K key)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(key);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveLast(K key) => RemoveLast(key);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveAll(K)"/>
    public virtual InvariantList<K, T> RemoveAll(K key)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(key);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveAll(K key) => RemoveAll(key);

    /// <inheritdoc cref="IInvariantList{K, T}.Remove(Predicate{T})"/>
    public virtual InvariantList<K, T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Remove(Predicate<T> predicate) => Remove(predicate);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveLast(Predicate{T})"/>
    public virtual InvariantList<K, T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveLast(Predicate<T> predicate) => RemoveLast(predicate);

    /// <inheritdoc cref="IInvariantList{K, T}.RemoveAll(Predicate{T})"/>
    public virtual InvariantList<K, T> RemoveAll(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(predicate);
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.RemoveAll(Predicate<T> predicate) => RemoveAll(predicate);

    /// <inheritdoc cref="IInvariantList{K, T}.Clear"/>
    public virtual InvariantList<K, T> Clear()
    {
        var clone = Clone();
        var done = clone.Items.Clear();
        return done > 0 ? clone : this;
    }
    IInvariantList<K, T> IInvariantList<K, T>.Clear() => Clear();
}