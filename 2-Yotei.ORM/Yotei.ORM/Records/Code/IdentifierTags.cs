using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = string;
using IKey = string;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[Cloneable(Specs = "(source)")]
[WithGenerator(Specs = "(source)+@")]
public partial class IdentifierTags : IHost
{
    // Represents the collection of contents of this instance.
    protected class InnerList : CoreList<IItem, IKey>
    {
        IdentifierTags Master;
        public InnerList(IdentifierTags master) => Master = master.ThrowWhenNull();
        protected InnerList(InnerList source) : this(source.Master) => AddRange(source);
        public override InnerList Clone() => new(this);

        public override IItem ValidateItem(IItem item) => ValidateKey(item);
        public override string GetKey(IItem item) => item;
        public override IKey ValidateKey(IKey key)
        {
            key.NotNullNotEmpty();

            if (key.Contains('.')) throw new ArgumentException(
                "Identifier tags cannot contain embedded dots.")
                .WithData(key);

            return key;
        }
        public override bool CompareKeys(string inner, string other)
        {
            return string.Compare(inner, other, !Master.CaseSensitiveTags) == 0;
        }
        public override bool AcceptDuplicated(IItem item)
        {
            throw new DuplicateException("Duplicated element.").WithData(item).WithData(Master);
        }
        public override bool ExpandNested(IItem _) => false;
    }

    // The actual contents carried by this instance.
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public IdentifierTags(bool caseSensitiveTags)
    {
        Items = new(this);
        CaseSensitiveTags = caseSensitiveTags;
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="item"></param>
    public IdentifierTags(bool caseSensitiveTags, IItem item)
        : this(caseSensitiveTags)
        => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with elements from the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public IdentifierTags(bool caseSensitiveTags, IEnumerable<IItem> range)
        : this(caseSensitiveTags)
        => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierTags(IdentifierTags source)
    {
        source.ThrowWhenNull();

        Items = new(this);
        CaseSensitiveTags = source.CaseSensitiveTags;
        Items.AddRange(source);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IItem> GetEnumerator() => Items.GetEnumerator();
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

            if (Count > 0)
            {
                var range = ToArray();
                Items.Clear();
                Items.AddRange(range);
            }
        }
    }
    bool _CaseSensitiveTags = false;

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
    public IItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(IKey key) => Items.Contains(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(IKey key) => Items.IndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IItem> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<IItem> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<IItem> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Splits the given key into its dot-separated parts. Empty or missed elements are not
    /// allowed and throw an exception.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string[] SplitKey(string key)
    {
        key.ThrowWhenNull();

        if (key.Contains('.'))
        {
            var parts = key.Split('.'); for (int i = 0; i < parts.Length; i++)
            {
                var part = Items.ValidateKey(parts[i]);
                parts[i] = part;
            }
            return parts;
        }
        else
        {
            key = Items.ValidateKey(key);
            return [key];
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost GetRange(int index, int count)
    {
        if (count == Count && index == 0) return this;
        if (count == 0) return Clear();

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
    public virtual IHost Replace(int index, IItem item)
    {
        var temp = Clone();
        var num = temp.ReplaceInternal(index, item);
        return num > 0 ? temp : this;
    }
    int ReplaceInternal(int index, IItem item)
    {
        Items.RemoveAt(index);

        var parts = SplitKey(item);
        return InsertRangeInternal(index, parts);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(IItem item)
    {
        var temp = Clone();
        var num = temp.AddInternal(item);
        return num > 0 ? temp : this;
    }
    int AddInternal(IItem item)
    {
        var parts = SplitKey(item);
        var count = 0;

        foreach (var part in parts)
        {
            var num = Items.Add(part);
            count += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<IItem> range)
    {
        var temp = Clone();
        var num = temp.AddRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<IItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range) num += AddInternal(item);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Insert(int index, IItem item)
    {
        var temp = Clone();
        var num = temp.InsertInternal(index, item);
        return num > 0 ? temp : this;
    }
    int InsertInternal(int index, IItem item)
    {
        var parts = SplitKey(item);
        var count = 0;

        foreach (var part in parts)
        {
            var num = Items.Insert(index, part);
            count += num;
            index += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertRange(int index, IEnumerable<IItem> range)
    {
        var temp = Clone();
        var num = temp.InsertRangeInternal(index, range);
        return num > 0 ? temp : this;
    }
    int InsertRangeInternal(int index, IEnumerable<IItem> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range)
        {
            var num = InsertInternal(index, item);
            count += num;
            index += num;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IHost RemoveAt(int index)
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
    public virtual IHost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IHost Remove(IKey key)
    {
        var temp = Clone();
        var num = temp.Items.Remove(key);
        return num > 0 ? temp : this;
    }

    ////// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost Remove(Predicate<IItem> predicate)
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
    public virtual IHost RemoveLast(Predicate<IItem> predicate)
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
    public virtual IHost RemoveAll(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }
}