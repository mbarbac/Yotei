using THost = Experimental.Yotei.ITemplateList;
using TItem = Experimental.Yotei.ITemplateElement;
using TKey = Experimental.Yotei.ITemplateKey;

namespace Experimental.Yotei;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[DebuggerDisplay("Items.{ToString(6)}")]
[Cloneable]
public partial class TemplateList : THost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public TemplateList(bool sensitive)
    {
        Items = CreateInnerList();
        CaseSensitive = sensitive;
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="item"></param>
    public TemplateList(bool sensitive, TItem item) : this(sensitive) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public TemplateList(
        bool sensitive, IEnumerable<TItem> range) : this(sensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected TemplateList(TemplateList source)
    {
        source.ThrowWhenNull();

        Items = CreateInnerList();
        CaseSensitive = source.CaseSensitive;
        Items.AddRange(source.Items);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(THost? other)
    {
        if (other is null) return false;

        if (CaseSensitive != other.CaseSensitive) return false;
        if (Count != other.Count) return false;
        //for (int i = 0; i < Count; i++)
        //    if (string.Compare(this[i], other[i], !CaseSensitive) != 0) return false;

        return true;
    }

    /// <summary>
    /// Determines if this object is the same as the other given one.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as THost);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = HashCode.Combine(CaseSensitive);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }

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

    // ----------------------------------------------------

    /// <summary>
    /// Represents the container of elements in this instance.
    /// </summary>
    [DebuggerDisplay("{ToString(6)}")]
    protected class InnerList(TemplateList master) : CoreList<TKey, TItem>
    {
        TemplateList Master { get; } = master.ThrowWhenNull();
        public override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        public override bool AcceptDuplicate(int index, TItem item)
        {
            return this[index].Equals(item)
                ? true
                : throw new DuplicateException("Element is duplicated.").WithData(item);
        }
        public override TKey GetKey(TItem item) => item.Key;
        public override TKey ValidateKey(TKey key) => key;
        public override bool CompareKeys(TKey source, TKey target)
            => string.Compare(source.Value, target.Value, !Master.CaseSensitive) == 0;
    }

    /// <summary>
    /// Invoked to create the container of elements of this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The actual collection of elements in this instance.
    /// </summary>
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitive
    {
        get => _CaseSensitive;
        init
        {
            if (_CaseSensitive == value) return;
            _CaseSensitive = value;

            if (Count == 0) return;

            var range = Items.ToArray();
            Items.Clear();
            Items.AddRange(range);
        }
    }
    bool _CaseSensitive;

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
    public virtual THost GetRange(int index, int count)
    {
        if (count == Count && index == 0) return this;
        if (count == 0)
        {
            if (Count == 0) return this;
            return Clear();
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
    public virtual THost Replace(int index, TItem item)
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
    public virtual THost Add(TItem item)
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
    public virtual THost AddRange(IEnumerable<TItem> range)
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
    public virtual THost Insert(int index, TItem item)
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
    public virtual THost InsertRange(int index, IEnumerable<TItem> range)
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
    public virtual THost RemoveAt(int index)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAt(index);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual THost RemoveRange(int index, int count)
    {
        if (count == 0) return this;

        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual THost Remove(TKey key)
    {
        var temp = Clone();
        var num = temp.Items.Remove(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(TKey key)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(TKey key)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost Remove(Predicate<TItem> predicate)
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
    public virtual THost RemoveLast(Predicate<TItem> predicate)
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
    public virtual THost RemoveAll(Predicate<TItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual THost Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }
}