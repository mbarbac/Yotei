namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents an error condition to be reported at source code generation time.
/// </summary>
internal class ErrorNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="diagnostic"></param>
    [SuppressMessage("", "IDE0290")]
    public ErrorNode(Diagnostic diagnostic) => Diagnostic = diagnostic.ThrowWhenNull();

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