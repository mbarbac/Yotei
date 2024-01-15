using TItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameterList"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(DebugCount)}")]
[Cloneable]
public sealed partial class ParameterList : IParameterList
{
    /// <summary>
    /// The builder type for instances of the host one.
    /// </summary>
    /// <param name="engine"></param>
    [Cloneable]
    sealed partial class Builder(IEngine engine) : Tools.Code.CoreList<TKey, TItem>
    {
        readonly IEngine Engine = engine.ThrowWhenNull();
        Builder(Builder source) : this(source.Engine) => AddRange(source);

        protected override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        protected override TKey GetKey(TItem item) => item.ThrowWhenNull().Name;
        protected override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();
        protected override bool CompareKeys(TKey source, TKey item)
            => string.Compare(source, item, !Engine.CaseSensitiveNames) == 0;
        protected override bool SameItem(TItem source, TItem item)
            => ReferenceEquals(source, item);
        protected override List<int> GetDuplicates(TKey key) => base.GetDuplicates(key);
        protected override bool AcceptDuplicate(TItem source, TItem item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="engine"></param>
    /// <returns></returns>
    public static Tools.ICoreList<TKey, TItem> CreateBuilder(IEngine engine) => new Builder(engine);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    readonly Tools.ICoreList<TKey, TItem> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public ParameterList(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Items = CreateBuilder(engine)
            ?? throw new UnExpectedException("Cannot create a builder for this type.");
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, TItem item) : this(engine) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(IEngine engine, IEnumerable<TItem> range)
        : this(engine)
        => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterList(ParameterList source) : this(source.Engine) => AddRangeInternal(source);

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

    string ToDebugString(int num) => Count == 0 ? "0:[]" : (Count <= num
        ? $"[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"[{string.Join(", ", Items.Take(num).Select(ItemToString))}, ...]");

    string ItemToString(TItem item) => item?.ToString() ?? string.Empty;
    static int DebugCount => 6;

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
    public IParameterList GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
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
    public IParameterList Replace(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, TItem item) => Items.Replace(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList Add(TItem item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    int AddInternal(TItem item) => Items.Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IParameterList AddRange(IEnumerable<TItem> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<TItem> range) => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList Insert(int index, TItem item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, TItem item) => Items.Insert(index, item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IParameterList InsertRange(int index, IEnumerable<TItem> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<TItem> range) => Items.InsertRange(index, range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IParameterList RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    int RemoveAtInternal(int index) => Items.RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IParameterList RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IParameterList Remove(TKey key)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(key);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(TKey key) => Items.Remove(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IParameterList RemoveLast(TKey key)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(key);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(TKey key) => Items.RemoveLast(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IParameterList RemoveAll(TKey key)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(key);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(TKey key) => Items.RemoveAll(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IParameterList Remove(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<TItem> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IParameterList RemoveLast(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<TItem> predicate) => Items.RemoveLast(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IParameterList RemoveAll(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<TItem> predicate) => Items.RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IParameterList Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal() => Items.Clear();
}