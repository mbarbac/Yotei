namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IMetadataPair"/>
/// </summary>
/// <param name="tag"></param>
/// <param name="value"></param>
public class MetadataPair(string tag, object? value) : IMetadataPair
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Tag}='{Value.Sketch()}'";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Tag { get; } = tag.NotNullNotEmpty();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object? Value { get; } = value;
}