namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IMetadataItem"/>
/// </summary>
[Cloneable(ReturnType = typeof(IMetadataItem), UseVirtual = false)]
[InheritsWith(ReturnType = typeof(IMetadataItem), UseVirtual = false)]
public sealed partial class MetadataItem : IMetadataItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public MetadataItem(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    MetadataItem(MetadataItem other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Name = other.Name;
        Value = other.Value.TryClone();
    }

    public override string ToString() => Value is null
        ? $"{Name}=NULL"
        : $"{Name}='{Value.Sketch()}'";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <param name="ignoreNameCase"></param>
    /// <returns></returns>
    public bool Equals(IMetadataItem? other, bool ignoreNameCase)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (string.Compare(Name, other.Name, ignoreNameCase) != 0) return false;
        if (!Value.EqualsEx(other.Value)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IMetadataItem? other) => Equals(other, false);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataItem);

    public static bool operator ==(MetadataItem? host, IMetadataItem? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(MetadataItem? host, IMetadataItem? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, Value);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; init => field = value.NotNullNotEmpty(trim: true); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object? Value { get; init; }
}