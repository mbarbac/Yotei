namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// An immutable object that describes a given relational database engine.
/// </summary>
[WithGenerator]
public partial interface IEngine : ORM.IEngine
{
    /// <summary>
    /// The ADO.NET factory used by this instance.
    /// </summary>
    DbProviderFactory Factory { get; }
}