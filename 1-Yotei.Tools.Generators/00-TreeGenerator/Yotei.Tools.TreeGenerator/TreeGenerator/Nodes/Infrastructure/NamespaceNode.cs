namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in a hierarchy.
/// </summary>
public sealed class NamespaceNode : INode
{

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CustomList<Diagnostic> Diagnostics { get; } = [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode? Parent
    {
        get;
        set
        {
            if (value is not null
                and not FileNode
                and not NamespaceNode)
                throw new ArgumentException(
                    "Invalid parent node.").WithData(value);

            field = value;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// Equality semantics are customized for generator caching purposes.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not NamespaceNode valid) return false;

        // TODO: NamespaceNode Equals...
        if (!Diagnostics.SequenceEqual(valid.Diagnostics)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int code = HashCode.Combine(Diagnostics);
        foreach (var item in Diagnostics) code = HashCode.Combine(code, item);

        // TODO: NamespaceNode GetHashCode...
        return code;
    }
}