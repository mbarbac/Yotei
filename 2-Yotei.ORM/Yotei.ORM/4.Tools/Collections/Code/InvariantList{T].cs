namespace Yotei.ORM.Tools;

// ========================================================
/// <inheritdoc cref="IInvariantList{T}"/>
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public abstract class InvariantList<T> : IInvariantList<T>
{
    protected abstract ICoreList<T> Items { get; }

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
    protected InvariantList(InvariantList<T> source)
    {
        Items.Capacity = source.Items.Capacity;
        Items.AddRange(source.Items);
        Items.Trim();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString() ?? string.Empty;

    /// <inheritdoc/>
    public abstract IInvariantList<T> Clone();

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

    /// <inheritdoc/>
    public List<T> ToList(int index, int count) => Items.ToList(index, count);

    /// <inheritdoc/>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IInvariantList<T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (index == 0 && count == 0) return Clear();

        var range = Items.ToList(index, count);
        var clone = (InvariantList<T>)Clone();

        clone.Items.Clear();
        var num = clone.Items.AddRange(range);

        if (num > 0) clone.Trim();
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> Replace(int index, T item)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.Replace(index, item);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> Add(T item)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.Add(item);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> AddRange(IEnumerable<T> range)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.AddRange(range);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> Insert(int index, T item)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.Insert(index, item);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.InsertRange(index, range);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> RemoveAt(int index)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.RemoveAt(index);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> RemoveRange(int index, int count)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.RemoveRange(index, count);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> Remove(T item)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.Remove(item);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> RemoveLast(T item)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.RemoveLast(item);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> RemoveAll(T item)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.RemoveAll(item);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> Remove(Predicate<T> predicate)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.Remove(predicate);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> RemoveLast(Predicate<T> predicate)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.RemoveLast(predicate);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> RemoveAll(Predicate<T> predicate)
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.RemoveAll(predicate);

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IInvariantList<T> Clear()
    {
        var clone = (InvariantList<T>)Clone();
        var done = clone.Items.Clear();

        if (done > 0) clone.Trim();
        return done > 0 ? clone : this;
    }
}