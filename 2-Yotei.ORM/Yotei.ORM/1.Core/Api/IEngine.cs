namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Describes an underlying database.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IEngine : IEquatable<IEngine>
{
    /// <summary>
    /// Determines if this engine treats its identifier-alike names and reserved keywords as........
    /// case-sensitive or not.
    /// </summary>
    [With(ReturnInterface = true)]
    bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by this engine to represent a null value in SQL command.
    /// </summary>
    [With(ReturnInterface = true)]
    string NullValueLiteral { get; }

    /// <summary>
    /// Determines if this engine treats the parameters in a SQL command as positional ones, or
    /// rather as named ones.
    /// </summary>
    [With(ReturnInterface = true)]
    bool PositionalParameters { get; }

    /// <summary>
    /// The prefix used by this engine to identify the parameters in a SQL command.
    /// </summary>
    [With(ReturnInterface = true)]
    string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine provides a native support for native pagination, or rather it
    /// has to be emulated by the framework.
    /// </summary>
    [With(ReturnInterface = true)]
    bool SupportsNativePaging { get; }

    /// <summary>
    /// Determines if this engine wraps the SQL command identifier-alike identifiers between
    /// specific terminators, or not.
    /// </summary>
    [With(ReturnInterface = true)]
    bool UseTerminators { get; }

    /// <summary>
    /// The left terminator used by this engine to wrap the its SQL command identifier-alike
    /// identifiers. The value of this setting is ignored if no terminators are used.
    /// </summary>
    [With(ReturnInterface = true)]
    char LeftTerminator { get; }

    /// <summary>
    /// The right terminator used by this engine to wrap the its SQL command identifier-alike
    /// identifiers. The value of this setting is ignored if no terminators are used.
    /// </summary>
    [With(ReturnInterface = true)]
    char RightTerminator { get; }

    /// <summary>
    /// The collection of metadata tags that are well-known to this engine.
    /// </summary>
    [With(ReturnInterface = true)]
    IKnownTags KnownTags { get; }
}