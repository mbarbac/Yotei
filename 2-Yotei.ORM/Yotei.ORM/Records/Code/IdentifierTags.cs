using THost = Yotei.ORM.Records.IIdentifierTags;
using TItem = string;
using TKey = string;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
[WithGenerator]
public sealed partial class IdentifierTags : THost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public IdentifierTags(bool caseSensitiveTags)
    {
        Items = CreateInnerList();
        CaseSensitiveTags = caseSensitiveTags;
    }

    /// <summary>
    /// Initializes a new instance with elements obtained from the given value.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="value"></param>
    public IdentifierTags(
        bool caseSensitiveTags, string value) : this(caseSensitiveTags) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the tags obtained from the given range of values.
    /// </summary>
    /// <param name="range"></param>
    public IdentifierTags(bool caseSensitiveTags, IEnumerable<TItem> range)
        : this(caseSensitiveTags)
        => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierTags(IdentifierTags source)
    {
        source.ThrowWhenNull();

        Items = CreateInnerList();
        CaseSensitiveTags = source.CaseSensitiveTags;
        Items.AddRange(source.Items);
    }

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
            if (string.Compare(this[i], other[i], !CaseSensitiveTags) != 0) return false;

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
        var code = HashCode.Combine(CaseSensitiveTags);
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
    /// Represents the container of elements in this instance.
    /// </summary>
    [DebuggerDisplay("{ToString(6)}")]
    class InnerList(IdentifierTags master) : CoreList<TKey, TItem>
    {
        IdentifierTags Master { get; } = master.ThrowWhenNull();
        public override TItem ValidateItem(TItem item) => item.NotNullNotEmpty();
        public override bool AcceptDuplicate(int index, TItem item)
            => throw new DuplicateException("Element is duplicated.").WithData(item);
        public override TKey GetKey(TItem item) => item;
        public override TKey ValidateKey(TKey key)
        {
            if ((key = key.NotNullNotEmpty()).Contains('.')) throw new ArgumentException(
                "Identifier tag cannot contain dots.")
                .WithData(key);
            return key;
        }
        public override bool CompareKeys(TKey source, TKey target)
            => string.Compare(source, target, !Master.CaseSensitiveTags) == 0;

        public override int Replace(int index, string item)
        {
            item = ValidateItem(item);

            var source = Items[index];
            if (CompareKeys(source, item)) return 0;

            var range = ToArray();
            RemoveAt(index);

            var values = ValidateItem(item).Split('.');
            var num = 0; foreach (var value in values)
            {
                var r = base.Insert(index, value);
                num += r;
                index += r;
            }
            if (num == 0)
            {
                Items.Clear();
                Items.AddRange(range);
            }
            return num;
        }
        public override int Add(string item)
        {
            var values = ValidateItem(item).Split('.');
            var num = 0; foreach (var value in values)
            {
                var r = base.Add(value);
                num += r;
            }
            return num;
        }
        public override int Insert(int index, string item)
        {
            var values = ValidateItem(item).Split('.');
            var num = 0; foreach (var value in values)
            {
                var r = base.Insert(index, value);
                num += r;
                index += r;
            }
            return num;
        }
    }

    /// <summary>
    /// Invoked to create the container of elements of this instance.
    /// </summary>
    /// <returns></returns>
    InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The actual collection of elements in this instance.
    /// </summary>
    InnerList Items { get; }

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
            Items.Clear();
            Items.AddRange(range);
        }
    }
    bool _CaseSensitiveTags;

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
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => Items.Contains(tag);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int IndexOf(string tag) => Items.IndexOf(tag);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<string> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<string> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<string> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<string> predicate) => Items.IndexesOf(predicate);

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
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Replace(int index, string value)
    {
        var temp = Clone();
        var num = temp.Items.Replace(index, value);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Add(string value)
    {
        var temp = Clone();
        var num = temp.Items.Add(value);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<string> range)
    {
        var temp = Clone();
        var num = temp.Items.AddRange(range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Insert(int index, string value)
    {
        var temp = Clone();
        var num = temp.Items.Insert(index, value);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost InsertRange(int index, IEnumerable<string> range)
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
    public THost RemoveAt(int index)
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
    public THost RemoveRange(int index, int count)
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
    public THost Remove(string tag)
    {
        var temp = Clone();
        var num = temp.Items.Remove(tag);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost Remove(Predicate<string> predicate)
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
    public THost RemoveLast(Predicate<string> predicate)
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
    public THost RemoveAll(Predicate<string> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public THost Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }
}