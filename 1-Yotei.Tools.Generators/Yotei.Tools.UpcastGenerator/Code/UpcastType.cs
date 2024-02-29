namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <summary>
/// Represents a type to be upcasted.
/// </summary>
internal class UpcastType(TypeSyntax syntax, INamedTypeSymbol symbol, bool withProperties)
{
    /// <inheritdoc/>
    public override string ToString() => Symbol.ToDisplayString();

    /// <summary>
    /// The syntax of the type whose members are to be upcasted.
    /// </summary>
    public TypeSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <summary>
    /// The symbol of the type whose members are to be upcasted.
    /// </summary>
    public INamedTypeSymbol Symbol { get; } = symbol.ThrowWhenNull();

    /// <summary>
    /// Whether to upcast the inherited properties, or not.
    /// </summary>
    public bool WithProperties { get; } = withProperties;
}