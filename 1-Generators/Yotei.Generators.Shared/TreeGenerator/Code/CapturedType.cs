namespace Yotei.Generators.Tree;

// ========================================================
/// <inheritdoc cref="ICapturedType">
/// </inheritdoc>
internal class CapturedType : ICapturedType
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="semantic"></param>
    /// <param name="generator"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <exception cref="ArgumentException"></exception>
    public CapturedType(
        SemanticModel semantic, IGenerator generator,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    {
        SemanticModel = semantic.ThrowIfNull(nameof(semantic));
        Generator = generator.ThrowIfNull(nameof(generator));
        TypeSyntax = syntax.ThrowIfNull(nameof(syntax));
        TypeSymbol = symbol.ThrowIfNull(nameof(symbol));

        if (symbol.IsNamespace)
            throw new ArgumentException($"Type symbol cannot be a namespace: {symbol.Name}");

        NamespaceSyntaxChain = TypeSyntax.NamespaceSyntaxChain();
        TypeSyntaxChain = TypeSyntax.TypeSyntaxChain();
        TypeSymbolChain = TypeSymbol.TypeSymbolChain();
        Name = symbol.ShortName().NotNullNotEmpty(nameof(Name));
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"Type: {Name}";

    /// <inheritdoc>
    /// </inheritdoc>
    public SemanticModel SemanticModel { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public IGenerator Generator { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public TypeDeclarationSyntax TypeSyntax { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public INamedTypeSymbol TypeSymbol { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public string Name { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public bool IsInterface => TypeSyntax is InterfaceDeclarationSyntax;

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual bool Validate(SourceProductionContext context)
    {
        // Type must always be a partial one...
        if (!TypeSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                "Kerosene",
                "Type must be partial",
                "The type '{0}' is not a partial one.",
                "Build",
                DiagnosticSeverity.Error,
                true),
                TypeSyntax.GetLocation(),
                new object[] { Name }));

            return false;
        }

        // Validations passed...
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual void Print(SourceProductionContext context, CodeBuilder cb)
    {
        var kind = TypeSyntax.Keyword.ToString();
        var name = GetNameAndInterfaces();

        cb.AppendLine($"partial {kind} {name}");
        cb.AppendLine("{");

        cb.Tabs++; OnPrint(context, cb);
        cb.Tabs--;

        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to obtain the name of this instance or, if it needs to implement any interfaces,
    /// then that name followed by these interfaces using a " : comma-separated-names" format.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetNameAndInterfaces() => Name;

    /// <summary>
    /// Invoked to print the code of the contents of this instance.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void OnPrint(SourceProductionContext context, CodeBuilder cb) { }
}