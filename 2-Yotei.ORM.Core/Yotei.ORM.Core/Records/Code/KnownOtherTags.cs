using IHost = Yotei.ORM.Records.IKnownOtherTags;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// /// <inheritdoc cref="IHost"/>
/// </summary>
[WithGenerator]
public partial class KnownOtherTags : IHost
{
    /// <summary>
    /// Represents an internal surrogate collection of elements.
    /// </summary>
    protected class Surrogate : CoreList<string>
    {
        public Surrogate(IHost master) => Master = master;
        readonly IHost Master;
        protected override string Validate(string item, bool add)
        {
            return item.NotNullNotEmpty();
        }
        protected override bool Compare(string x, string y)
        {
            return string.Compare(x, y, !Master.CaseSensitiveTags) == 0;
        }
        protected override bool IgnoreDuplicate(string _) => false;
        protected override void ThrowWhenDuplicate(string tag)
        {
            throw new DuplicateException("Duplicate tags are not allowed.").WithData(tag);
        }
        protected override bool ExpandNested => false;
    }

    /// <summary>
    /// The internal surrogate collection of elements.
    /// </summary>
    protected Surrogate Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public KnownOtherTags(bool caseSensitiveTags)
    {
        Items = new(this);
        CaseSensitiveTags = caseSensitiveTags;
    }

    /// <summary>
    /// Initializes a new instance with the given tag.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="item"></param>
    public KnownOtherTags(bool caseSensitiveTags, string item)
        : this(caseSensitiveTags)
        => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the given range of tags.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public KnownOtherTags(bool caseSensitiveTags, IEnumerable<string> range)
        : this(caseSensitiveTags)
        => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc cref="IHost.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual KnownOtherTags Clone() => new(CaseSensitiveTags, this);
    IHost IHost.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveTags
    {
        get => _CaseSensitiveTags;
        init
        {
            if (value == _CaseSensitiveTags) return;

            _CaseSensitiveTags = value;

            if (Items.Count > 0)
            {
                var range = Items.ToList();
                Items.Clear();
                Items.AddRange(range);
            }
        }
    }
    bool _CaseSensitiveTags = false;

    /// <summary>
    /// Returns an instance of the hosting type where the value of the decorated member
    /// has been replaced by the new given one.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public virtual KnownOtherTags WithCaseSensitiveTags(bool sensitive)
    {
        return new(sensitive, this);
    }
    IHost IHost.WithCaseSensitiveTags(bool value) => WithCaseSensitiveTags(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => Items.Contains(tag);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual IHost Replace(string source, string target)
    {
        var temp = Clone();
        var index = temp.Items.IndexOf(source);
        if (index >= 0)
        {
            var done = temp.Items.ReplaceItem(index, target);
            if (done > 0) return temp;
        }
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual IHost Add(string tag)
    {
        var temp = Clone();
        var done = temp.Items.Add(tag);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<string> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual IHost Remove(string tag)
    {
        var temp = Clone();
        var done = temp.Items.Remove(tag);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost RemoveRange(IEnumerable<string> range)
    {
        var temp = Clone();
        var done = 0;

        foreach (var item in range) done += temp.Items.Remove(item);
        return done == 0 ? this : temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done == 0 ? this : temp;
    }
}