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
    CustomList<Diagnostic> Diagnostics { get; }

    /// <summary>
    /// The parent node this instance belongs to in the source code generation hierarchy, or null
    /// if it is a detached one. If not null, then this property is just for hierarchy purposes,
    /// and needs not to represent the declaring element of the one represented by this instance.
    /// </summary>
    INode? Parent { get; }
}