namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a candidate that carries an error diagnostic.
/// </summary>
internal interface IErrorCandidate : ICandidate
{
    /// <summary>
    /// The diagnostic that describes the error represented by this object.
    /// </summary>
    Diagnostic Diagnostic { get; }
}