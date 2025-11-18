namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IMetadataEntry"/>
/// </summary>
[InheritWiths<IMetadataEntry>]
public partial class MetadataEntry : IMetadataEntry
{
    /// <summary>
    /// Initialiazes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public MetadataEntry(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected MetadataEntry(MetadataEntry source)
    {
        source.ThrowWhenNull();

        Name = source.Name;
        Value = source.Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{Name}='{Value.Sketch()}']";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveTags"></param>
    /// <returns></returns>
    public virtual bool Equals(IMetadataEntry? other, bool caseSensitiveTags)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (string.Compare(Name, other.Name, !caseSensitiveTags) != 0) return false;
        if (!Value.EqualsEx(other.Value)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IMetadataEntry? other) => Equals(other, true);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataEntry);

    public static bool operator ==(MetadataEntry? host, IMetadataEntry? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(MetadataEntry? host, IMetadataEntry? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => HashCode.Combine(Name, Value);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object? Value { get; init; }
}