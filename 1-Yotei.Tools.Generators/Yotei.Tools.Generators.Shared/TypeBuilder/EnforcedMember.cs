namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents an optional enforced member whose value will be replaced while obtaining a new
/// instance of its hosting type.
/// </summary>
internal class EnforcedMember
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="valueName"></param>
    public EnforcedMember(IPropertySymbol symbol, string valueName)
    {
        Property = symbol.ThrowWhenNull(nameof(symbol));
        ValueName = valueName.NotNullNotEmpty(nameof(valueName));
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="valueName"></param>
    public EnforcedMember(IFieldSymbol symbol, string valueName)
    {
        Field = symbol.ThrowWhenNull(nameof(symbol));
        ValueName = valueName.NotNullNotEmpty(nameof(valueName));
    }

    /// <summary>
    /// The symbol of the property member, or null.
    /// </summary>
    IPropertySymbol? Property { get; }

    /// <summary>
    /// The symbol of the field member, or null.
    /// </summary>
    IFieldSymbol? Field { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Name}={ValueName})";

    // ----------------------------------------------------

    /// <summary>
    /// The symbol of the member this instance refers to.
    /// </summary>
    public ISymbol Symbol => (ISymbol)Property! ?? (ISymbol)Field!;

    /// <summary>
    /// The name of the member this instance refers to.
    /// </summary>
    public string Name => Symbol.Name;

    /// <summary>
    /// The external variable name from which to obtain the new value of this member.
    /// </summary>
    public string ValueName { get; }
}