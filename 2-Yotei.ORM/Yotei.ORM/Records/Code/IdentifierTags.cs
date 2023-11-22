using TMaster = Yotei.ORM.Code.IdentifierTags;
using THost = Yotei.ORM.IIdentifierTags;
using TItem = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
[WithGenerator]
public sealed partial class IdentifierTags : THost
{
    class InnerList : CoreList<TItem>
    {
        public InnerList(TMaster master)
        {
            Master = master.ThrowWhenNull();
            ValidateItem = (item, _) =>
            {
                item = item.NotNullNotEmpty();
                if (item.Contains('.')) throw new ArgumentException(
                    "Identifier tags cannot contain dots.")
                    .WithData(item);
                return item;
            };
            Compare = (source, target)
                => string.Compare(source, target, !Master.CaseSensitiveTags) == 0;
            IsSame = (source, target) => source == target;
            ValidDuplicate = (source, target) => IsSame(source, target)
                ? true
                : throw new DuplicateException("Duplicated element.").WithData(target);
        }
        public TMaster Master { get; }
    }
    static InnerList CreateItems(TMaster master) => new(master);
    InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public IdentifierTags(bool sensitive)
    {
        Items = CreateItems(this);
        CaseSensitiveTags = sensitive;
    }

    /// <summary>
    /// Initializes a new instance with the tags obtained from the given dotted values.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="dotted"></param>
    public IdentifierTags(bool sensitive, TItem dotted) : this(sensitive) => AddInternal(dotted);

    /// <summary>
    /// Initializes a new instance with the tags obtained from the dotted values of the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="dottedrange"></param>
    public IdentifierTags(
        bool sensitive, IEnumerable<TItem> dottedrange) : this(sensitive) => AddRangeInternal(dottedrange);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(
        TMaster source) : this(source.CaseSensitiveTags) => AddRangeInternal(source.Items);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(THost? other)
    {
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
            if (!Items.Compare(this[i], other[i])) return false;

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as InvariantFake);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = CaseSensitiveTags.GetHashCode();
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
    public override string ToString() => string.Join('.', Items);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveTags
    {
        get => _CaseSensitiveTags;
        init
        {
            if (_CaseSensitiveTags == value) return;
            _CaseSensitiveTags = value;

            if (Count == 0) return;
            var range = ToArray();
            ClearInternal();
            AddRangeInternal(range);
        }
    }
    bool _CaseSensitiveTags = Engine.CASESENSITIVETAGS;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

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
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(TItem tag) => Items.Contains(tag);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int IndexOf(TItem tag) => Items.IndexOf(tag);

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
    public THost GetRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.GetRangeInternal(index, count);
        return done > 0 ? clone : this;
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
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Replace(int index, TItem dotted)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, dotted);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, TItem dotted)
    {
        dotted = dotted.NotNullNotEmpty();

        var source = Items[index];
        if (Items.IsSame(source, dotted)) return 0;

        RemoveAtInternal(index);
        var num = 0;
        var tags = dotted.Split('.'); foreach (var tag in tags)
        {
            var r = Items.Insert(index, tag);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Add(TItem dotted)
    {
        var clone = Clone();
        var done = clone.AddInternal(dotted);
        return done > 0 ? clone : this;
    }
    int AddInternal(TItem dotted)
    {
        dotted = dotted.NotNullNotEmpty();

        var num = 0;
        var tags = dotted.Split('.'); foreach (var tag in tags)
        {
            var r = Items.Add(tag);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<TItem> dottedrange)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(dottedrange);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<TItem> dottedrange)
    {
        dottedrange.ThrowWhenNull();

        var num = 0; foreach (var item in dottedrange)
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
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Insert(int index, TItem dotted)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, dotted);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, TItem dotted)
    {
        dotted = dotted.NotNullNotEmpty();

        var num = 0;
        var tags = dotted.Split('.'); foreach (var tag in tags)
        {
            var r = Items.Insert(index, tag);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    public THost InsertRange(int index, IEnumerable<TItem> dottedrange)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, dottedrange);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<TItem> dottedrange)
    {
        dottedrange.ThrowWhenNull();

        var num = 0; foreach (var item in dottedrange)
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
    public THost RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    int RemoveAtInternal(int index) => Items.RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public THost RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.RemoveRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public THost Remove(TItem tag)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(tag);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(TItem tag) => Items.Remove(tag);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost Remove(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<TItem> predicate) => Items.Remove(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveLast(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<TItem> predicate) => Items.RemoveLast(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveAll(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<TItem> predicate) => Items.RemoveAll(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public THost Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    int ClearInternal() => Items.Clear();
}