namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Describes an underlying relational database engine.
/// </summary>
[WithGenerator]
public partial interface IEngine : ORM.IEngine
{
    /// <summary>
    /// The underlying ADO.NET factory used by this instance.
    /// </summary>
    DbProviderFactory Factory { get; }
}