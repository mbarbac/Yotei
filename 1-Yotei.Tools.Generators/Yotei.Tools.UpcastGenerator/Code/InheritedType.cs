namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <summary>
/// Represents an inherited type.
/// </summary>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
/// <param name="changeProperties"></param>
/// <param name="preventVirtual"></param>
internal class InheritedType(
    SimpleNameSyntax syntax,
    INamedTypeSymbol symbol,
    bool changeProperties,
    bool preventVirtual)
{
    /// <inheritdoc/>
    public override string ToString() => Symbol.EasyName(new EasyNameOptions(useGenerics: true));

    /// <summary>
    /// The syntax associated with this inherited type.
    /// </summary>
    public SimpleNameSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <summary>
    /// The symbol associated with this inherited type.
    /// </summary>
    public INamedTypeSymbol Symbol { get; } = symbol.ThrowWhenNull();

    /// <summary>
    /// Determines if the inherited properties shall be changed with a 'new' keyword, or not.
    /// The default value of this property is false.
    /// </summary>
    public bool ChangeProperties { get; } = changeProperties;

    /// <summary>
    /// Determines if the upcasted methods and properties shall be virtual-alike, or not. The
    /// default value of this property is 'false' to produce virtual or override ones.
    /// </summary>
    public bool PreventVirtual { get; } = preventVirtual;
}