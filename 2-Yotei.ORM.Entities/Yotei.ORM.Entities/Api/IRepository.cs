#pragma warning disable CS0436

namespace Yotei.ORM.Entities;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database that acts as a unit of work and as a
/// repository for managed entities.
/// </summary>
[Cloneable]
[WithGenerator]
public partial interface IRepository : IConnection
{
    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }
}