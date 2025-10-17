namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an error situation in the source code generation process.
/// </summary>
internal sealed class ErrorNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="diagnostic"></param>
    public ErrorNode(Diagnostic diagnostic) => Diagnostic = diagnostic.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Diagnostic.GetMessage();

    /// <summary>
    /// The diagnostic that describes the error situation.
    /// </summary>
    public Diagnostic Diagnostic { get; }
}