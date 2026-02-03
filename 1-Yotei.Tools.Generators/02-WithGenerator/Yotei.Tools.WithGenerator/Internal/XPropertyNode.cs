namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }
}