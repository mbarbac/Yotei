using IHost = Yotei.ORM.Records.IKnownIdentifierTags;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
public sealed class KnownIdentifierTags : IHost
{
    /// <summary>
    /// Represents an internal surrogate collection of elements.
    /// </summary>
    class Surrogate : CoreList<string>
    {
        public Surrogate(IHost master) => Master = master;
        readonly IHost Master;

        protected override string Validate(string item, bool _)
        {
            return item.NotNullNotEmpty();
        }
        protected override bool Compare(string x, string y)
        {
            return string.Compare(x, y, !Master.CaseSensitiveTags) == 0;
        }
        protected override bool IgnoreDuplicate(string _) => false;
        protected override void ThrowWhenDuplicate(string item)
        {
            throw new DuplicateException("Duplicated tag.").WithData(item);
        }
        protected override bool ExpandNested => false;

        // ------------------------------------------------

        public override int Add(string item)
        {
            if (item != null && item.Contains('.'))
            {
                var parts = item.Split('.');
                var count = 0;
                foreach (var part in parts) count += base.Add(part);
                return count;
            }
            else return base.Add(item!);
        }

        public override int Insert(int index, string item)
        {
            if (item != null && item.Contains('.'))
            {
                var parts = item.Split('.');
                var count = 0;
                foreach (var part in parts)
                {
                    var temp = base.Insert(index, part);
                    count += temp;
                    index += temp;
                }
                return count;
            }
            else return base.Insert(index, item!);
        }

        public override int Remove(string item)
        {
            if (item != null && item.Contains('.'))
            {
                var parts = item.Split('.');
                var count = 0;
                foreach (var part in parts) count += base.Remove(part);
                return count;
            }
            return base.Remove(item!);
        }
    }

    /// <summary>
    /// The internal surrogate collection of elements.
    /// </summary>
    Surrogate Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public KnownIdentifierTags(bool caseSensitiveTags)
    {
        Items = new(this);
        CaseSensitiveTags = caseSensitiveTags;
    }

    /// <summary>
    /// Initializes a new instance with the given element. If it contains dot-separated parts,
    /// then each part is added to this collection.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="value"></param>
    public KnownIdentifierTags(bool caseSensitiveTags, string value)
        : this(caseSensitiveTags)
        => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public KnownIdentifierTags(bool caseSensitiveTags, IEnumerable<string> range)
        : this(caseSensitiveTags)
        => Items.AddRange(range);

    /// <summary>
    /// Invoked to obtain a clone of this instance.
    /// </summary>
    /// <returns></returns>
    KnownIdentifierTags Clone() => new(CaseSensitiveTags, this);

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
        init => WithCaseSensitiveTagsInternal(value);
    }
    bool _CaseSensitiveTags = false;

    /// <summary>
    /// Returns an instance of the hosting type where the value of the decorated member
    /// has been replaced by the new given one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IHost WithCaseSensitiveTags(bool value)
    {
        var temp = Clone();
        var done = temp.WithCaseSensitiveTagsInternal(value);
        return done == 0 ? this : temp;
    }
    int WithCaseSensitiveTagsInternal(bool value)
    {
        if (_CaseSensitiveTags == value) return 0;

        _CaseSensitiveTags = value;

        if (Count > 0)
        {
            var range = ToList();
            Items.Clear();
            Items.AddRange(range);
        }
        return Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(string item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(string item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<string> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string[] ToArray() => Items.ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IHost GetRange(int index, int count)
    {
        if (count > 0)
        {
            var range = Items.GetRange(index, count);
            var temp = Clone();
            temp.Items.Clear();
            temp.Items.AddRange(range);
            return temp;
        }
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IHost ReplaceItem(int index, string item)
    {
        var temp = Clone();
        var done = temp.Items.ReplaceItem(index, item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IHost Add(string item)
    {
        var temp = Clone();
        var done = temp.Items.Add(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IHost AddRange(IEnumerable<string> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IHost Insert(int index, string item)
    {
        var temp = Clone();
        var done = temp.Items.Insert(index, item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IHost InsertRange(int index, IEnumerable<string> range)
    {
        var temp = Clone();
        var done = temp.Items.InsertRange(index, range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IHost RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAt(index);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IHost Remove(string item)
    {
        var temp = Clone();
        var done = temp.Items.Remove(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IHost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.Items.RemoveRange(index, count);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IHost Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done == 0 ? this : temp;
    }
}