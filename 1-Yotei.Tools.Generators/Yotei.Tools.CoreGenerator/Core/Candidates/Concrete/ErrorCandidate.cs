namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an error condition while capturing a source code generation candidate.
/// </summary>
public sealed class ErrorCandidate : ICandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="diagnostic"></param>
    public ErrorCandidate(Diagnostic diagnostic) => Diagnostic = diagnostic.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Diagnostic.GetMessage();

    /// <summary>
    /// The diagnostic that describes the error situation.
    /// </summary>
    public Diagnostic Diagnostic { get; }

    // ----------------------------------------------------

    ISymbol ICandidate.Symbol => throw new NotSupportedException("Error candidate.");
    SyntaxNode? ICandidate.Syntax => throw new NotSupportedException("Error candidate.");
    ImmutableArray<AttributeData> ICandidate.Attributes => throw new NotSupportedException("Error candidate.");
}