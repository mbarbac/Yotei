using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = string;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[WithGenerator("(sensitiveTags=@,this)+@")]
public partial class IdentifierTags : IHost
{
    // Represents the actual contents carried by this instance.
    protected class InnerList : CoreList<IItem>
    {
        public InnerList(IdentifierTags master)
        {
            Master = master.ThrowWhenNull();
            ExpandNested = false;
        }
        IdentifierTags Master;
        public override IItem Validate(IItem item, bool _) => item.NotNullNotEmpty();
        public override bool Equivalent(IItem inner, IItem other)
        {
            return string.Compare(inner, other, !Master.CaseSensitiveTags) == 0;
        }
        public override bool IgnoreDuplicate(IItem item) => base.IgnoreDuplicate(item);
        protected override int OnAdd(string item)
        {
            if (item != null && item.Contains('.'))
            {
                var parts = item.Split('.');
                var count = 0;
                foreach (var part in parts) count += base.OnAdd(part);
                return count;
            }
            return base.OnAdd(item!);
        }
        protected override int OnInsert(int index, string item)
        {
            if (item != null && item.Contains('.'))
            {
                var parts = item.Split('.');
                var count = 0;
                foreach (var part in parts)
                {
                    var temp = base.OnInsert(index, part);
                    count += temp;
                    index += temp;
                }
                return count;
            }
            return base.OnInsert(index, item!);
        }
        protected override int OnRemove(string item)
        {
            if (item != null && item.Contains('.'))
            {
                var parts = item.Split('.');
                var count = 0;
                foreach (var part in parts) count += base.OnRemove(part);
                return count;
            }
            return base.OnRemove(item!);
        }
    }

    // The actual contents carried by this instance.
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="sensitiveTags"></param>
    public IdentifierTags(bool sensitiveTags)
    {
        Items = new(this);
        CaseSensitiveTags = sensitiveTags;
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="sensitiveTags"></param>
    /// <param name="item"></param>
    public IdentifierTags(bool sensitiveTags, IItem item) : this(sensitiveTags) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="sensitiveTags"></param>
    /// <param name="range"></param>
    public IdentifierTags(bool sensitiveTags, IEnumerable<IItem> range) : this(sensitiveTags) => Items.AddRange(range);

    /// <summary>
    /// Invoked to obtain a clone of this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual IdentifierTags Clone() => new(CaseSensitiveTags, this);

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
    public IItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(IItem item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(IItem item) => Items.IndexOf(item);

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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost GetRange(int index, int count)
    {
        if (count == 0) return this;
        if (index == 0 && count == Count) return this;

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
        var done = temp.Items.Replace(index, item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Add(item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<IItem> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done > 0 ? temp : this;
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
        var done = temp.Items.Insert(index, item);
        return done > 0 ? temp : this;
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
        var done = temp.Items.InsertRange(index, range);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IHost RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAt(index);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Remove(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Remove(item);
        return done > 0 ? temp : this;
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
        var done = temp.Items.RemoveRange(index, count);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost Remove(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.Remove(predicate);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(predicate);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(predicate);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done > 0 ? temp : this;
    }
}