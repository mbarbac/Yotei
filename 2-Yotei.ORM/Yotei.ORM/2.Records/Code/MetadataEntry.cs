namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IMetadataEntry"/>
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

    /// <inheritdoc/>
    public virtual IMetadataEntry WithName(string value) => new MetadataEntry(this) { Name = value };

    /// <inheritdoc/>
    public virtual IMetadataEntry WithValue(object? value) => new MetadataEntry(this) { Value = value };

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IMetadataEntry? other, bool caseSensitiveNames)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (string.Compare(Name, other.Name, !caseSensitiveNames) != 0) return false;
        if (!Value.EqualsEx(other.Value)) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IMetadataEntry? other) => Equals(other, true);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IMetadataEntry);

    public static bool operator ==(MetadataEntry? host, IMetadataEntry? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(MetadataEntry? host, IMetadataEntry? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name, Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;

    /// <inheritdoc/>
    public object? Value { get; init; }
}