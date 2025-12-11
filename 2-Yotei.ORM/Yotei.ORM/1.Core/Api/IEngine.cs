namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Describes an underlying database engine.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IEngine : IEquatable<IEngine>
{
    /// <summary>
    /// Determines if the identifier-alike names and reserved keywords are case sensitive or not.
    /// </summary>
    [With] bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by the engine to represent NULL values.
    /// </summary>
    [With] string NullValueLiteral { get; }

    /// <summary>
    /// Determines if this engine uses positional parameters in its database commands, or rather,
    /// use them by name.
    /// </summary>
    [With] bool PositionalParameters { get; }

    /// <summary>
    /// The literal prefix used with parameters to identify them as such in database commands.
    /// </summary>
    [With] string ParametersPrefix { get; }

    /// <summary>
    /// Determines if this engine supports native paging capabilities, or rather they shall be
    /// emulated by the framework.
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