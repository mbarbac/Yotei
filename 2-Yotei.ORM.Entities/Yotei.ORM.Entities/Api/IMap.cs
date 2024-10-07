namespace Yotei.ORM.Entities;

// ========================================================
/// <summary>
/// Represents a map between the members of a given entity type and the contents stored in an
/// associated database.
/// </summary>
public interface IMap
{
    /// <summary>
    /// The entity type this instance refers to.
    /// </summary>
    Type EntityType { get; }
}