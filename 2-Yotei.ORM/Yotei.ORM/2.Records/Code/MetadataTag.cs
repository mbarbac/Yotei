namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc/>
public class MetadataTag : IMetadataTag
{
    readonly MetadataTagBuilder Items;

    /// <summary>
    /// Initializes a new instance with the given default name.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="name"></param>
    public MetadataTag(bool sensitive, string name) => Items = new(sensitive, name);

    /// <summary>
    /// Initializes a new instance with the names of the given range, using the first one as
    /// the default tag. If that range was an empty one, then an exception is thrown.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public MetadataTag(bool sensitive, IEnumerable<string> range) => Items = new(sensitive, range);

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public IMetadataTagBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IMetadataTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;

        var sensitive = CaseSensitiveTags;
        var sources = new List<string>(this);
        var targets = new List<string>(other); while (targets.Count > 0)
        {
            var target = targets[0];
            var index = sources.FindIndex(x => string.Compare(x, target, !sensitive) == 0);

            if (index < 0) return false;
            sources.RemoveAt(index);
            targets.RemoveAt(0);
        }
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);
    public static bool operator ==(MetadataTag x, IMetadataTag y) => x is not null && x.Equals(y);
    public static bool operator !=(MetadataTag x, IMetadataTag y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(CaseSensitiveTags);
        foreach (var item in Items.Order()) code = HashCode.Combine(code, item);
        return code;
    }

    // ----------------------------------------------------

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
    public bool ContainsAny(IEnumerable<string> range) => Items.ContainsAny(range);

    /// <inheritdoc/>
    public string[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<string> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag Replace(string name, string item)
    {
        var clone = Items.Clone();
        var done = clone.Replace(name, item);
        return done > 0 ? clone.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag Add(string item)
    {
        var clone = Items.Clone();
        var done = clone.Add(item);
        return done > 0 ? clone.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag AddRange(IEnumerable<string> range)
    {
        var clone = Items.Clone();
        var done = clone.AddRange(range);
        return done > 0 ? clone.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag Remove(string name)
    {
        var clone = Items.Clone();
        var done = clone.Remove(name);
        return done > 0 ? clone.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IMetadataTag Clear()
    {
        var clone = Items.Clone();
        var done = clone.Clear();
        return done > 0 ? clone.ToInstance() : this;
    }
}