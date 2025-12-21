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
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        var item = new TypeCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

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
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        var item = new PropertyCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

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
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        var item = new FieldCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

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
        IEnumerable<AttributeData> attributes,
        SemanticModel mode)
    {
        var item = new MethodCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked by the compiler to transform the syntax node carried by the given context into a
    /// source code generation candidate. This method may also return '<c>null</c>' values if the
    /// node shall be ignored, or error candidates to report their diagnostics.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        static IEnumerable<AttributeData> FindAttributes(
            ISymbol symbol, MemberDeclarationSyntax syntax, Type[] types)
        {
            var ats = symbol.GetAttributes(syntax).ToDebugArray();
            var items = ats.Where(x =>
                x.AttributeClass is not null &&
                x.AttributeClass.MatchAny(types)).ToDebugArray();

            return items;
        }

        // Types...
        while (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, typeSyntax, TypeAttributes).ToDebugArray();
            if (!ats.Any()) break;

            return CreateCandidate(symbol, typeSyntax, [.. ats], model);
        }

        // Properties...
        while (syntax is MethodDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, propertySyntax, PropertyAttributes).ToDebugArray();
            if (!ats.Any()) break;

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

                var ats = FindAttributes(symbol, fieldSyntax, FieldAttributes).ToDebugArray();
                if (!ats.Any()) break;

                return CreateCandidate(symbol, fieldSyntax, [.. ats], model);
            }
            break;
        }

        // Methods...
        while (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, methodSyntax, MethodAttributes).ToDebugArray();
            if (!ats.Any()) break;

            return CreateCandidate(symbol, methodSyntax, [.. ats], model);
        }

        // Finishing ignoring the node...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new file to carry the given type.
    /// </summary>
    protected virtual FileNode CreateFile(TypeNode node) => new(node);

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual TypeNode CreateNode(TypeCandidate candidate)
    {
        var item = new TypeNode(candidate.Symbol) { Syntax = candidate.Syntax };
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
    {
        var item = new PropertyNode(parent, candidate.Symbol) { Syntax = candidate.Syntax };
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
    {
        var item = new FieldNode(parent, candidate.Symbol) { Syntax = candidate.Syntax };
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candidate)
    {
        var item = new MethodNode(parent, candidate.Symbol) { Syntax = candidate.Syntax };
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for the captured candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        // Removing null candidates...
        Extract(candidates, out candidates, static x => x is null);

        // Reporting error candidates...
        Extract(candidates, out candidates, static x => x is ErrorCandidate)
            .ForEach(x => ((ErrorCandidate)x).Diagnostic.Report(context));

        // Creating hierarchy for types...
        var comparer = SymbolEqualityComparer.Default;
        var files = new List<FileNode>();
        Extract(candidates, out candidates, static x => x is TypeCandidate)
            .ForEach(x => OnExecute((IValidCandidate)x));

        // Remaining candidates...
        candidates.ForEach(x => OnExecute((IValidCandidate)x));

        // Finishing...
        foreach (var file in files)
        {
            // HIGH: emit soure code for each file...
        }

        /// <summary>
        /// Invoked to process the given candidate...
        /// </summary>
        void OnExecute(IValidCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var tpcandidate = candidate as TypeCandidate;
            var tpsymbol = FindTypeSymbol(candidate.Symbol);
            var file = files.Find(x => comparer.Equals(tpsymbol, x.Node.Symbol));

            if (file is null)
            {
                var tpsyntax = FindTypeDeclaration(candidate.Syntax);
                var node = tpcandidate is null
                    ? new TypeNode(tpsymbol) { Syntax = tpsyntax } // Not a type candidate
                    : CreateNode(tpcandidate);

                file = CreateFile(node);
                files.Add(file);
            }
            else if (tpcandidate is not null)
            {
                // LOW: add a collection of syntaxes for duplicated type elements...
                // So far, we just add the newly captured attributes...
                file.Attributes.AddRange(candidate.Attributes);
            }

            // HIGH: remaining candidate kinds in the hierarchy...
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the type symbol associated with the given element.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    static INamedTypeSymbol FindTypeSymbol(ISymbol symbol)
    {
        return symbol is INamedTypeSymbol named ? named : symbol.ContainingType;
    }

    /// <summary>
    /// Finds the type declaration associated with the given element.
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    static TypeDeclarationSyntax? FindTypeDeclaration(SyntaxNode? syntax)
    {
        while (syntax != null)
        {
            if (syntax is TypeDeclarationSyntax type) return type;
            syntax = syntax.Parent;
        }
        return null;
    }

    /// <summary>
    /// Returns from the given collection of element those that match the given predicate. The
    /// remaining ones are returned in the out argument.
    /// </summary>
    static IEnumerable<T> Extract<T>(
        ImmutableArray<T> items, out ImmutableArray<T> remaining, Predicate<T> predicate)
    {
        List<T> found = [];
        List<T> survived = [];

        foreach (var item in items)
        {
            if (predicate(item)) found.Add(item);
            else survived.Add(item);
        }
        remaining = [.. survived];
        return found;
    }

}