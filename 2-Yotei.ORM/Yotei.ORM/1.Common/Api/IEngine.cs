namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Describes an underlying database engine.
/// </summary>
public partial interface IEngine
{
    /// <summary>
    /// Determines if this engine treats its identifier-alike names as case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveNames { get; }

    /// <summary>
    /// The literal used by this engine to represent null or missed values.
    /// </summary>
    [WithGenerator] string NullValueLiteral { get; }

    /// <summary>
    /// Determines if this engine provides native paging capabilities, or rather they have to be
    /// emulated by the framework.
    /// </summary>
    [WithGenerator] bool NativePaging { get; }

    /// <summary>
    /// The default prefix used by this engine to identifiy the names of parameters of a command.
    /// </summary>
    [WithGenerator] string ParameterPrefix { get; }

    /// <summary>
    /// Determines if this engine treats the parameters of a command by their ordinal poisitions,
    /// or rather by their names.
    /// </summary>
    [WithGenerator] bool PositionalParameters { get; }

    /// <summary>
    /// Determines if this engine wraps the names of its identifier-alike elements, or not.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the indexes of the unwrapped ocurrences of the given char in the
    /// given value. Unwrapped ocurrences are those not protected by the terminators of this
    /// instance, if they are used.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    List<int> UnwrappedIndexes(string? value, char ch);
}