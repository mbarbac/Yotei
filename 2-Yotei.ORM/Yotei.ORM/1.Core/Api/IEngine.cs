namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Describes an underlying database engine.
/// </summary>
[Cloneable]
public partial interface IEngine : IEquatable<IEngine>
{
    /// <summary>
    /// Determines if this engine treats its identifier-alike names and reserved keywords as........
    /// case-sensitive or not.
    /// </summary>
    [With] bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by this engine to represent a null value in SQL command.
    /// </summary>
    [With] string NullValueLiteral { get; }

    /// <summary>
    /// Determines if this engine treats the parameters in a SQL command as positional ones, or
    /// rather as named ones.
    /// </summary>
    [With] bool PositionalParameters { get; }

    /// <summary>
    /// The prefix used by this engine to identify the parameters in a SQL command.
    /// </summary>
    [With] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine provides a native support for native pagination, or rather it
    /// has to be emulated by the framework.
    /// </summary>
    [With] bool NativePagination { get; }

    /// <summary>
    /// Determines if this engine wraps the SQL command identifier-alike identifiers between
    /// specific terminators, or not.
    /// </summary>
    [With] bool UseTerminators { get; }

    /// <summary>
    /// The left terminator used by this engine to wrap the its SQL command identifier-alike
    /// identifiers. The value of this setting is ignored if no terminators are used.
    /// </summary>
    [With] char LeftTerminator { get; }

    /// <summary>
    /// The right terminator used by this engine to wrap the its SQL command identifier-alike
    /// identifiers. The value of this setting is ignored if no terminators are used.
    /// </summary>
    [With] char RightTerminator { get; }

    /// <summary>
    /// The collection of metadata tags that are well-known to this engine.
    /// </summary>
    [With] IKnownTags KnownTags { get; }
}