namespace Yotei.ORM.SqlServer;

// ========================================================
/// <summary>
/// Describes an underlying Sql Server database engine.
/// </summary>
[WithGenerator]
public partial interface IEngine : Relational.IEngine { }