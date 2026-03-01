namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an underlying database engine.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
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
    /// Determines if this engine uses positional parameters in its database commands, or rather
    /// use them by name.
    /// </summary>
    [With] bool PositionalParameters { get; }

    /// <summary>
    /// The literal used by the engine to prefix the command parameter names.
    /// </summary>
    [With] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine provides native paging capabilities, or rather they have to be
    /// emulated by the framework.
    /// </summary>
    [With] bool NativePaging { get; }

    /// <summary>
    /// Determines if this engine wraps the identifier-alike elements in its database commands
    /// with the specified terminators, or not.
    /// </summary>
    [With] bool UseTerminators { get; }

    /// <summary>
    /// If terminators are used, the left character used to wrap identifier-alike elements.
    /// </summary>
    [With] char LeftTerminator { get; }

    /// <summary>
    /// If terminators are used, the right character used to wrap identifier-alike elements.
    /// </summary>
    [With] char RightTerminator { get; }
}