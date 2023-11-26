using THost = Yotei.ORM.Records.IIdentifierTags;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
[WithGenerator]
public sealed partial class IdentifierTags : THost
{
    readonly List<string> Items = [];
    static string ValidateItem(string item, bool _)
    {
        item = item.NotNullNotEmpty();
        if (item.Contains('.')) throw new ArgumentException(
            "Identifier tags cannot contain dots.")
            .WithData(item);
        return item;
    }
    bool SameItem(string source, string target) => source == target;
    bool AllowDuplicate(string source, string target)
        => SameItem(source, target)
        ? true
        : throw new DuplicateException("Duplicated element.").WithData(target);
    List<int> GetDuplicates(string item) => IndexesOf(x => CompareItems(x, item));
    bool CompareItems(string source, string target)
        => string.Compare(source, target, !CaseSensitiveTags) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitive"></param>
    public IdentifierTags(bool caseSensitive) => CaseSensitiveTags = caseSensitive;

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="item"></param>
    public IdentifierTags(
        bool caseSensitive, string item) : this(caseSensitive) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="range"></param>
    public IdentifierTags(bool caseSensitive, IEnumerable<string> range)
        : this(caseSensitive)
        => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(
        IdentifierTags source) : this(source.CaseSensitiveTags) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IIdentifierTags? other)
    {
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++) if (!CompareItems(this[i], other[i])) return false;

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
        var code = CaseSensitiveTags.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
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
            var range = Items.ToArray();
            ClearInternal();
            AddRangeInternal(range);
        }
    }
    bool _CaseSensitiveTags = Engine.CASESENSITIVETAGS;

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
    public string this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => IndexOf(tag) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int IndexOf(string tag)
    {
        tag = ValidateItem(tag, false);
        return IndexOf(x => CompareItems(x, tag));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<string> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<string> predicate)
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
    public int LastIndexOf(Predicate<string> predicate)
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
    public List<int> IndexesOf(Predicate<string> predicate)
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
    public string[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<string> ToList() => Items.ToList();

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
    public THost Replace(int index, string dotted)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, dotted);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, string dotted)
    {
        dotted = dotted.NotNullNotEmpty();

        var source = Items[index];
        if (SameItem(source, dotted)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, dotted);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Add(string dotted)
    {
        var clone = Clone();
        var done = clone.AddInternal(dotted);
        return done > 0 ? clone : this;
    }
    int AddInternal(string dotted)
    {
        dotted = dotted.NotNullNotEmpty();

        var num = 0;
        var tags = dotted.Split('.');
        foreach (var tag in tags)
        {
            var r = AddInternalSingle(tag);
            num += r;
        }
        return num;
    }
    int AddInternalSingle(string item)
    {
        item = ValidateItem(item, true);

        var nums = GetDuplicates(item);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], item)) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<string> dottedrange)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(dottedrange);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string> range)
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
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Insert(int index, string dotted)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, dotted);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, string dotted)
    {
        dotted = dotted.NotNullNotEmpty();

        var num = 0;
        var tags = dotted.Split('.');
        foreach (var tag in tags)
        {
            var r = InsertInternalSingle(index, tag);
            num += r;
            index += r;
        }
        return num;
    }
    int InsertInternalSingle(int index, string item)
    {
        item = ValidateItem(item, true);

        var nums = GetDuplicates(item);
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
    public THost InsertRange(int index, IEnumerable<string> dottedrange)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, dottedrange);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string> range)
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
    /// <param name="index"></param>
    /// <returns></returns>
    public THost RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
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
    public THost RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.RemoveRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public THost Remove(string tag)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(tag);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string tag)
    {
        var index = IndexOf(tag);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost Remove(Predicate<string> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<string> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveLast(Predicate<string> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<string> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveAll(Predicate<string> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<string> predicate)
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
    public THost Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}