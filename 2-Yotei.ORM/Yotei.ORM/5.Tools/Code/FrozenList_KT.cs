namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <inheritdoc cref="IFrozenList{TKey, TItem}"/>
[DebuggerDisplay("{Items.ToDebugString(6)}")]
[Cloneable]
public abstract partial class FrozenList<TKey, TItem> : IFrozenList<TKey, TItem>
{
    /// <summary>
    /// The repository of elements of this instance.
    /// </summary>
    protected abstract ICoreList<TKey, TItem> Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public FrozenList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public FrozenList(TItem item) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public FrozenList(IEnumerable<TItem> range) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenList(FrozenList<TKey, TItem> source) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual ICoreList<TKey, TItem> ToBuilder() => Items.Clone();

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public TItem this[int index] => Items[index];

    /// <inheritdoc/>
    public bool Contains(TKey key) => Items.Contains(key);

    /// <inheritdoc/>
    public int IndexOf(TKey key) => Items.IndexOf(key);

    /// <inheritdoc/>
    public int LastIndexOf(TKey key) => Items.LastIndexOf(key);

    /// <inheritdoc/>
    public List<int> IndexesOf(TKey key) => Items.IndexesOf(key);

    /// <inheritdoc/>
    public bool Contains(Predicate<TItem> predicate) => Items.Contains(predicate);

    /// <inheritdoc/>
    public int IndexOf(Predicate<TItem> predicate) => Items.IndexOf(predicate);

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<TItem> predicate) => Items.LastIndexOf(predicate);

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<TItem> predicate) => Items.IndexesOf(predicate);

    /// <inheritdoc/>
    public TItem[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<TItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem> GetRange(int index, int count)
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
    public virtual IFrozenList<TKey, TItem>  Replace(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  Add(TItem item)
    {
        var clone = Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  AddRange(IEnumerable<TItem> range)
    {
        if (range is ICollection irange && irange.Count == 0) return this;
        if (range is ICollection<TItem> trange && trange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  Insert(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, item);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  InsertRange(int index, IEnumerable<TItem> range)
    {
        if (range is ICollection irange && irange.Count == 0) return this;
        if (range is ICollection<TItem> trange && trange.Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.Items.RemoveAt(index);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  RemoveRange(int index, int count)
    {
        if (count == 0 && index >= 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveRange(index, count);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  Remove(TKey key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  RemoveLast(TKey key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  RemoveAll(TKey key)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(key);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  Remove(Predicate<TItem> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  RemoveLast(Predicate<TItem> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  RemoveAll(Predicate<TItem> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(predicate);
        return num > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public virtual IFrozenList<TKey, TItem>  Clear()
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }
}