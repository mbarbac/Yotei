namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IMetadataTag"/>
/// </summary>
[Cloneable<IMetadataTag>]
public partial class MetadataTag : IMetadataTag
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new instance with the given default tag name.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="tagname"></param>
    public MetadataTag(bool sensitive, string tagname) => Items = new(sensitive, tagname);

    /// <summary>
    /// Initializes a new instance with the names from the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public MetadataTag(
        bool sensitive, IEnumerable<string> range) => Items = new(sensitive, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected MetadataTag(MetadataTag source)
    {
        source.ThrowWhenNull();
        Items = new(source.CaseSensitiveTags, source);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"[{string.Join(", ", Items)}]";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);

    public static bool operator ==(MetadataTag? host, IMetadataTag? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(MetadataTag? host, IMetadataTag? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveTags);
        foreach (var name in Items) code = HashCode.Combine(code, name);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IMetadataTag.IBuilder CreateBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveTags => Items.CaseSensitiveTags;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Default => Items.Default;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    public bool Contains(string tagname) => Items.Contains(tagname);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="oldtagname"></param>
    /// <param name="newtagname"></param>
    /// <returns></returns>
    public virtual IMetadataTag Replace(string oldtagname, string newtagname)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(oldtagname, newtagname);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    public virtual IMetadataTag Add(string tagname)
    {
        var builder = CreateBuilder();
        var done = builder.Add(tagname);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IMetadataTag AddRange(IEnumerable<string> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    public virtual IMetadataTag Remove(string tagname)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(tagname);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IMetadataTag Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
}