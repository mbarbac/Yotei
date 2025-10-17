namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents the base class of a tree-oriented incremental source generator that emits code
/// organized by type hierarchy this instance has captured. To be recognized by the compiler,
/// derived classes must be decorated with the '<see cref="GeneratorAttribute"/>' attribute
/// with a '<see cref="LanguageNames.CSharp"/>' argument.
/// </summary>
internal class TreeGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE = "Attribute";

    // ====================================================

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked at initialization time to allow application code to register post-initialization
    /// actions, such as generating additional code, reading external files, and so on.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked automatically by the compiler to initialize the generator and to register source
    /// code generation steps via callbacks on the context.
    /// <br/> This method is INFRASTRUCTURE ONLY. It shall not be used by application code.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Register post-initialization constant actions....
        context.RegisterPostInitializationOutput(OnInitialized);

        // Capturing attribute names...
        TypeAttributeNames = CaptureAttributeNames(TypeAttributes);
        PropertyAttributeNames = CaptureAttributeNames(PropertyAttributes);
        FieldAttributeNames = CaptureAttributeNames(FieldAttributes);
        MethodAttributeNames = CaptureAttributeNames(MethodAttributes);

        // Registering actions...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(x => x != null)
            .Collect();

        // Registering source code emission...
        context.RegisterSourceOutput(items, Execute);
    }

    /// <summary>
    /// Invoked to capture the attribute names specified for the different kinds of candidates
    /// supported, so that those names are cached and can compared later.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    static string[] CaptureAttributeNames(Type[] types)
    {
        var array = new string[types.Length];
        for (int i = 0; i < types.Length; i++)
        {
            var type = types[i];
            var name = type.Name;

            if (!name.Contains(ATTRIBUTE))
            {
                var index = name.IndexOf('`');
                if (index > 0)
                {
                    var gens = name[index..];
                    name = name.Replace(gens, "");
                    name += ATTRIBUTE;
                    name += gens;
                }
                else name += ATTRIBUTE;
            }
            array[i] = name;
        }
        return array;
    }

    // ====================================================

    /// <summary>
    /// The collection of attributes types used by this generator to identify type candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] TypeAttributes { get; } = [];
    string[] TypeAttributeNames = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify property candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] PropertyAttributes { get; } = [];
    string[] PropertyAttributeNames = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify field candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] FieldAttributes { get; } = [];
    string[] FieldAttributeNames = [];

    /// <summary>
    /// The collection of attributes types used by this generator to identify method candidates
    /// for source code generation.
    /// </summary>
    protected virtual Type[] MethodAttributes { get; } = [];
    string[] MethodAttributeNames = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to quickly determine if the given syntax node shall be considered
    /// as a potential candidate for source code generation, or not.
    /// <br/> Unless overriden this method, by default, compares if any of the attributes applied
    /// to the syntax node match with any of the specified ones for that kind of syntax node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Predicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node switch
        {
            TypeDeclarationSyntax item => HasAttributes(item, TypeAttributeNames),
            PropertyDeclarationSyntax item => HasAttributes(item, PropertyAttributeNames),
            FieldDeclarationSyntax item => HasAttributes(item, FieldAttributeNames),
            MethodDeclarationSyntax item => HasAttributes(item, MethodAttributeNames),

            _ => false
        };
    }

    /// <summary>
    /// Determines if the given member is decorated by any of the given attributes by comparing
    /// the names of the attributes that decorate that member with the names of the given ones.
    /// </summary>
    bool HasAttributes(MemberDeclarationSyntax syntax, string[] types)
    {
        var ats = syntax.AttributeLists.GetAttributes().ToDebugArray();
        foreach (var at in ats)
        {
            // When using syntaxes, the names don't end with 'Attribute', but we have captured
            // the names of the specified ones with that tail.

            var name = at.Name.ShortName();
            if (!name.EndsWith(ATTRIBUTE)) name += ATTRIBUTE;

            var arity = at.Name.Arity;
            if (arity > 0) name += $"`{arity}";

            foreach (var type in types) if (name == type) return true;
        }
        return false;
    }

    // ====================================================

    /// <summary>
    /// Invoked to create a candidate of the appropriate type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a candidate of the appropriate type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        MethodDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a candidate of the appropriate type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        PropertyDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a candidate of the appropriate type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to transform given the syntax node, carried by the given context,
    /// into a valid source generation candidate, or to an error condition that describes why that
    /// transformation was not possible.
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
        if (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token);
            if (symbol is null) return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(syntax));

            var ats = Matches(symbol.GetAttributes(), TypeAttributes);
            return ats.Length == 0
                ? new ErrorCandidate(TreeDiagnostics.AttributesNotFound(syntax))
                : CreateCandidate(symbol, typeSyntax, ats, model);
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol is null) return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(syntax));

            var ats = Matches(symbol.GetAttributes(), MethodAttributes);
            return ats.Length == 0
                ? new ErrorCandidate(TreeDiagnostics.AttributesNotFound(syntax))
                : CreateCandidate(symbol, methodSyntax, ats, model);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol is null) return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(syntax));

            var ats = Matches(symbol.GetAttributes(), PropertyAttributes);
            return ats.Length == 0
                ? new ErrorCandidate(TreeDiagnostics.AttributesNotFound(syntax))
                : CreateCandidate(symbol, propertySyntax, ats, model);
        }

        // Fields...
        else if (syntax is FieldDeclarationSyntax fieldSyntax)
        {
            IFieldSymbol? symbol = null;
            var items = fieldSyntax.Declaration.Variables;
            
            foreach (var item in items)
            {
                var temp = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (temp != null)
                {
                    symbol = temp;
                    var ats = Matches(symbol.GetAttributes(), FieldAttributes);
                    if (ats.Length > 0) return CreateCandidate(symbol, fieldSyntax, ats, model);
                }
            }
            return symbol is null
                ? new ErrorCandidate(TreeDiagnostics.SymbolNotFound(syntax))
                : new ErrorCandidate(TreeDiagnostics.AttributesNotFound(syntax));
        }

        // Not supported...
        return new ErrorCandidate(TreeDiagnostics.SyntaxNotSupported(syntax));
    }

    /// <summary>
    /// Extracts the attributes that match any of the given types.
    /// </summary>
    static ImmutableArray<AttributeData> Matches(IEnumerable<AttributeData> attributes, Type[] types)
    {
        List<AttributeData> items = [];

        foreach (var attribute in attributes)
            foreach (var type in types)
                if (attribute.Match(type)) items.Add(attribute);

        return [.. items];
    }

    // ====================================================

    /// <summary>
    /// Invoked to emit the source code of the collection of captured candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        if (candidates.Any(x => x is null)) throw new ArgumentException(
            "Collection of source code generation candidates carries null elements.")
            .WithData(candidates);
    }
}