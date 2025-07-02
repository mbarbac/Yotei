namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IMetadataTag"/>
[Cloneable]
public partial class MetadataTag : IMetadataTag
{
    protected virtual Builder Items { get; }
    protected virtual Builder OnInitialize(bool sensitive, string name) => new(sensitive, name);
    protected virtual Builder OnInitialize(bool sensitive, IEnumerable<string> range) => new(sensitive, range);

    /// <summary>
    /// Initializes a new instance with the given default name.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="name"></param>
    public MetadataTag(
        bool caseSensitiveTags, string name) => Items = OnInitialize(caseSensitiveTags, name);

    /// <summary>
    /// Initializes a new instance with the names of the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public MetadataTag(
        bool caseSensitiveTags, IEnumerable<string> range)
        => Items = OnInitialize(caseSensitiveTags, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected MetadataTag(MetadataTag source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IMetadataTag.IBuilder IMetadataTag.CreateBuilder() => CreateBuilder();

    // ------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IMetadataTag? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (Count != other.Count) return false;

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

    public static bool operator ==(MetadataTag? host, IMetadataTag? item)
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
    public virtual MetadataTag Replace(string oldname, string newname)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(oldname, newname);
        return done ? builder.CreateInstance() : this;
    }
    IMetadataTag IMetadataTag.Replace(string oldname, string newname) => Replace(oldname, newname);

    /// <inheritdoc/>
    public virtual MetadataTag Add(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Add(name);
        return done ? builder.CreateInstance() : this;
    }
    IMetadataTag IMetadataTag.Add(string name) => Add(name);

    /// <inheritdoc/>
    public virtual MetadataTag AddRange(IEnumerable<string> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }
    IMetadataTag IMetadataTag.AddRange(IEnumerable<string> range) => AddRange(range);

    /// <inheritdoc/>
    public virtual MetadataTag Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done ? builder.CreateInstance() : this;
    }
    IMetadataTag IMetadataTag.Remove(string name) => Remove(name);

    /// <inheritdoc/>
    public virtual MetadataTag Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
    IMetadataTag IMetadataTag.Clear() => Clear();
}