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
    /// The prefix used when the framework needs to generate names for the parameters of a command.
    /// </summary>
    [WithGenerator] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if the database treats the parameters of a command by their ordinal poisitions,
    /// or rather by their names.
    /// </summary>
    [WithGenerator] bool PositionalParameters { get;}

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

    /// <summary>
    /// The collection of metadata tags that are well-known to this engine, for the purposes
    /// of the framework.
    /// </summary>
    [WithGenerator] IKnownTags KnownTags { get; }

    /// <summary>
    /// Returns a list with the indexes of the unwrapped ocurrences of the given character in
    /// the given source value. Unwrapped ocurrences are those not protected by the terminators
    /// in this engine, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    List<int> GetUnwrappedIndexes(string? value, char ch);
}