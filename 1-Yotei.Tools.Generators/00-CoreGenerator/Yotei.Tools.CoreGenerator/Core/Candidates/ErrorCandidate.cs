namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an error condition detected while capturing source code generation candidates.
/// This error will be later reported in source code emitting phase.
/// </summary>
internal class ErrorCandidate : ICandidate
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
    /// The diagnostic carried by this instance.
    /// </summary>
    public Diagnostic Diagnostic { get; }
}