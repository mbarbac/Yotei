namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="IErrorCandidate"/>
internal class ErrorCandidate : IErrorCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="diagnostic"></param>
    public ErrorCandidate(Diagnostic diagnostic) => Diagnostic = diagnostic.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Diagnostic.GetMessage();

    /// <inheritdoc/>
    public Diagnostic Diagnostic { get; }
}