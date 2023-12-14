namespace Yotei.ORM.Entities;

// ========================================================
/// <summary>
/// Represents a map between a given entity type and a primary table in an underlying database.
/// </summary>
public interface IEntityMap
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The type of the entities this map is created for.
    /// </summary>
    Type EntityType { get; }
}