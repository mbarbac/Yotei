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
    /// Invoked to create a source generation node of the requested type. If the node cannot be
    /// created then this method shall return an error one with the appropriate diagnostic.
    /// </summary>
    /// <returns></returns>
    protected virtual INode CreateNode(
        SemanticModel model,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol,
        ImmutableArray<AttributeData> attributes) => throw null;

    /// <summary>
    /// Invoked to create a source generation node of the requested type. If the node cannot be
    /// created then this method shall return an error one with the appropriate diagnostic.
    /// </summary>
    /// <returns></returns>
    protected virtual INode CreateNode(
        SemanticModel model,
        MethodDeclarationSyntax syntax,
        IMethodSymbol symbol,
        ImmutableArray<AttributeData> attributes) => throw null;

    /// <summary>
    /// Invoked to create a source generation node of the requested type. If the node cannot be
    /// created then this method shall return an error one with the appropriate diagnostic.
    /// </summary>
    /// <returns></returns>
    protected virtual INode CreateNode(
        SemanticModel model,
        PropertyDeclarationSyntax syntax,
        IPropertySymbol symbol,
        ImmutableArray<AttributeData> attributes) => throw null;

    /// <summary>
    /// Invoked to create a source generation node of the requested type. If the node cannot be
    /// created then this method shall return an error one with the appropriate diagnostic.
    /// </summary>
    /// <returns></returns>
    protected virtual INode CreateNode(
        SemanticModel model,
        FieldDeclarationSyntax syntax,
        IFieldSymbol symbol,
        ImmutableArray<AttributeData> attributes) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to transform given the syntax node, carried by the given context,
    /// into a valid source generation one, or to an error condition that describes why that
    /// transformation was not possible.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual INode Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token);
            if (symbol == null) return new ErrorNode(TreeDiagnostics.SymbolNotFound(syntax));

            var ats = Filter(symbol.GetAttributes(), TypeAttributes);
            return ats.Length == 0
                ? new ErrorNode(TreeDiagnostics.AttributesNotFound(typeSyntax))
                : CreateNode(model, typeSyntax, symbol, ats);
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol == null) return new ErrorNode(TreeDiagnostics.SymbolNotFound(syntax));

            var ats = Filter(symbol.GetAttributes(), TypeAttributes);
            return ats.Length == 0
                ? new ErrorNode(TreeDiagnostics.AttributesNotFound(methodSyntax))
                : CreateNode(model, methodSyntax, symbol, ats);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol == null) return new ErrorNode(TreeDiagnostics.SymbolNotFound(syntax));

            var ats = Filter(symbol.GetAttributes(), TypeAttributes);
            return ats.Length == 0
                ? new ErrorNode(TreeDiagnostics.AttributesNotFound(propertySyntax))
                : CreateNode(model, propertySyntax, symbol, ats);
        }

        // Fields...
        else if (syntax is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            IFieldSymbol? centinel = null;

            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol != null)
                {
                    centinel = symbol;
                    var ats = Filter(symbol.GetAttributes(), TypeAttributes);
                    if (ats.Length != 0) return CreateNode(model, fieldSyntax, symbol, ats);
                }
            }

            return centinel is null
                ? new ErrorNode(TreeDiagnostics.SymbolNotFound(syntax))
                : new ErrorNode(TreeDiagnostics.AttributesNotFound(fieldSyntax));
        }

        // Not supported...
        return new ErrorNode(TreeDiagnostics.SyntaxNotSupported(syntax));
    }

    /// <summary>
    /// Extracts the attributes that match any of the given types.
    /// </summary>
    static ImmutableArray<AttributeData> Filter(IEnumerable<AttributeData> attributes, Type[] types)
    {
        List<AttributeData> items = [];

        foreach (var attribute in attributes)
            foreach (var type in types)
                if (attribute.Match(type)) items.Add(attribute);

        return [.. items];
    }

    // ====================================================

    /// <summary>
    /// Invoked to emit the source code of the collection of captured nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<INode> candidates)
    {
        throw null;
    }
}