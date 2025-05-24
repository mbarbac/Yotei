namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IMetadataTag"/>
[Cloneable]
public sealed partial class MetadataTag : IMetadataTag
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new instance with the given default name.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="name"></param>
    public MetadataTag(bool caseSensitive, string name) => Items = new(caseSensitive, name);

    /// <summary>
    /// Initializes a new instance with the names of the given range, using as the default
    /// one the first one in the range. If the range is empty, then an exception is thrown.
    /// </summary>
    /// <param name="caseSensitive"></param>
    /// <param name="range"></param>
    public MetadataTag(
        bool caseSensitive, IEnumerable<string> range) => Items = new(caseSensitive, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataTag(MetadataTag source)
    {
        source.ThrowWhenNull();
        Items = new(source.CaseSensitiveTags, source);
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public IMetadataTag.IBuilder GetBuilder() => Items.Clone();

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IMetadataTag? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IMetadataTag valid) return false;

        if (CaseSensitiveTags != valid.CaseSensitiveTags) return false;
        if (Count != valid.Count) return false;

        var targets = new List<string>(other);
        while (targets.Count > 0)
        {
            var target = targets[0];
            var found = Contains(target);

            if (!found) return false;
            targets.RemoveAt(0);
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);

    public static bool operator ==(MetadataTag? host, IMetadataTag? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(MetadataTag? host, IMetadataTag? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveTags);
        foreach (var name in Items) code = HashCode.Combine(code, name);
        return code;
    }

    // ------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags => Items.CaseSensitiveTags;

    /// <inheritdoc/>
    public string Default
    {
        get => Items.Default;
        set => Items.Default = value;
    }

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

    /// <inheritdoc/>
    public string[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<string> ToList() => Items.ToList();

    /// <inheritdoc/>
    public void Trim() => Items.Trim();

    // ------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag Replace(string oldname, string newname)
    {
        var builder = GetBuilder();
        var done = builder.Replace(oldname, newname);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag Add(string name)
    {
        var builder = GetBuilder();
        var done = builder.Add(name);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag AddRange(IEnumerable<string> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag Remove(string name)
    {
        var builder = GetBuilder();
        var done = builder.Remove(name);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag Clear()
    {
        var builder = GetBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }
}