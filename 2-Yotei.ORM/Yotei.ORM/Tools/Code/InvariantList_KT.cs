namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{TKey, TItem}"/>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
[DebuggerDisplay("{ToString(7)}")]
[Cloneable]
public abstract partial class InvariantList<TKey, TItem> : IInvariantList<TKey, TItem>
{
    protected class InnerList(InvariantList<TKey, TItem> master) : CoreList<TKey, TItem>
    {
        public InvariantList<TKey, TItem> Master { get; } = master.ThrowWhenNull();
        public override TItem ValidateItem(TItem item) => Master.ValidateItem(item);
        public override bool CanDuplicate(TItem source, TItem target) => Master.CanDuplicate(source, target);
        public override TKey GetKey(TItem item) => Master.GetKey(item);
        public override TKey ValidateKey(TKey key) => Master.ValidateKey(key);
        public override bool Compare(TKey source, TKey target) => Master.Compare(source, target);
    }
    protected virtual InnerList CreateItems(InvariantList<TKey, TItem> master) => new(master);
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TItem ValidateItem(TItem item);

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
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TKey GetKey(TItem key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract TKey ValidateKey(TKey key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool Compare(TKey source, TKey target);

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
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TKey key) => Items.Contains(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(TKey key) => Items.IndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(TKey key) => Items.LastIndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TKey key) => Items.IndexesOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => Items.Contains(predicate);

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
    public virtual IInvariantList<TKey, TItem>  GetRange(int index, int count)
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
    public virtual IInvariantList<TKey, TItem>  Replace(int index, TItem item)
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
    public virtual IInvariantList<TKey, TItem>  Add(TItem item)
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
    public virtual IInvariantList<TKey, TItem>  AddRange(IEnumerable<TItem> range)
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
    public virtual IInvariantList<TKey, TItem>  Insert(int index, TItem item)
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
    public virtual IInvariantList<TKey, TItem>  InsertRange(int index, IEnumerable<TItem> range)
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
    public virtual IInvariantList<TKey, TItem>  RemoveAt(int index)
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
    public virtual IInvariantList<TKey, TItem>  RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.RemoveRangeInternal(index, count);
        return done > 0 ? temp : this;
    }
    protected int RemoveRangeInternal(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IInvariantList<TKey, TItem>  Remove(TKey key)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(key);
        return done > 0 ? temp : this;
    }
    protected int RemoveInternal(TKey key) => Items.Remove(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IInvariantList<TKey, TItem>  RemoveLast(TKey key)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(key);
        return done > 0 ? temp : this;
    }
    protected int RemoveLastInternal(TKey key) => Items.RemoveLast(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IInvariantList<TKey, TItem>  RemoveAll(TKey key)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(key);
        return done > 0 ? temp : this;
    }
    protected int RemoveAllInternal(TKey key) => Items.RemoveAll(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<TKey, TItem>  Remove(Predicate<TItem> predicate)
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
    public virtual IInvariantList<TKey, TItem>  RemoveLast(Predicate<TItem> predicate)
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
    public virtual IInvariantList<TKey, TItem>  RemoveAll(Predicate<TItem> predicate)
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
    public virtual IInvariantList<TKey, TItem>  Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done > 0 ? temp : this;
    }
    protected int ClearInternal() => Items.Clear();
}