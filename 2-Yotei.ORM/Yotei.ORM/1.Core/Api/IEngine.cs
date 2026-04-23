namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the kind and operational settings of an underlying database engine.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IEngine : IEquatable<IEngine>
{
    /// <summary>
    /// Determines whether the engine ignores case for identifier-alike names and keywords.
    /// </summary>
    [With] bool IgnoreCase { get; }

    /// <summary>
    /// The literal used by the engine to represent NULL values.
    /// <br/> The value of this property must not be null not empty.
    /// </summary>
    [With] string NullValueLiteral { get; }

    /// <summary>
    /// Determines if the engine uses positional parameters instead of named ones.
    /// </summary>
    [With] bool PositionalParameters { get; }

    /// <summary>
    /// The prefix uses by the engine to identify command parameter names.
    /// <br/> This property accepts empty values, but not null ones.
    /// </summary>
    [With] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if the engine provides native paging capabilities, or rather they have to be
    /// emulated by the framework.
    /// </summary>
    [With] bool NativePaging { get; }

    /// <summary>
    /// Determines if the engine wraps its identifier-alike elements with the given terminators,
    /// or not.
    /// </summary>
    [With] bool UseTerminators { get; }

    /// <summary>
    /// If the engine uses identifier terminators, then the left one.
    /// <br/> The value of this property must be a valid character.
    /// </summary>
    [With] char LeftTerminator { get; }

    /// <summary>
    /// If the engine uses identifier terminators, then the right one.
    /// <br/> The value of this property must be a valid character.
    /// </summary>
    [With] char RightTerminator { get; }
}