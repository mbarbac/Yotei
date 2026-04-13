namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured method-alike source code generation node.
/// </summary>
public record MethodNode : INode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];
}