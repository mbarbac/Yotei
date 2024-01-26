namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IFrozenList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public abstract partial class FrozenList<T> : IFrozenList<T>
{
    protected abstract ICoreList<T> Items { get; }

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
    protected FrozenList(CoreList<T> source) => Items.AddRange(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count) => Items.ToDebugString(count);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICoreList<T> ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item) => Items.IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (count == 0 && index >= 0) return Clear();

        var range = Items.ToList().GetRange(index, count);
        var clone = Clone();
        clone.Items.Clear();
        clone.Items.AddRange(range);
        return clone;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> Replace(int index, T item)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> Add(T item)
    {
        var clone = Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> AddRange(IEnumerable<T> range)
    {
        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;        

        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> Insert(int index, T item)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> InsertRange(int index, IEnumerable<T> range)
    {
        if (range is ICollection<T> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;        

        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> RemoveAt(int index)
    {
        if (index < 0 || index >= Items.Count) throw new IndexOutOfRangeException("Index out of range.").WithData(index);

        var clone = Clone();
        var num = clone.Items.RemoveAt(index);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> RemoveRange(int index, int count)
    {
        if (count < 0) throw new ArgumentException("Count is less than cero.").WithData(count);
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index less than cero or bigger then count.")
            .WithData(index);

        if (count > 0)
        {
            var clone = Clone();
            var num = clone.Items.RemoveRange(index, count);
            if (num > 0) return clone;
        }
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> Remove(T item)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> RemoveLast(T item)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> RemoveAll(T item)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> Remove(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Remove(predicate);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveLast(predicate);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenList<T> RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.RemoveAll(predicate);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IFrozenList<T> Clear()
    {
        if (Count == 0) return this;

        var clone = Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }
}