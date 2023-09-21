using System.Collections.Specialized;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class InvariantList<T> : IInvariantList<T>
{
    readonly CoreList<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() => Items = new();

    /// <summary>
    /// Initializes a new instance that carries the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(T item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance that carries the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range) : this() => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual InvariantList<T> Clone()
    {
        var temp = new InvariantList<T>();
        temp.Items.Validator = Validator;
        temp.Items.Comparer = Comparer;
        temp.Items.Behavior = Behavior;
        temp.Items.FlatCollection = FlatCollection;

        temp.Items.AddRange(Items);
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Validator"/>
    /// </summary>
    public Func<T, bool, T> Validator
    {
        get => Items.Validator;
        init => Items.Validator = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public InvariantList<T> WithValidator(Func<T, bool, T> value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (ReferenceEquals(Validator, value)) return this;

        var temp = Clone();
        temp.Items.Validator = value;
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.WithValidator(Func<T, bool, T> value) => WithValidator(value);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Comparer"/>
    /// </summary>
    public Func<T, T, bool> Comparer
    {
        get => Items.Comparer;
        init => Items.Comparer = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public InvariantList<T> WithComparer(Func<T, T, bool> value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (ReferenceEquals(Comparer, value)) return this;

        var temp = Clone();
        temp.Items.Comparer = value;
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.WithComparer(Func<T, T, bool> value) => WithComparer(value);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.Behavior"/>
    /// </summary>
    public CoreListBehavior Behavior
    {
        get => Items.Behavior;
        init => Items.Behavior = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public InvariantList<T> WithBehavior(CoreListBehavior value)
    {
        if (Behavior == value) return this;

        var temp = Clone();
        temp.Items.Behavior = value;
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.WithBehavior(CoreListBehavior value) => WithBehavior(value);

    /// <summary>
    /// <inheritdoc cref="ICoreList{T}.FlatCollection"/>
    /// </summary>
    public bool FlatCollection
    {
        get => Items.FlatCollection;
        init => Items.FlatCollection = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IInvariantList<T> WithFlatCollection(bool value)
    {
        if (FlatCollection == value) return this;

        var temp = Clone();
        temp.Items.FlatCollection = value;
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.WithFlatCollection(bool value) => WithFlatCollection(value);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

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
    public List<T> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual InvariantList<T> GetRange(int index, int count)
    {
        if (count == 0) return this;

        var range = Items.GetRange(index, count);
        var temp = Clone();
        temp.Items.Clear();
        temp.Items.AddRange(range);
        return temp;
    }
    IInvariantList<T> IInvariantList<T>.GetRange(int index, int count) => GetRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<T> ReplaceItem(int index, T item)
    {
        var temp = Clone();
        var done = temp.Items.ReplaceItem(index, item);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.ReplaceItem(int index, T item) => ReplaceItem(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<T> Add(T item)
    {
        var temp = Clone();
        var done = temp.Items.Add(item);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.Add(T item) => Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual InvariantList<T> AddRange(IEnumerable<T> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.AddRange(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<T> Insert(int index, T item)
    {
        var temp = Clone();
        var done = temp.Items.Insert(index, item);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.Insert(int index, T item) => Insert(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual InvariantList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var temp = Clone();
        var done = temp.Items.InsertRange(index, range);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual InvariantList<T> RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAt(index);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<T> Remove(T item)
    {
        var temp = Clone();
        var done = temp.Items.Remove(item);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.Remove(T item) => Remove(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<T> RemoveLast(T item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(item);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.RemoveLast(T item) => RemoveLast(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual InvariantList<T> RemoveAll(T item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(item);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.RemoveAll(T item) => RemoveAll(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual InvariantList<T> RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.Items.RemoveRange(index, count);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.RemoveRange(int index, int count) => RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual InvariantList<T> Remove(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.Items.Remove(predicate);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.Remove(Predicate<T> predicate) => Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual InvariantList<T> RemoveLast(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(predicate);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.RemoveLast(Predicate<T> predicate) => RemoveLast(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual InvariantList<T> RemoveAll(Predicate<T> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(predicate);
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.RemoveAll(Predicate<T> predicate) => RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual InvariantList<T> Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done == 0 ? this : temp;
    }
    IInvariantList<T> IInvariantList<T>.Clear() => Clear();
}