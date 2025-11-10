namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the descriptor of an underlying database engine.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IEngine : IEquatable<IEngine>
{
    /// <summary>
    /// Determines if the identifier-alike elements and reserved keywords are case sensitive,
    /// or not.
    /// </summary>
    [With] bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by this engine to represents NULL values.
    /// </summary>
    [With] string NullValueLiteral { get; }

    /// <summary>
    /// Whether this engine treats the parameters of database commands as positional ones, or not,
    /// and so treats them by name.
    /// </summary>
    [With] bool PositionalParameters { get; }

    /// <summary>
    /// The literal used by this engine to prepend the parameters of database commands.
    /// </summary>
    [With] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine provides native pagination capabilities, or rather they have to
    /// be emulated by the framework.
    /// </summary>
    [With] bool NativePaging { get; }

    /// <summary>
    /// Determines if this engine wraps the identifier-alike elements in database commands with
    /// the specified terminators, or not.
    /// </summary>
    [With] bool UseTerminators { get; }

    /// <summary>
    /// The left terminator used by this engine to wrap identifier-alike elements in database
    /// commands, if terminators are used.
    /// </summary>
    [With] char LeftTerminator { get; }

    /// <summary>
    /// The right terminator used by this engine to wrap identifier-alike elements in database
    /// commands, if terminators are used.
    /// </summary>
    [With] char RightTerminator { get; }
}