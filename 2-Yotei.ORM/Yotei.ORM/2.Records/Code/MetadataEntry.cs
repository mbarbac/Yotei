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
    MetadataEntry(MetadataEntry source)
    {
        Tag = source.Tag;
        Value = source.Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Tag.DefaultName} = '{Value.Sketch()}'";

    /// <inheritdoc/>
    public IMetadataTag Tag
    {
        get => _Tag;
        init => _Tag = value.ThrowWhenNull();
    }
    IMetadataTag _Tag = default!;

    /// <inheritdoc/>
    public object? Value { get; init; }
}