namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{K, T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(IInvariantList<,>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public abstract partial class InvariantList<K, T> : IInvariantList<K, T>
{
    /// <summary>
    /// The underlying builder used by this instance.
    /// </summary>
    protected abstract ICoreList<K, T> Items { get; }

    // ----------------------------------------------------

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
        Items.AddRange(range.ThrowWhenNull());
        Items.Trim();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantList(InvariantList<T> source)
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
    public virtual ICoreList<K, T> ToBuilder() => Items.Clone();

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
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(K key) => Items.Contains(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(K key) => Items.IndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(K key) => Items.LastIndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(K key) => Items.IndexesOf(key);

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
    public bool FindLast(
        Predicate<T> predicate, Action<T>? found = null) => Items.FindLast(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindLast(
        Predicate<T> predicate, out T found) => Items.FindLast(predicate, out found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, Action<T>? found = null) => Items.FindAll(predicate, found);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(
        Predicate<T> predicate, out List<T> found) => Items.FindAll(predicate, out found);

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
    public virtual IInvariantList<K, T> Add(T item)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> AddRange(IEnumerable<T> range)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Insert(int index, T item)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Insert(index, item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> GetRange(int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Items.Count - index);

        if (index == 0 && count == Count) return this;

        var range = Items.ToList(index, count);
        var clone = (InvariantList<K, T>)Clone();
        clone.Items.Clear();

        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Replace(int index, T item, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Replace(index, item, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Replace(int index, T item, out T removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Replace(index, item, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAt(int index, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var done = clone.Items.RemoveAt(index, removed);
        return done ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAt(int index, out T removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var done = clone.Items.RemoveAt(index, out removed);
        return done ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveRange(int index, int count, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveRange(index, count, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveRange(int index, int count, out List<T> removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveRange(index, count, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(K key, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Remove(key, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(K key, out T removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Remove(key, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(K key, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveLast(key, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(K key, out T removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveLast(key, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(K key, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveAll(key, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(K key, out List<T> removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveAll(key, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(Predicate<T> predicate, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Remove(predicate, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Remove(Predicate<T> predicate, out T removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Remove(predicate, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(Predicate<T> predicate, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveLast(predicate, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveLast(Predicate<T> predicate, out T removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveLast(predicate, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(Predicate<T> predicate, Action<T>? removed = null)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveAll(predicate, removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual IInvariantList<K, T> RemoveAll(Predicate<T> predicate, out List<T> removed)
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.RemoveAll(predicate, out removed);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<K, T> Clear()
    {
        var clone = (InvariantList<K, T>)Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }

    // ----------------------------------------------------

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => Items.SyncRoot;
}