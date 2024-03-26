namespace Yotei.ORM.Records;

// ========================================================
/// <inheritdoc cref="ORM.IEngine"/>
[WithGenerator]
public partial interface IEngine : ORM.IEngine, IEquatable<IEngine>
{
    /// <summary>
    /// The collection of metadata tags that are well-known to this engine.
    /// </summary>
    [WithGenerator] IKnownTags KnownTags { get; }
}