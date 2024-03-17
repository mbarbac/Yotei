namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IMetadataEntry"/>
[WithGenerator]
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
    MetadataEntry(MetadataEntry source) : this(source.Tag, source.Value) { }

    /// <inheritdoc/>
    public override string ToString() => $"{Tag.DefaultName} = '{Value.Sketch()}'";
    
    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IMetadataEntry? other)
    {
        if (other is null) return false;

        if (!Tag.Equals(other.Tag)) return false;
        if (!Value.EquivalentTo(other.Value)) return false;
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IMetadataEntry);
    public static bool operator ==(MetadataEntry x, IMetadataEntry y) => x is not null && x.Equals(y);
    public static bool operator !=(MetadataEntry x, IMetadataEntry y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(Tag);
        code = HashCode.Combine(code, Value);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag Tag
    {
        get => _Tag!;
        init => _Tag = value.ThrowWhenNull();
    }
    IMetadataTag? _Tag;

    /// <inheritdoc/>
    public object? Value { get; init; }
}