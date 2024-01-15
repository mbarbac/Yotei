//using TItem = ...;
//using TKey = ...;

namespace Yotei.ORM.Tools.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IFrozenList{TKey, TItem}"/>
/// </summary>
/// <typeparam name="TItem"></typeparam>
[DebuggerDisplay("{ToDebugString(DebugCount)}")]
[Cloneable]
public partial class FrozenList<TKey, TItem> : IFrozenList<TKey, TItem>
{
    /// <summary>
    /// The builder type for instances of the host one.
    /// </summary>
    [Cloneable]
    partial class Builder() : CoreList<TKey, TItem>
    {
        protected Builder(Builder source) : this() => AddRange(source);

        protected override TItem ValidateItem(TItem item) => base.ValidateItem(item);
        protected override TKey GetKey(TItem item) => base.GetKey(item);
        protected override TKey ValidateKey(TKey key) => base.ValidateKey(key);
        protected override bool CompareKeys(TKey source, TKey item) => base.CompareKeys(source, item);
        protected override bool SameItem(TItem source, TItem item) => base.SameItem(source, item);
        protected override List<int> GetDuplicates(TKey key) => base.GetDuplicates(key);
        protected override bool AcceptDuplicate(TItem source, TItem item) => base.AcceptDuplicate(source, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public static Tools.ICoreList<TKey, TItem> CreateBuilder() => new Builder();

    // ----------------------------------------------------

    readonly Tools.ICoreList<TKey, TItem> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public FrozenList() => Items = CreateBuilder()
        ?? throw new UnExpectedException("Cannot create a builder.");

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public FrozenList(TItem item) : this() => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public FrozenList(IEnumerable<TItem> range) : this() => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenList(FrozenList<TKey, TItem> source) => Items = source.Items.Clone();

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

    protected virtual string ToDebugString(int num) => Count == 0 ? "0:[]" : (Count <= num
        ? $"[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"[{string.Join(", ", Items.Take(num).Select(ItemToString))}, ...]");

    protected virtual string ItemToString(TItem item) => item?.ToString() ?? string.Empty;
    protected virtual int DebugCount => 6;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public Tools.ICoreList<TKey, TItem> ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    protected int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        Items.Clear();
        Items.AddRange(range);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> Replace(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    protected int ReplaceInternal(int index, TItem item) => Items.Replace(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> Add(TItem item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    protected int AddInternal(TItem item) => Items.Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> AddRange(IEnumerable<TItem> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    protected int AddRangeInternal(IEnumerable<TItem> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> Insert(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    protected int InsertInternal(int index, TItem item) => Items.Insert(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> InsertRange(int index, IEnumerable<TItem> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    protected int InsertRangeInternal(
        int index, IEnumerable<TItem> range) => Items.InsertRange(index, range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    protected int RemoveAtInternal(int index) => Items.RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    protected int RemoveRangeInternal(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> Remove(TKey key)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(key);
        return num > 0 ? clone : this;
    }
    protected int RemoveInternal(TKey key) => Items.Remove(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> RemoveLast(TKey key)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(key);
        return num > 0 ? clone : this;
    }
    protected int RemoveLastInternal(TKey key) => Items.RemoveLast(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> RemoveAll(TKey key)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(key);
        return num > 0 ? clone : this;
    }
    protected int RemoveAllInternal(TKey key) => Items.RemoveAll(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> Remove(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    protected int RemoveInternal(Predicate<TItem> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> RemoveLast(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    protected int RemoveLastInternal(Predicate<TItem> predicate) => Items.RemoveLast(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> RemoveAll(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    protected int RemoveAllInternal(Predicate<TItem> predicate) => Items.RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IFrozenList<TKey, TItem> Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    protected int ClearInternal() => Items.Clear();
}