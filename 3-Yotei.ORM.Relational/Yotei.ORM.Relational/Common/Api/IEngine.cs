namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// An immutable object that describes a given relational ADO.NET database engine.
/// </summary>
[WithGenerator]
public partial interface IEngine : ORM.IEngine
{
    /// <summary>
    /// The ADO.NET factory this instance is associated with.
    /// </summary>
    DbProviderFactory Factory { get; }
}