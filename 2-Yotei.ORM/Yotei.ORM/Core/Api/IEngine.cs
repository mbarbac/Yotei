namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents an underlying database engine.
/// </summary>
public partial interface IEngine
{
    /// <summary>
    /// Determines if this instance treats the identifier-alike names in the underlying database
    /// as case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by the underlying database to represent null or missed valued.
    /// </summary>
    [WithGenerator] string NullValueLiteral { get; }

    /// <summary>
    /// Determines if the underlying database engine provides native paging capabilities, or if
    /// they have to be emulated by the framework.
    /// </summary>
    [WithGenerator] bool NativePaging { get; }

    /// <summary>
    /// Determines if the underlying database wraps the names of the identifier-alike elements
    /// between terminators, or not.
    /// </summary>
    [WithGenerator] bool UseTerminators { get; }

    /// <summary>
    /// The left terminator uses to wrap the identifier-alike names, if terminators are used.
    /// </summary>
    [WithGenerator] char LeftTerminator { get; }

    /// <summary>
    /// The right terminator uses to wrap the identifier-alike names, if terminators are used.
    /// </summary>
    [WithGenerator] char RightTerminator { get; }
}