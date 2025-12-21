namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <<inheritdoc cref="TypeCandidate"/>
/// </summary>
internal class XTypeNode : TypeCandidate
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }
}