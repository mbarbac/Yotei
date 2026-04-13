namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured source code generation node or an error condition.
/// </summary>
public interface INode : IEquatable<INode>
{
    /// <summary>
    /// Gets the collection of diagnostics collected for this node. If this collection contains
    /// at least one element with a '<see cref="DiagnosticSeverity.Error"/>' severity, then source
    /// code generation is disabled.
    /// </summary>
    List<Diagnostic> Diagnostics { get; }
}