namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured property-alike source code generation node.
/// </summary>
public class PropertyNode : ITreeNode
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
    public INode? Parent { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CustomList<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IPropertySymbol Symbol { get; }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CustomList<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// Equality semantics are customized for generator caching purposes.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not MethodNode valid) return false;

        // TODO: PropertyNode Equals...
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

        // TODO: PropertyNode GetHashCode...
        return code;
    }
}