namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IMetadataName"/>
[Cloneable]
public partial class MetadataName : IMetadataName
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new instance with the given default name.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="name"></param>
    public MetadataName(bool sensitive, string name) => Items = new(sensitive, name);

    /// <summary>
    /// Initializes a new instance with the names of the given name. The first name becomes
    /// the default one. If the range is empty, then an exception is thrown.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public MetadataName(
        bool sensitive, IEnumerable<string> range) => Items = new(sensitive, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected MetadataName(MetadataName source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual IMetadataName.IBuilder CreateBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IMetadataName? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (CaseSensitiveNames != other.CaseSensitiveNames) return false;
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
    public override bool Equals(object? obj) => Equals(obj as IMetadataName);

    public static bool operator ==(MetadataName? host, IMetadataName? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(MetadataName? host, IMetadataName? item) => !(host == item);


    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveNames);
        foreach (var name in Items) code = HashCode.Combine(code, name);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveNames => Items.CaseSensitiveNames;

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IMetadataName Replace(string oldname, string newname)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(oldname, newname);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IMetadataName Add(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Add(name);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IMetadataName AddRange(IEnumerable<string> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IMetadataName Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IMetadataName Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
}