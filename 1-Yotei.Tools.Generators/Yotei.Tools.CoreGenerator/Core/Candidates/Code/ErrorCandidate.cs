namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an error condition while transforming a syntax node into a candidate for source
/// code generation.
/// </summary>
internal sealed class ErrorCandidate : ICandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="diagnostic"></param>
    public ErrorCandidate(Diagnostic diagnostic)
    {
        throw null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Diagnostic.GetMessage();

    /// <summary>
    /// The diagnostic that describes the error situation.
    /// </summary>
    public Diagnostic Diagnostic { get; }
}