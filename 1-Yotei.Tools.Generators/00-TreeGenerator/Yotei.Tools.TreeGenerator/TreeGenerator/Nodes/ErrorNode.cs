namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a generic error node that produces no generated code, but only carries diagnostics.
/// </summary>
public record ErrorNode : INode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Diagnostics.Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(INode other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Diagnostics.SequenceEqual(other.Diagnostics);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];
}