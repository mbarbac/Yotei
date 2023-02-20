namespace Yotei.Generators.Tree;

// ========================================================
/// <inheritdoc cref="ICapturedField">
/// </inheritdoc>
internal class CapturedField : ICapturedField
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="capturedType"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public CapturedField(
        ICapturedType capturedType,
        FieldDeclarationSyntax syntax, IFieldSymbol symbol)
    {
        CapturedType = capturedType.ThrowIfNull(nameof(capturedType));
        FieldSyntax = syntax.ThrowIfNull(nameof(syntax));
        FieldSymbol = symbol.ThrowIfNull(nameof(symbol));

        Name = symbol.Name.NotNullNotEmpty(nameof(Name));
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"Field: {Name}";

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
    public FieldDeclarationSyntax FieldSyntax { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public IFieldSymbol FieldSymbol { get; }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual bool Validate(SourceProductionContext context) => true;

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual void Print(SourceProductionContext context, CodeBuilder cb) { }
}