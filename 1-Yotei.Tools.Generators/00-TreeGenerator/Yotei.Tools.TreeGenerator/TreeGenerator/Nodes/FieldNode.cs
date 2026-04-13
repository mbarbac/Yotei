namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured field-alike source code generation node.
/// </summary>
public record FieldNode : INode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];
}