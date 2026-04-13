namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured property-alike source code generation node.
/// </summary>
public record PropertyNode : INode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];
}