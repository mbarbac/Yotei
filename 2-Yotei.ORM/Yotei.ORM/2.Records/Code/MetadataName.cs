namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IMetadataName"/>
[Cloneable]
public partial class MetadataName : IMetadataName
{
    /// <summary>
    /// Initializes a new instance with the given default name.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="name"></param>
    public MetadataName(bool sensitive, string name) => throw null;

    /// <summary>
    /// Initializes a new instance with the names of the given name. The first name becomes
    /// the default one. If the range is empty, then an exception is thrown.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public MetadataName(bool sensitive, IEnumerable<string> range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected MetadataName(MetadataName source) => throw null;

    /// <inheritdoc/>
    public override string ToString() => throw null;

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual IMetadataName.IBuilder CreateBuilder() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IMetadataName? other) => throw null;
    //{
    //    if (ReferenceEquals(this, other)) return true;
    //    if (other is null) return false;

    //    if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
    //    if (Count != other.Count) return false;

    //    var targets = new List<string>(other);
    //    while (targets.Count > 0)
    //    {
    //        var target = targets[0];
    //        var found = Contains(target);

    //        if (!found) return false;
    //        targets.RemoveAt(0);
    //    }

    //    return true;
    //}

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
    public override int GetHashCode() => throw null;
    //{
    //    var code = 0;
    //    code = HashCode.Combine(code, CaseSensitiveTags);
    //    foreach (var name in Items) code = HashCode.Combine(code, name);
    //    return code;
    //}

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveNames { get => throw null; }

    /// <inheritdoc/>
    public string Default { get => throw null; set => throw null; }

    /// <inheritdoc/>
    public int Count { get => throw null; }

    /// <inheritdoc/>
    public bool Contains(string name) => throw null;

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => throw null;

    /// <inheritdoc/>
    public string[] ToArray() => throw null;

    /// <inheritdoc/>
    public List<string> ToList() => throw null;

    /// <inheritdoc/>
    public void Trim() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IMetadataName Replace(string oldname, string newname) => throw null;

    /// <inheritdoc/>
    public virtual IMetadataName Add(string name) => throw null;

    /// <inheritdoc/>
    public virtual IMetadataName AddRange(IEnumerable<string> range) => throw null;

    /// <inheritdoc/>
    public virtual IMetadataName Remove(string name) => throw null;

    /// <inheritdoc/>
    public virtual IMetadataName Clear() => throw null;
}