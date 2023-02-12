namespace Yotei.Generators.Tree;

// ========================================================
/// <inheritdoc cref="ICapturedProperty">
/// </inheritdoc>
public class CapturedProperty : ICapturedProperty
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="capturedType"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public CapturedProperty(
        ICapturedType capturedType,
        PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    {
        CapturedType = capturedType.ThrowIfNull(nameof(capturedType));
        PropertySyntax = syntax.ThrowIfNull(nameof(syntax));
        PropertySymbol = symbol.ThrowIfNull(nameof(symbol));

        Name = symbol.Name.NotNullNotEmpty(nameof(Name));
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"Property: {Name}";

    /// <inheritdoc>
    /// </inheritdoc>
    public SemanticModel SemanticModel => CapturedType.SemanticModel;

    /// <inheritdoc>
    /// </inheritdoc>
    public IGenerator Generator => CapturedType.Generator;

    /// <inheritdoc>
    /// </inheritdoc>
    public ICapturedType CapturedType { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public string Name { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public PropertyDeclarationSyntax PropertySyntax { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public IPropertySymbol PropertySymbol { get; }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual bool Validate(SourceProductionContext context) => true;

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual void Print(SourceProductionContext context, CodeBuilder cb) { }
}