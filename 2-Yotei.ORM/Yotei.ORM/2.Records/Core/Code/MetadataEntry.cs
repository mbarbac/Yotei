namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IMetadataEntry"/>
[InheritWiths]
public sealed partial class MetadataEntry : IMetadataEntry
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public MetadataEntry(IMetadataTag tag, object? value)
    {
        Tag = tag;
        Value = value;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataEntry(MetadataEntry source)
    {
        source.ThrowWhenNull();

        Tag = source.Tag;
        Value = source.Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Tag.Default} = '{Value.Sketch()}'";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IMetadataEntry? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (!Tag.Equals(other.Tag)) return false;
        if (!Value.EqualsEx(other.Value)) return false;
        return true;
    }

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
    public override int GetHashCode() => HashCode.Combine(Tag, Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag Tag
    {
        get => _Tag!;
        init => _Tag = value.ThrowWhenNull();
    }
    IMetadataTag? _Tag = default;

    /// <inheritdoc/>
    public object? Value { get; init; }
}