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
    /// <param name="symbol"></param>
    public XFieldNode(IFieldSymbol symbol) : base(symbol)
    {
        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"v_{Symbol.Name}";
    }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }

    readonly string MethodName = default!;
    readonly string ArgumentName = default!;
}