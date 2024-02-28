namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <summary>
/// Represents a type to be upcasted.
/// </summary>
public class UpcastType(INamedTypeSymbol symbol, bool withProperties)
{
    /// <summary>
    /// The symbol whose members are to be upcasted.
    /// </summary>
    public INamedTypeSymbol Symbol { get; } = symbol.ThrowWhenNull();

    /// <summary>
    /// Whether to upcast the inherited properties, or not.
    /// </summary>
    public bool WithProperties { get; } = withProperties;
}