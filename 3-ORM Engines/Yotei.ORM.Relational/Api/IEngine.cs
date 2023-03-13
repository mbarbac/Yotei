namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents an underlying relational database engine.
/// </summary>
public interface IEngine : ORM.IEngine
{
    /// <summary>
    /// The ADO.NET factory used by this instance.
    /// </summary>
    DbProviderFactory Factory { get; }
}