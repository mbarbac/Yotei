namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{TItem}"/>
/// </summary>
/// <typeparam name="TItem"></typeparam>
[DebuggerDisplay("{ToString(7)}")]
[Cloneable]
public abstract partial class InvariantList<TItem> : IInvariantList<TItem>
{
    protected class InnerList(InvariantList<TItem> master) : CoreList<TItem>
    {
        public InvariantList<TItem> Master { get; } = master.ThrowWhenNull();
        public override TItem Validate(TItem item) => Master.Validate(item);
        public override bool CanDuplicate(TItem source, TItem target) => Master.CanDuplicate(source, target);
        public override bool Compare(TItem source, TItem target) => Master.Compare(source, target);
    }
    protected virtual InnerList CreateItems(InvariantList<TItem> master) => new(master);
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty list.
    /// </summary>
    public InvariantList() => Items = CreateItems(this);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(TItem item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<TItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a string representation of this collection containing at most the given number
    /// of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected string ToString(int count)
    {
        var items = count <= Items.Count ? Items : Items.Take(count);
        var temp = string.Join(", ", items);
        return $"({Count})[{temp}]";
    }

    // ----------------------------------------------------

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TItem Validate(TItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool CanDuplicate(TItem source, TItem target);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool Compare(TItem source, TItem target);

    // ----------------------------------------------------

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
    public TItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(TItem item) => Items.IndexOf(item) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(TItem item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(TItem item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TItem item) => Items.IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => Items.IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<TItem> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<TItem> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> GetRange(int index, int count)
    {
        if (count == 0 && index >= 0) return Clear();
        if (index == 0 && count == Count) return this;

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
    public virtual IInvariantList<TItem> Replace(int index, TItem item)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(index, item);
        return done > 0 ? temp : this;
    }
    protected int ReplaceInternal(int index, TItem item) => Items.Replace(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> Add(TItem item)
    {
        var temp = Clone();
        var done = temp.AddInternal(item);
        return done > 0 ? temp : this;
    }
    protected int AddInternal(TItem item) => Items.Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> AddRange(IEnumerable<TItem> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done > 0 ? temp : this;
    }
    protected int AddRangeInternal(IEnumerable<TItem> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> Insert(int index, TItem item)
    {
        var temp = Clone();
        var done = temp.InsertInternal(index, item);
        return done > 0 ? temp : this;
    }
    protected int InsertInternal(int index, TItem item) => Items.Insert(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> InsertRange(int index, IEnumerable<TItem> range)
    {
        var temp = Clone();
        var done = temp.InsertRangeInternal(index, range);
        return done > 0 ? temp : this;
    }
    protected int InsertRangeInternal(
        int index, IEnumerable<TItem> range) => Items.InsertRange(index, range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.RemoveAtInternal(index);
        return done > 0 ? temp : this;
    }
    protected int RemoveAtInternal(int index) => Items.RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.RemoveRangeInternal(index, count);
        return done > 0 ? temp : this;
    }
    protected int RemoveRangeInternal(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> Remove(TItem item)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(item);
        return done > 0 ? temp : this;
    }
    protected int RemoveInternal(TItem item) => Items.Remove(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> RemoveLast(TItem item)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(item);
        return done > 0 ? temp : this;
    }
    protected int RemoveLastInternal(TItem item) => Items.RemoveLast(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> RemoveAll(TItem item)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(item);
        return done > 0 ? temp : this;
    }
    protected int RemoveAllInternal(TItem item) => Items.RemoveAll(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> Remove(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(predicate);
        return done > 0 ? temp : this;
    }
    protected int RemoveInternal(Predicate<TItem> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> RemoveLast(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(predicate);
        return done > 0 ? temp : this;
    }
    protected int RemoveLastInternal(Predicate<TItem> predicate) => Items.RemoveLast(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<TItem> RemoveAll(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(predicate);
        return done > 0 ? temp : this;
    }
    protected int RemoveAllInternal(Predicate<TItem> predicate) => Items.RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<TItem> Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done > 0 ? temp : this;
    }
    protected int ClearInternal() => Items.Clear();
}