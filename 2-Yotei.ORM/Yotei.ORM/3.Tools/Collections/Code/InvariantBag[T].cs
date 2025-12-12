namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(IInvariantBag<>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public abstract partial class InvariantBag<T> : IInvariantBag<T>
{
    protected abstract ICoreBag<T> Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantBag() { }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantBag(IEnumerable<T> range)
    {
        Items.AddRange(range.ThrowWhenNull());
        Items.Trim();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantBag(InvariantBag<T> source)
    {
        Items.AddRange(source.ThrowWhenNull());
        Items.Trim();
    }

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

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes with at most
    /// the requested number of elements.
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
    /// Invoked to obtain a string representation of the given element suitable for debug
    /// purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ICoreBag<T> ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(
        Predicate<T> predicate, Action<T>? found = null) => Items.Find(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(
        Predicate<T> predicate, out T found) => Items.Find(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, Action<T>? found = null) => FindAll(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, out List<T> found) => FindAll(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index) => Items.CopyTo(array, index);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Add(T item)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> AddRange(IEnumerable<T> range)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(T item, Action<T>? removed = null)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.Remove(item, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(T item, out List<T> removed)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.Remove(item, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(T item, Action<T>? removed = null)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.RemoveAll(item, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(T item, out List<T> removed)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.RemoveAll(item, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(Predicate<T> predicate, Action<T>? removed = null)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.Remove(predicate, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> Remove(Predicate<T> predicate, out T removed)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.Remove(predicate, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.RemoveAll(predicate, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantBag<T> RemoveAll(Predicate<T> predicate, out List<T> removed)
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.RemoveAll(predicate, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantBag<T> Clear()
    {
        var clone = (InvariantBag<T>)Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => Items.SyncRoot;
}