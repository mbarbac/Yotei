namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in a hierarchy.
/// </summary>
public sealed class NamespaceNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="displayName"></param>
    public NamespaceNode(string displayName) => DisplayName = displayName;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Namespace: {DisplayName}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string DisplayName { get; private set => field = value.NotNullNotEmpty(trim: true); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<Diagnostic> Diagnostics { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
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
        if (DisplayName != valid.DisplayName) return false;
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
        code = HashCode.Combine(code, DisplayName);
        return code;
    }
}