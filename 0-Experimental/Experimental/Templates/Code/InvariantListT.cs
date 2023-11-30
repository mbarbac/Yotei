using TMaster = Experimental.Templates.Code.InvariantListT;
using THost = Experimental.Templates.IInvariantListT;
using TItem = Experimental.Templates.IInvariantElement;

namespace Experimental.Templates.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[DebuggerDisplay("{ToStringEx(7)}")]
[Cloneable]
[WithGenerator]
public partial class InvariantListT : THost
{
    readonly List<TItem> Items = [];
    protected virtual TItem ValidateItem(TItem item, bool add)
    {
        item.ThrowWhenNull();
        if (add) item.Name.NotNullNotEmpty();
        return item;
    }
    protected virtual bool SameItem(TItem source, TItem target)
        => ReferenceEquals(source, target);
    protected virtual List<int> FindDuplicates(TItem item) => IndexesOf(item);
    protected virtual bool AllowDuplicate(TItem source, TItem target)
        => SameItem(source, target)
        ? true
        : throw new DuplicateException("Duplicated element.").WithData(target);
    protected bool CompareItems(TItem source, TItem target)
        => string.Compare(source.Name, target.Name, !CaseSensitive) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public InvariantListT(bool sensitive) => CaseSensitive = sensitive;

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="item"></param>
    public InvariantListT(bool sensitive, TItem item) : this(sensitive) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the element of the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public InvariantListT(
        bool sensitive, IEnumerable<TItem> range) : this(sensitive) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantListT(
        TMaster source) : this(source.CaseSensitive) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(THost? other)
    {
        if (other is null) return false;

        if (CaseSensitive != other.CaseSensitive) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
            if (!CompareItems(this[i], other[i])) return false; // Override as needed!

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
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
        var code = CaseSensitive.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    protected virtual string ToStringEx(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ToStringEx))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ToStringEx))}]...";

    protected virtual string ToStringEx(TItem item) => item?.ToString() ?? string.Empty;

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
            ClearInternal();
            AddRangeInternal(range);
        }
    }
    bool _CaseSensitive;

    // ----------------------------------------------------

    /// <summary>
    /// Minimizes memory consumption.
    /// </summary>
    public void Trim() => Items.TrimExcess();

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
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(TItem item) => IndexOf(item) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(TItem item)
    {
        item = ValidateItem(item, false);
        return IndexOf(x => CompareItems(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(TItem item)
    {
        item = ValidateItem(item, false);
        return LastIndexOf(x => CompareItems(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TItem item)
    {
        item = ValidateItem(item, false);
        return IndexesOf(x => CompareItems(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

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
        var clone = Clone();
        var done = clone.GetRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    protected virtual int GetRangeInternal(int index, int count)
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
    public virtual THost Replace(int index, TItem item)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, item);
        return done > 0 ? clone : this;
    }
    protected virtual int ReplaceInternal(int index, TItem item)
    {
        item = ValidateItem(item, true);

        var source = Items[index];
        if (SameItem(source, item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Add(TItem item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int AddInternal(TItem item)
    {
        item = ValidateItem(item, true);

        var nums = FindDuplicates(item);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], item)) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost AddRange(IEnumerable<TItem> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    protected virtual int AddRangeInternal(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Insert(int index, TItem item)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, item);
        return done > 0 ? clone : this;
    }
    protected virtual int InsertInternal(int index, TItem item)
    {
        item = ValidateItem(item, true);

        var nums = FindDuplicates(item);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], item)) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost InsertRange(int index, IEnumerable<TItem> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    protected virtual int InsertRangeInternal(int index, IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual THost RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual THost RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.RemoveRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Remove(TItem item)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(TItem item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(TItem item)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveLastInternal(TItem item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(TItem item)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveAllInternal(TItem item)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost Remove(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(Predicate<TItem> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveLastInternal(Predicate<TItem> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveAllInternal(Predicate<TItem> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual THost Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    protected virtual int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}