namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XFieldNode : FieldNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }
}