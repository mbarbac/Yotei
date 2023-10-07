#pragma warning disable CS0436

namespace Yotei.ORM.Entities;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database that acts as a context for managed
/// entities.
/// </summary>
[Cloneable]
[WithGenerator]
public partial interface IRepository : IConnection
{
}