using THost = Yotei.ORM.Records.IIdentifierTags;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
[WithGenerator]
public partial class IdentifierTags : THost
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
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="tag"></param>
    public IdentifierTags(
        bool caseSensitiveTags, string tag) : this(caseSensitiveTags) => Items.Add(tag);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public IdentifierTags(
        bool caseSensitiveTags, IEnumerable<string> range) : this(caseSensitiveTags)
        => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierTags(IdentifierTags source)
    {
        source.ThrowWhenNull();

        Items = CreateInnerList();
        CaseSensitiveTags = source.CaseSensitiveTags;
        Items.AddRange(source.Items);
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IIdentifierTags);

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
    public override string ToString() => string.Join('.', Items);

    // ----------------------------------------------------

    /// <summary>
    /// Represents the internal collection of elements in this instance.
    /// </summary>
    protected class InnerList : CoreList<string, string>
    {
        readonly THost Master;
        public InnerList(THost master) => Master = master.ThrowWhenNull();
        public new string ToDebugString() => base.ToDebugString();

        public override string ValidateItem(string item) => item.NotNullNotEmpty();
        public override string GetKey(string item) => item;
        public override string ValidateKey(string key)
        {
            key.NotNullNotEmpty();
            if (key.Contains('.')) throw new ArgumentException(
                "Identifier tags cannot contain embedded dots.").WithData(key);
            return key;
        }
        public override bool CompareKeys(string source, string target)
            => string.Compare(source, target, !Master.CaseSensitiveTags) == 0;
        public override bool AcceptDuplicate(string item)
            => throw new DuplicateException("Duplicated element.").WithData(item);

        public override int Replace(int index, string item)
        {
            item = item.NotNullNotEmpty();
            if (item == this[index]) return 0;

            RemoveAt(index);
            var num = 0;
            var parts = item.Split('.'); foreach (var part in parts)
            {
                var temp = base.Insert(index, ValidateKey(part));
                num += temp;
                index += temp;
            }
            return num;
        }
        public override int Add(string item)
        {
            item = item.NotNullNotEmpty();
            var num = 0;
            var parts = item.Split('.'); foreach (var part in parts)
            {
                var temp = base.Add(ValidateKey(part));
                num += temp;
            }
            return num;
        }
        public override int Insert(int index, string item)
        {
            item = item.NotNullNotEmpty();
            var num = 0;
            var parts = item.Split('.'); foreach (var part in parts)
            {
                var temp = base.Insert(index, ValidateKey(part));
                num += temp;
                index += temp;
            }
            return num;
        }
    }

    /// <summary>
    /// Obtains an inner list to be used by this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The internal collection of elements in this instance.
    /// </summary>
    protected InnerList Items { get; }

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
    bool _CaseSensitiveTags = Engine.CASESENSITIVETAGS;

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
    public string this[int index] => Items[index];

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
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Replace(int index, string value)
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
    public virtual THost Add(string value)
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
    public virtual THost AddRange(IEnumerable<string> range)
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
    public virtual THost Insert(int index, string value)
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
    public virtual THost InsertRange(int index, IEnumerable<string> range)
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
        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual THost Remove(string tag)
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
    public virtual THost Remove(Predicate<string> predicate)
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
    public virtual THost RemoveLast(Predicate<string> predicate)
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
    public virtual THost RemoveAll(Predicate<string> predicate)
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