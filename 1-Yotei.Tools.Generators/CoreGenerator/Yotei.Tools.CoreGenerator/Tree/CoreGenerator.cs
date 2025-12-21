namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a incremental source generator that emits code for the relevant types in their own
/// files, each captured by itself or by its belonging elements. Derived classes must be decorated
/// with a '<see cref="GeneratorAttribute"/>' attribute to be recognized by the compiler. It is
/// recommended that the attribute includes a '<see cref="LanguageNames.CSharp"/>' argument.
/// <para>
/// By default, instances of this type filter the relevant nodes by comparing the attributes that
/// decorate them with the ones specified for that node kind, and produce candidate objects that
/// carry the information needed to build the respective hierarchies and emit code.</para>
/// </summary>
internal class CoreGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attributes types used by this generator to identify type candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify property candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify field candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify method candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked at initialization time to register register post-initialization actions, such as
    /// generating additional code, reading external files, and so on.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context) { }

    /// <summary>
    /// <inheritdoc/>. By default, this method register actions to identify candidate elements
    /// whose attributes match any of the given ones for that element kind.
    /// </summary>
    /// <param name="context"></param>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Register actions....
        context.RegisterPostInitializationOutput(OnInitialized);

        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(static x => x != null)
            .Collect();

        context.RegisterSourceOutput(items, Execute);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to quickly determine if the given syntax node shall be considered
    /// as a potential candidate for source code generation, or not. Only potential candidates are
    /// passed to the transform phase.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Predicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node
            is TypeDeclarationSyntax
            or PropertyDeclarationSyntax
            or FieldDeclarationSyntax
            or MethodDeclarationSyntax;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel mode)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        PropertyDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel mode)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel mode)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        MethodDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel mode)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to transform the syntax node carried by the given context into a
    /// source code generation candidate. This method may also return '<c>null</c>' values if the
    /// node shall be ignored, or error candidates to report their diagnostics.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        while (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token);
            if (symbol is null) break;

            var ats = Matches(symbol.GetAttributes(), TypeAttributes).ToArray();
            if (ats.Length == 0) break;

            return CreateCandidate(symbol, typeSyntax, [.. ats], model);
        }

        // Properties...
        while (syntax is MethodDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol is null) break;

            var ats = Matches(symbol.GetAttributes(), PropertyAttributes).ToArray();
            if (ats.Length == 0) break;

            return CreateCandidate(symbol, propertySyntax, [.. ats], model);
        }

        // Fields...
        while (syntax is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol is null) continue;

                var ats = Matches(symbol.GetAttributes(), FieldAttributes).ToArray();
                if (ats.Length == 0) continue;

                return CreateCandidate(symbol, fieldSyntax, [.. ats], model);
            }
            break;
        }

        // Methods...
        while (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol is null) break;

            var ats = Matches(symbol.GetAttributes(), MethodAttributes).ToArray();
            if (ats.Length == 0) break;

            return CreateCandidate(symbol, methodSyntax, [.. ats], model);
        }

        // Finishing ignoring the node...
        return null!;
    }

    /// <summary>
    /// Selects from the given collection the attributes whose classes match any of the given
    /// types.
    /// </summary>
    static IEnumerable<AttributeData> Matches(IEnumerable<AttributeData> ats, Type[] types)
    {
        foreach (var at in ats)
            foreach (var type in types)
                if (at.Match(type)) yield return at;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for the captured candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
    }
}