namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an error condition detected while capturing a source code generation candidate.
/// Instances of this type are not transformed into hierarchy nodes.
/// </summary>
internal sealed class ErrorCandidate : ICandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="diagnostic"></param>
    [SuppressMessage("", "IDE0290")]
    public ErrorCandidate(Diagnostic diagnostic) => Diagnostic = diagnostic.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Diagnostic.GetMessage();

    /// <summary>
    /// The diagnostic of the error represented by this instance.
    /// </summary>
    public Diagnostic Diagnostic { get; }
}