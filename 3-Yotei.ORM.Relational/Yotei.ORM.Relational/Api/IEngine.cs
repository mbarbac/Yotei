namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents the kind and settings of an underlying relational ADO.NET-based database engine.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
[InheritsWith]
public partial interface IEngine : ORM.IEngine
{
    /// <summary>
    /// The underlying ADO.NET provider factory used by this instance.
    /// </summary>
    [With] DbProviderFactory Factory { get; }
}