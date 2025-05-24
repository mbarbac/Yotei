namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// The collection of metadata tags that are well-known to an underlying relational engine.
/// </summary>
[Cloneable]
[InheritWiths]
public partial interface IKnownTags : ORM.IKnownTags
{
    /// <summary>
    /// The tag used to determine if a given metadata entry is a hidden one, or null if this
    /// information is not available.
    /// </summary>
    [With] IMetadataTag? IsHidden { get; }
}