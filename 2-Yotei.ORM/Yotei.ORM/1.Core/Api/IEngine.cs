namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Describes an underlying database.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IEngine : IEquatable<IEngine>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    IEngine Clone();

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithCaseSensitiveNames(bool value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithNullValueLiteral(string value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithPositionalParameters(bool value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithParameterPrefix(string value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithSupportsNativePaging(bool value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithUseTerminators(bool value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithLeftTerminator(char value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithRightTerminator(char value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IEngine WithKnownTags(IKnownTags value);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this engine treats its identifier-alike names and reserved keywords as........
    /// case-sensitive or not.
    /// </summary>
    bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by this engine to represent a null value in SQL command.
    /// </summary>
    string NullValueLiteral { get; }

    /// <summary>
    /// Determines if this engine treats the parameters in a SQL command as positional ones, or
    /// rather as named ones.
    /// </summary>
    bool PositionalParameters { get; }

    /// <summary>
    /// The prefix used by this engine to identify the parameters in a SQL command.
    /// </summary>
    string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine provides a native support for native pagination, or rather it
    /// has to be emulated by the framework.
    /// </summary>
    bool SupportsNativePaging { get; }

    /// <summary>
    /// Determines if this engine wraps the SQL command identifier-alike identifiers between
    /// specific terminators, or not.
    /// </summary>
    bool UseTerminators { get; }

    /// <summary>
    /// The left terminator used by this engine to wrap the its SQL command identifier-alike
    /// identifiers. The value of this setting is ignored if no terminators are used.
    /// </summary>
    char LeftTerminator { get; }

    /// <summary>
    /// The right terminator used by this engine to wrap the its SQL command identifier-alike
    /// identifiers. The value of this setting is ignored if no terminators are used.
    /// </summary>
    char RightTerminator { get; }

    /// <summary>
    /// The collection of metadata tags that are well-known to this engine.
    /// </summary>
    IKnownTags KnownTags { get; }
}