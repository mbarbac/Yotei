namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a hierarchical incremental source generator whose code is emitted using a captured
/// hierarchy so that all code that belongs to the same type will be placed into the same file,
/// per each generator.
/// <br/> Derived classes must be decorated with the '<see cref="GeneratorAttribute"/>' attribute
/// with a '<see cref="LanguageNames.CSharp"/>' argument to be recognized by the compiler.
/// <br/> The lifetime of a generator is controlled by the compiler. State should not be stored
/// directly on the generator, as there is no guarantee that the same instance will be used on a
/// subsequent generation pass.
/// </summary>
internal class CoreGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE = "Attribute";

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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
    /// <param name="syntax"></param>
    /// <param name="types"></param>
    /// <returns></returns>
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a candidate of the appropriate type, capturing the information that
    /// will be needed for source code generation purposes.
    /// </summary>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a candidate of the appropriate type, capturing the information that
    /// will be needed for source code generation purposes.
    /// </summary>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        PropertyDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a candidate of the appropriate type, capturing the information that
    /// will be needed for source code generation purposes.
    /// </summary>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked to create a candidate of the appropriate type, capturing the information that
    /// will be needed for source code generation purposes.
    /// </summary>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        MethodDeclarationSyntax syntax,
        ImmutableArray<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = attributes };

    /// <summary>
    /// Invoked by the compiler to transform given the syntax node, carried by the given context,
    /// into a source generation candidate, or to an error condition node that describes why that
    /// transformation was not possible.
    /// <br/> Inheritors must prevent returning null instances.
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
            if (symbol is null) return new ErrorCandidate(CoreDiagnostics.SymbolNotFound(syntax));

            var ats = Matches(symbol.GetAttributes(), TypeAttributes);
            return ats.Count == 0
                ? new ErrorCandidate(CoreDiagnostics.NoAttributes(syntax))
                : CreateCandidate(symbol, typeSyntax, [.. ats], model);
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol is null) return new ErrorCandidate(CoreDiagnostics.SymbolNotFound(syntax));

            var ats = Matches(symbol.GetAttributes(), MethodAttributes);
            return ats.Count == 0
                ? new ErrorCandidate(CoreDiagnostics.NoAttributes(syntax))
                : CreateCandidate(symbol, methodSyntax, [.. ats], model);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol is null) return new ErrorCandidate(CoreDiagnostics.SymbolNotFound(syntax));

            var ats = Matches(symbol.GetAttributes(), PropertyAttributes);
            return ats.Count == 0
                ? new ErrorCandidate(CoreDiagnostics.NoAttributes(syntax))
                : CreateCandidate(symbol, propertySyntax, [.. ats], model);
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
                    if (ats.Count > 0) return CreateCandidate(symbol, fieldSyntax, [.. ats], model);
                }
            }
            return symbol is null
                ? new ErrorCandidate(CoreDiagnostics.SymbolNotFound(syntax))
                : new ErrorCandidate(CoreDiagnostics.NoAttributes(syntax));
        }

        // Not supported...
        return new ErrorCandidate(CoreDiagnostics.SyntaxNotSupported(syntax));
    }

    /// <summary>
    /// Extracts the attributes that match any of the given types.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    static List<AttributeData> Matches(IEnumerable<AttributeData> attributes, Type[] types)
    {
        List<AttributeData> items = [];

        foreach (var attribute in attributes)
            foreach (var type in types)
                if (attribute.Match(type)) items.Add(attribute);

        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a top-most file node.
    /// </summary>
    protected virtual FileNode CreateFile(TypeNode node) => new(node);

    /// <summary>
    /// Invoked to create a hierarchy node of the appropriate type.
    /// </summary>
    protected virtual TypeNode CreateNode(TypeCandidate candidate)
        => new(candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };

    /// <summary>
    /// Invoked to create a hierarchy node of the appropriate type.
    /// </summary>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
        => new(parent, candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };

    /// <summary>
    /// Invoked to create a hierarchy node of the appropriate type.
    /// </summary>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
        => new(parent, candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };

    /// <summary>
    /// Invoked to create a hierarchy node of the appropriate type.
    /// </summary>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candidate)
        => new(parent, candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };

    /// <summary>
    /// Invoked to emit the source code of the collection of captured nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        if (candidates.Any(x => x is null)) throw new ArgumentException(
            "Collection of source code generation candidates carries null elements.")
            .WithData(candidates);

        var comparer = SymbolEqualityComparer.Default;
        var files = new CustomList<FileNode>()
        { AreEqual = (x, y) => comparer.Equals(x.Node.Symbol, y.Node.Symbol) };

        candidates.OfType<ErrorCandidate>().ForEach(x => x.Diagnostic.Report(context));
        candidates.OfType<TypeCandidate>().ForEach(OnExecute);
        candidates.OfType<IValidCandidate>().ForEach(x => x is not TypeCandidate, OnExecute);

        foreach (var file in files)
        {
            if (!file.Validate(context)) continue;

            var cb = new CodeBuilder(); file.Emit(context, cb);
            var code = cb.ToString();
            var name = file.FileName() + ".g.cs";
            context.AddSource(name, code);
        }

        /// <summary>
        /// Invoked to process the current candidate.
        /// </summary>
        void OnExecute(IValidCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var tpsymbol = GetMainTypeSymbol(candidate.Symbol);
            var file = files.Find(x => comparer.Equals(tpsymbol, x.Node.Symbol));
            if (file == null)
            {
                var tpsyntax = GetMainTypeSyntax(candidate.Syntax);
                var node = candidate is TypeCandidate tpcandidate
                    ? CreateNode(tpcandidate)
                    : new TypeNode(tpsymbol) { Syntax = tpsyntax };

                file = CreateFile(node);
                files.Add(file);
            }
            else if (candidate is TypeCandidate)
            {
                CoreDiagnostics.InvalidHierarchy(candidate).Report(context);
                return;
            }

            // Transforming candidates into nodes of the hierarchy...
            if (candidate is TypeCandidate)
            {
                return; // Just captured as the top-most element!
            }
            else if (candidate is PropertyCandidate propertyCandidate)
            {
                var node = CreateNode(file.Node, propertyCandidate);
                if (!file.Node.ChildProperties.Contains(node)) file.Node.ChildProperties.Add(node);
                else CoreDiagnostics.InvalidHierarchy(candidate).Report(context);
            }
            else if (candidate is FieldCandidate fieldCandidate)
            {
                var node = CreateNode(file.Node, fieldCandidate);
                if (!file.Node.ChildFields.Contains(node)) file.Node.ChildFields.Add(node);
                else CoreDiagnostics.InvalidHierarchy(candidate).Report(context);
            }
            else if (candidate is MethodCandidate methodCandidate)
            {
                var node = CreateNode(file.Node, methodCandidate);
                if (!file.Node.ChildMethods.Contains(node)) file.Node.ChildMethods.Add(node);
                else CoreDiagnostics.InvalidHierarchy(candidate).Report(context);
            }
            else
            {
                CoreDiagnostics.SyntaxNotSupported(candidate.Symbol).Report(context);
            }
        }

        /// <summary>
        /// Gets the main type element associated with the given symbol.
        /// </summary>
        static INamedTypeSymbol GetMainTypeSymbol(ISymbol symbol)
            => symbol is INamedTypeSymbol named ? named : symbol.ContainingType;

        /// <summary>
        /// Gets the main type element associated with the given syntax.
        /// </summary>
        static TypeDeclarationSyntax? GetMainTypeSyntax(SyntaxNode? syntax)
        {
            while (syntax != null)
            {
                if (syntax is TypeDeclarationSyntax type) return type;
                syntax = syntax.Parent;
            }
            return null;
        }
    }
}