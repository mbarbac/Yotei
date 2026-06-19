namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IMetadataTag"/>
/// </summary>
[Cloneable(ReturnType = typeof(IMetadataTag), UseVirtual = false)]
public sealed partial class MetadataTag : IMetadataTag
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new instance with the given name as its default one.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="name"></param>
    public MetadataTag(bool ignoreCase, string name) => Items = new(ignoreCase, name);

    /// <summary>
    /// Initializes a new instance with the names from the given not-empty range.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="range"></param>
    public MetadataTag(bool ignoreCase, IEnumerable<string> range) => Items = new(ignoreCase, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    MetadataTag(MetadataTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Items = new(other.IgnoreCase, other);
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
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IMetadataTag? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (IgnoreCase != other.IgnoreCase) return false;
        if (Count != other.Count) return false;

        var temps = other.ToList();
        foreach (var item in Items)
        {
            var index = temps.FindIndex(x => string.Compare(x, item, IgnoreCase) == 0);
            if (index >= 0) temps.RemoveAt(index);
            else break;
        }

        return temps.Count == 0;
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
        code = HashCode.Combine(code, IgnoreCase);
        code = HashCode.Combine(code, Count);
        foreach (var name in Items) code = HashCode.Combine(code, name);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase => Items.IgnoreCase;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Default
    {
        get => Items.Default;
        set => Items.Default = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => Items.Contains(name);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> range) => Items.ContainsAny(range);

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
    /// <returns></returns>
    public IMetadataTag.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <returns></returns>
    public IMetadataTag Replace(string oldname, string newname)
    {
        var builder = ToBuilder();
        var done = builder.Replace(oldname, newname);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataTag Add(string name)
    {
        var builder = ToBuilder();
        var done = builder.Add(name);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IMetadataTag AddRange(IEnumerable<string> range)
    {
        var builder = ToBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataTag Remove(string name)
    {
        var builder = ToBuilder();
        var done = builder.Remove(name);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IMetadataTag Clear()
    {
        var builder = ToBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }
}