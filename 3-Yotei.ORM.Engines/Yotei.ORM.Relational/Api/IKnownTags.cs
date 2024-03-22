namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents the collection of tags that are well-known for an underlying relational engine.
/// </summary>
[Cloneable]
[WithGenerator]
public partial interface IKnownTags : Records.IKnownTags { }