using System.Transactions;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class InvariantList<T> : IInvariantList<T>
{
    /// <summary>
    /// Invoked to create a new internal collection.
    /// </summary>
    /// <returns></returns>
    protected virtual CoreList<T> CreateItems() => new();

    /// <summary>
    /// The internal collection of elements carried by this instance.
    /// </summary>
    protected CoreList<T> Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() => Items = CreateItems() ?? throw new UnExpectedException(
        "Cannot obtain an instance of the internal collection.");

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(T item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// <br/> By default this method copies both the delegates and the original contents from
    /// the source instance. If any delegate depends on the state of its concrete host instance,
    /// then override this method appropriately.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantList(InvariantList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Items = CreateItems() ?? throw new UnExpectedException(
            "Cannot obtain an instance of the internal collection.");

        Items.Validate = source.Items.Validate;
        Items.Compare = source.Items.Compare;
        Items.AcceptDuplicate = source.Items.AcceptDuplicate;
        Items.ExpandNested = source.Items.ExpandNested;

        Items.AddRange(source);
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual InvariantList<T> Clone() => new(this);
    IInvariantList<T> IInvariantList<T>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

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
    public virtual IInvariantList<T> GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (count == 0)
        {
            if (Count == 0) return this;

            var other = Clone(); other.Items.Clear();
            return other;
        }

        var range = Items.GetRange(index, count);
        var temp = Clone();
        temp.Items.Clear();
        temp.Items.AddRange(range);
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Replace(int index, T item)
    {
        var temp = Clone();
        var num = temp.Items.Replace(index, item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Add(T item)
    {
        var temp = Clone();
        var num = temp.Items.Add(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> AddRange(IEnumerable<T> range)
    {
        var temp = Clone();
        var num = temp.Items.AddRange(range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Insert(int index, T item)
    {
        var temp = Clone();
        var num = temp.Items.Insert(index, item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var temp = Clone();
        var num = temp.Items.InsertRange(index, range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAt(int index)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAt(index);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(T item)
    {
        var temp = Clone();
        var num = temp.Items.Remove(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(T item)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(T item)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveRange(int index, int count)
    {
        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(Predicate<T> predicate)
    {
        var temp = Clone();
        var num = temp.Items.Remove(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(Predicate<T> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(Predicate<T> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<T> Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }
}