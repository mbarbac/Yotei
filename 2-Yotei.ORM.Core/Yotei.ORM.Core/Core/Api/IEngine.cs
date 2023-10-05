namespace Yotei.ORM.Core;

// ========================================================
/// <summary>
/// Represents an underlying database engine.
/// </summary>
public partial interface IEngine
{
    /// <summary>
    /// Determines if the identifier-alike elements in the database are case sensitive, or not.
    /// </summary>
    [WithGenerator]
    bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by the underlying database to represent null or missed values.
    /// </summary>
    [WithGenerator]
    string NullValueLiteral { get; }

    /// <summary>
    /// Whether the underlying database provides a native paging capability, or rather it shall
    /// be emulated by the framework.
    /// </summary>
    [WithGenerator]
    bool NativePaging { get; }

    /// <summary>
    /// The literal used by the underlying database to prefix the names of command arguments.
    /// </summary>
    [WithGenerator]
    string ParameterPrefix { get; }

    /// <summary>
    /// Whether the underlying database treats the parameters of a command in positional order,
    /// or <c>false</c> if it treats them by name.
    /// </summary>
    [WithGenerator]
    bool PositionalParameters { get; }

    /// <summary>
    /// Whether the database identifiers shall be wrapped between the terminators defined in
    /// this instance, or not.
    /// </summary>
    [WithGenerator]
    bool UseTerminators { get; }

    /// <summary>
    /// Determines the left terminator to use when wrapping database identifiers, if used.
    /// </summary>
    [WithGenerator]
    char LeftTerminator { get; }

    /// <summary>
    /// Determines the right terminator to use when wrapping database identifiers, if used.
    /// </summary>
    [WithGenerator]
    char RightTerminator { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Splits the given source string into its dot-separated parts using the terminator rules
    /// of this instance. The returned array is empty only if the source string was a null one.
    /// Otherwise, each element contains the normalized part, not containing terminators, and
    /// being null if it was empty after trimming.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string?[] GetParts(string? value);
}