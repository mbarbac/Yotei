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
internal class TreeGenerator : IIncrementalGenerator
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
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = [.. attributes] };

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        PropertyDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = [.. attributes] };

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = [.. attributes] };

    /// <summary>
    /// Invoked to create a valid candidate for source code generation. Inheritors may choose
    /// what elements to use to cache in the returned object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        MethodDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => new(symbol) { Syntax = syntax, Attributes = [.. attributes] };

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

            var candidate = CreateCandidate(symbol, typeSyntax, [.. ats], model);
            return candidate;
        }

        // Properties...
        while (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, propertySyntax, PropertyAttributes).ToDebugArray();
            if (!ats.Any()) break;

            var candidate = CreateCandidate(symbol, propertySyntax, [.. ats], model);
            return candidate;
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

                var candidate = CreateCandidate(symbol, fieldSyntax, [.. ats], model);
                return candidate;
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

            var candidate = CreateCandidate(symbol, methodSyntax, [.. ats], model);
            return candidate;
        }

        // Finishing ignoring the node...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new file node.
    /// </summary>
    protected virtual FileNode CreateFileNode(TypeNode node) => new(node);

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual TypeNode CreateNode(TypeCandidate candidate)
    {
        var item = new TypeNode(candidate.Symbol);
        if (candidate.Syntax is not null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
    {
        var item = new PropertyNode(parent, candidate.Symbol);
        if (candidate.Syntax is not null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
    {
        var item = new FieldNode(parent, candidate.Symbol);
        if (candidate.Syntax is not null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candidate)
    {
        var item = new MethodNode(parent, candidate.Symbol);
        if (candidate.Syntax is not null) item.SyntaxNodes.Add(candidate.Syntax);
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
        // Error candidates...
        candidates.ForEach(
            x => x is ErrorCandidate,
            x => ((ErrorCandidate)x).Diagnostic.Report(context));

        // Capturing hierarchy...
        var comparer = SymbolEqualityComparer.Default;
        List<FileNode> files = [];

        candidates.ForEach(
            x => x is IValidCandidate,
            x => Capture((IValidCandidate)x));

        // Finishing...
        foreach (var file in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!file.Validate(context)) continue;
            var cb = new CodeBuilder();
            file.Emit(context, cb);

            var code = cb.ToString();
            var name = file.FileName() + ".g.cs";
            context.AddSource(name, code);
        }

        /// <summary>
        /// Invoked to capture in the hierarchy the given candidate.
        /// </summary>
        void Capture(IValidCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var tpcandidate = candidate as TypeCandidate;
            var tpsymbol = candidate.Symbol is INamedTypeSymbol named
                ? named
                : candidate.Symbol.ContainingType;

            // Capturing the file-alike element...            
            var file = files.Find(x => comparer.Equals(tpsymbol, x.Node.Symbol));
            if (file == null)
            {
                var node = tpcandidate is not null
                    ? CreateNode(tpcandidate)
                    : new TypeNode(tpsymbol) { IsAutoGenerated = true };

                file = CreateFileNode(node);
                files.Add(file);
            }
            else if (tpcandidate is not null)
            {
                if (file.Node.IsAutoGenerated)
                {
                    var node = CreateNode(tpcandidate);
                    node.Augment(file.Node);

                    files.Remove(file);
                    file = CreateFileNode(node);
                    files.Add(file);
                }
                else file.Node.Augment(tpcandidate);
            }

            // Capturing property-alike elements...
            if (candidate is PropertyCandidate propertyCandidate)
            {
                var node = file.Node.ChildProperties.Find(
                    x => comparer.Equals(x.Symbol, propertyCandidate.Symbol));

                if (node is null)
                {
                    node = CreateNode(file.Node, propertyCandidate);
                    file.Node.ChildProperties.Add(node);
                }
                else node.Augment(propertyCandidate);
            }

            // Capturing field-alike elements...
            else if (candidate is FieldCandidate fieldCandidate)
            {
                var node = file.Node.ChildFields.Find(
                    x => comparer.Equals(x.Symbol, fieldCandidate.Symbol));

                if (node is null)
                {
                    node = CreateNode(file.Node, fieldCandidate);
                    file.Node.ChildFields.Add(node);
                }
                else node.Augment(fieldCandidate);
            }

            // Capturing method-alike elements...
            else if (candidate is MethodCandidate methodCandidate)
            {
                var node = file.Node.ChildMethods.Find(
                    x => comparer.Equals(x.Symbol, methodCandidate.Symbol));

                if (node is null)
                {
                    node = CreateNode(file.Node, methodCandidate);
                    file.Node.ChildMethods.Add(node);
                }
                else node.Augment(methodCandidate);
            }
        }
    }
}