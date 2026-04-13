namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured method-alike source code generation node.
/// </summary>
public class MethodNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="displayName"></param>
    public MethodNode(TypeNode parent, string displayName)
    {
        Parent = parent;
        DisplayName = displayName;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Method: {DisplayName}";

    // ----------------------------------------------------

    /// <summary>
    /// The parent node this one belongs to in the hierarchy, that needs not to be the same as the
    /// declaring element of this instance.
    /// </summary>
    public TypeNode Parent { get; private set => field = value.ThrowWhenNull(); }

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
    public virtual bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not MethodNode valid) return false;

        // TODO: MethodNode Equals...
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

        // TODO: MethodNode GetHashCode...
        code = HashCode.Combine(code, DisplayName);
        return code;
    }
}