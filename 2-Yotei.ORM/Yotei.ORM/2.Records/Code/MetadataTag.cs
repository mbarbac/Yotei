namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IMetadataTag"/>
[Cloneable]
public sealed partial class MetadataTag : IMetadataTag
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
        AddInternal(name, asDefault: true);
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

        if (Items.Count == 0) throw new ArgumentException(
            "Collection of metadata names was empty when creating this instance.")
            .WithData(range);
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

    // <inheritdoc/>
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
    public bool Equals(IMetadataTag? other)
    {
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;

        List<string> sources = new(Items);
        List<string> targets = new(other);
        while (sources.Count > 0)
        {
            var source = sources[0];
            var index = targets.FindIndex(x => Compare(x, source));
            if (index < 0) return false;

            sources.RemoveAt(0);
            targets.RemoveAt(index);
        }
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);
    public static bool operator ==(MetadataTag x, IMetadataTag y) => x is not null && x.Equals(y);
    public static bool operator !=(MetadataTag x, IMetadataTag y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(CaseSensitiveTags);
        for (int i = 0; i < Items.Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    bool Compare(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }

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
            "Name to set as the default one is not in this collection.").WithData(name);

        if (index == 0) return;

        var item = Items[index];
        Items.RemoveAt(0);
        Items.Insert(index, item);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag Replace(string oldName, string newName, bool asDefault = false)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(oldName, newName, asDefault);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(string oldName, string newName, bool asDefault)
    {
        oldName = oldName.NotNullNotEmpty();
        newName = newName.NotNullNotEmpty();

        if (Compare(oldName, newName)) return 0;

        var index = Items.FindIndex(x => Compare(x, oldName));
        if (index < 0) return 0;

        Items.RemoveAt(index);
        return AddInternal(newName, asDefault);
    }

    /// <inheritdoc/>
    public IMetadataTag Add(string name, bool asDefault = false)
    {
        var clone = Clone();
        var done = clone.AddInternal(name, asDefault);
        return done > 0 ? clone : this;
    }
    int AddInternal(string name, bool asDefault)
    {
        name = name.NotNullNotEmpty();

        if (Contains(name)) throw new DuplicateException(
            "This instance already contains the given name.").WithData(name).WithData(this);

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
            var r = AddInternal(name, asDefault: false);
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
            "Cannot remove the unique name in this collection.")
            .WithData(name);

        Items.RemoveAt(index);
        return 1;
    }

    /// <inheritdoc/>
    public IMetadataTag Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    int ClearInternal()
    {
        if (Count <= 1) return 0;

        var item = Items[0];
        Items.Clear();
        Items.Add(item);
        return 1;
    }
}