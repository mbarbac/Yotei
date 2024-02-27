/*namespace Yotei.Tools.Code;

// ========================================================
/// <inheritdoc cref="IFrozenList{K, T}"/>
[Cloneable]
[DebuggerDisplay("{ToDebugString(6)}")]
public partial class FrozenList<K, T> : IFrozenList<K, T>
{
    protected virtual ICoreList<K, T> Items { get; } = new CoreList<K, T>();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public FrozenList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public FrozenList(T item) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public FrozenList(IEnumerable<T> range) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenList(CoreList<K, T> source) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count) => Items.ToDebugString(count);

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

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (count == 0 && index >= 0) return Clear();

        var range = Items.ToList().GetRange(index, count);
        var clone = Clone();
        clone.Items.Clear();
        clone.Items.AddRange(range);
        return clone;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Replace(int index, T item)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Add(T item)
    {
        var clone = Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> AddRange(IEnumerable<T> range)
    {
        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Insert(int index, T item)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAt(int index)
    {
        if (index < 0 || index >= Items.Count) throw new IndexOutOfRangeException("Index out of range.").WithData(index);

        var clone = Clone();
        var num = clone.Items.RemoveAt(index);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveRange(int index, int count)
    {
        if (count < 0) throw new ArgumentException("Count is less than cero.").WithData(count);
        if (index < 0 || index >= Items.Count) throw new IndexOutOfRangeException("Index out of range.").WithData(index);

        if (count > 0)
        {
            var clone = Clone();
            var num = clone.Items.RemoveRange(index, count);
            if (num > 0) return clone;
        }
        return this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Remove(K key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveLast(K key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAll(K key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Remove(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<K, T> Clear()
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }
}*/