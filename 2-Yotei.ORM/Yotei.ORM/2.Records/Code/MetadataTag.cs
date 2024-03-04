namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IMetadataTag"/>
[Cloneable]
public partial class MetadataTag : IMetadataTag
{
    readonly List<string> Items = [];

    /// <summary>
    /// Initializes a new instance with the given name.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="name"></param>
    public MetadataTag(bool caseSensitive, string name)
    {
        CaseSensitiveTags = caseSensitive;
        AddInternal(name);
    }

    /// <summary>
    /// Initializes a new instance with the names from the given range.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="range"></param>
    public MetadataTag(bool caseSensitive, IEnumerable<string> range)
    {
        CaseSensitiveTags = caseSensitive;
        AddRangeInternal(range);

        if (Items.Count == 0) throw new ArgumentException("No names when creating a new instance");
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataTag(MetadataTag source)
    {
        CaseSensitiveTags = source.CaseSensitiveTags;
        AddRangeInternal(source);
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Items.Count == 1) return DefaultName;
        else
        {
            var sb = new StringBuilder();
            sb.Append(Items[0]);
            
            sb.Append(" ("); for (int i = 1; i < Items.Count; i++)
            {
                if (i > 1) sb.Append(", ");
                sb.Append(Items[i]);
            }
            sb.Append(')');
            return sb.ToString();

        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }
    bool Compare(string x, string y)
    {
        return string.Compare(x, y, !CaseSensitiveTags) == 0;
    }

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public bool Contains(string name)
    {
        name = name.NotNullNotEmpty();
        return Items.Find(x => Compare(x, name)) != null;
    }

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        foreach (var name in range) if (Contains(name)) return true;
        return false;
    }

    /// <inheritdoc/>
    public string DefaultName => Items[0];

    /// <inheritdoc/>
    public void SetDefault(string name)
    {
        name = name.NotNullNotEmpty();

        var index = Items.FindIndex(x => Compare(x, name));
        if (index < 0) throw new NotFoundException(
            "Name is not present in this collection.").WithData(name);

        if (index == 0) return;

        var item = Items[index];
        Items.RemoveAt(index);
        Items.Insert(0, item);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag Replace(string oldname, string newname, bool asDefault = false)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(oldname, newname, asDefault);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(string oldname, string newname, bool asDefault = false)
    {
        oldname = oldname.NotNullNotEmpty();
        newname = newname.NotNullNotEmpty();

        if (oldname == newname) return 0;

        var index = Items.FindIndex(x => Compare(x, oldname));
        if (index < 0) return 0;

        Items.RemoveAt(index);
        return AddInternal(newname, asDefault);
    }

    /// <inheritdoc/>
    public IMetadataTag Add(string name, bool asDefault = false)
    {
        var clone = Clone();
        var done = clone.AddInternal(name, asDefault);
        return done > 0 ? clone : this;
    }
    int AddInternal(string name, bool asDefault = false)
    {
        name = name.NotNullNotEmpty();

        if (Contains(name)) throw new DuplicateException("Duplicated name.").WithData(name);

        if (asDefault) Items.Insert(0, name);
        else Items.Add(name);

        return 1;
    }

    /// <inheritdoc/>
    public IMetadataTag AddRange(IEnumerable<string> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<string> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var name in range)
        {
            var r = AddInternal(name);
            num += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public IMetadataTag Remove(string name)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(name);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string name)
    {
        name = name.NotNullNotEmpty();

        var index = Items.FindIndex(x => Compare(x, name));
        if (index < 0) return 0;

        if (Items.Count <= 1) throw new InvalidOperationException(
            "Cannot remove that last remaining name.")
            .WithData(name);

        Items.RemoveAt(index);
        return 1;
    }
}