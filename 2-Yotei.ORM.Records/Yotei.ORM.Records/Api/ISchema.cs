namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of metadata entries that describes the structure and contents
/// of associated records.
/// </summary>
public interface ISchema : IEquatable<ISchema>
{
    /// <summary>
    /// The engine this instance is associted with.
    /// </summary>
    IEngine Engine { get; }
}