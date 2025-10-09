namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements, identified by their respective
/// keys, with customizable behavior.
/// </summary>
/// <typeparam name="K">The type of the keys used to identify elements.</typeparam>
/// <typeparam name="T">The type of the elements in this collection.</typeparam>
[DebuggerDisplay("{ToDebugString(3)}")]
public abstract class InvariantList<K, T> : IReadOnlyList<T>, ICloneable
{
    protected abstract CoreList<K, T> Items { get; }

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
    protected InvariantList(InvariantList<K, T> source)
    {
        Items.Capacity = source.ThrowWhenNull().Items.Capacity;
        Items.AddRange(source);
        Items.Trim();
    }

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract InvariantList<K, T> Clone();
    object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a string representation of this instance, suitable for debug purposes, with at
    /// most the given number of elements.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int max)
    {
        if (Count == 0) return "0:[]";
        if (max == 0) return $"{Count}:[...]";

        return Count <= max
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(max).Select(ToDebugItem))}]";
    }

    /// <summary>
    /// Invoked to obtain a debug string representation of the given element.
    /// </summary>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index] => Items[index];

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(K key) => Items.Contains(key);

    /// <summary>
    /// Returns the index of the first element in this collection with the given key, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(K key) => Items.IndexOf(key);

    /// <summary>
    /// Returns the index of the last element in this collection with the given key, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(K key) => Items.LastIndexOf(key);

    /// <summary>
    /// Returns the indexes of all the element in this collection with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(K key) => Items.IndexesOf(key);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => Items.Contains(predicate);

    /// <summary>
    /// Returns the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// Returns the indexes of all the element in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// Returns a new array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => [.. Items];

    /// <summary>
    /// Returns a new list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => [.. Items];

    /// <summary>
    /// Returns a new list with the given number of elements from this collection, starting from
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> ToList(int index, int count) => Items.ToList(index, count);

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance with the given number of elements, starting from the given
    /// index, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (index == 0 && count == 0) return Clear();

        var range = Items.ToList(index, count);
        var clone = Clone();
        clone.Items.Clear();

        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the element at the given index has been replaced
    /// by the given one, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> Replace(int index, T item)
    {
        var clone = Clone();
        var num = clone.Items.Replace(index, item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the given element has been added it, or the
    /// original collection if no changes have been made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> Add(T item)
    {
        var clone = Clone();
        var num = clone.Items.Add(item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where elements from the given range have been added to
    /// it, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var num = clone.Items.AddRange(range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the given element has been inserted into it, at
    /// the given index, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> Insert(int index, T item)
    {
        var clone = Clone();
        var num = clone.Items.Insert(index, item);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where elements from the given range have been inserted
    /// into it, starting at the given index, or the original collection if no changes have been
    /// made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var num = clone.Items.InsertRange(index, range);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the element at the given index has been removed,
    /// or the original collection if no changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.Items.RemoveAt(index);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the given number of elements, starting at the
    /// given index, have been removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.Items.RemoveRange(index, count);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the first element with the given key has been
    /// removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> Remove(K key)
    {
        var clone = Clone();
        var num = clone.Items.Remove(key);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the last element with the given key has been
    /// removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> RemoveLast(K key)
    {
        var clone = Clone();
        var num = clone.Items.RemoveLast(key);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where all the elements with the given key have been
    /// removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> RemoveAll(K key)
    {
        var clone = Clone();
        var num = clone.Items.RemoveAll(key);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the first element that matches the given
    /// predicate has been removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var num = clone.Items.Remove(predicate);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where the last element that matches the given
    /// predicate has been removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var num = clone.Items.RemoveLast(predicate);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where all the elements that matches the given
    /// predicate have been removed, or the original collection if no changes have been made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual InvariantList<K, T> RemoveAll(Predicate<T> predicate)
    {
        var clone = Clone();
        var num = clone.Items.RemoveAll(predicate);
        return num > 0 ? clone : this;
    }

    /// <summary>
    /// Returns either a new instance where where all the elements have been removed, or the
    /// original collection if no changes have been made.
    /// </summary>
    /// <returns></returns>
    public virtual InvariantList<K, T> Clear()
    {
        var clone = Clone();
        var num = clone.Items.Clear();
        return num > 0 ? clone : this;
    }
}