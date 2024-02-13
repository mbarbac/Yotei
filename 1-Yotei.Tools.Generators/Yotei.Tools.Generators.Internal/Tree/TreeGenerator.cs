namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents the base class of tree-oriented incremental source generators. Derived ones must
/// be decorated with the <see cref="GeneratorAttribute"/> attribute to be used by the compiler,
/// using '(<see cref="LanguageNames.CSharp"/>)' as its argument.
/// </summary>
internal abstract class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        context.RegisterPostInitializationOutput(OnInitialized);

        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Validate, Transform)
            .Collect();

        context.RegisterSourceOutput(items, Print);
    }

    /// <summary>
    /// Determines if this instance will try to launch a debug session when involked by the
    /// compiler.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked to register post-initialization actions, such as generating code for custom
    /// attributes or reading external files.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine if the given node shall be considered a valid one for source code
    /// generation purposes, if the given name of any of its attributes match with any of the
    /// names defined for its node type.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            if (TypeAttributes.Length == 0) return false;

            var atts = typeSyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, TypeAttributes));
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            if (PropertyAttributes.Length == 0) return false;

            var atts = propertySyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, PropertyAttributes));
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            if (FieldAttributes.Length == 0) return false;

            var atts = fieldSyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, FieldAttributes));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            if (MethodAttributes.Length == 0) return false;

            var atts = methodSyntax.AttributeLists.GetAttributes();
            return atts.Any(x => Compare(x.Name, MethodAttributes));
        }

        // Not supported node type...
        return false;
    }

    static string AdjustNameSyntax(NameSyntax nameSyntax)
    {
        var name = nameSyntax.ShortName();
        return name.EndsWith("Attribute") ? name : name + "Attribute";
    }

    static bool Compare(NameSyntax nameSyntax, string[] targets)
    {
        var name = AdjustNameSyntax(nameSyntax);

        for (int i = 0; i < targets.Length; i++) if (name == targets[i]) return true;
        return false;
    }

    /// <summary>
    /// The collection of attribute names that, if decorate a given type, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorate a given property, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorate a given field, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorate a given method, identifies it as a
    /// potential candidate for source code generation.
    /// </summary>
    public virtual string[] MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform the given node into a candidate for source code generation.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <remarks>
    /// This implementation works by quickly capturing the model, syntax and symbol of the nodes
    /// that are supported and that have been identified by their attributes. Because the nature
    /// of this info these candidates are hardly cachable - but we want to delay the generation
    /// of the hierarchy to when we have the complete set of candidates, and so we need that
    /// information for each.
    /// </remarks>
    Candidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token)
                ?? throw new InvalidOperationException(
                "Cannot obtain symbol for type node.").WithData(typeSyntax, nameof(node));

            return new TypeCandidate(model, typeSyntax, symbol);
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token)
                ?? throw new InvalidOperationException(
                "Cannot obtain symbol for property node.").WithData(propertySyntax, nameof(node));

            return new PropertyCandidate(model, propertySyntax, symbol);
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            foreach (var item in items)
            {
                if (model.GetDeclaredSymbol(item, token) is IFieldSymbol symbol)
                    return new FieldCandidate(model, fieldSyntax, symbol);
            }
            throw new InvalidOperationException(
                "Cannot obtain symbol for field node.").WithData(fieldSyntax, nameof(node));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token)
                ?? throw new InvalidOperationException(
                "Cannot obtain symbol for method node.").WithData(methodSyntax, nameof(node));

            return new MethodCandidate(model, methodSyntax, symbol);
        }

        // Not supported...
        throw new ArgumentException("Unsupported syntax node.").WithData(node);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public virtual NamespaceNode CreateNode(
        INode parent,
        BaseNamespaceDeclarationSyntax syntax) => new(parent, syntax);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual TypeNode CreateNode(
        INode parent,
        TypeCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual PropertyNode CreateNode(
        TypeNode parent,
        PropertyCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual FieldNode CreateNode(
        TypeNode parent,
        FieldCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual MethodNode CreateNode(
        TypeNode parent,
        MethodCandidate candidate) => new(parent, candidate);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate the hierarchy for the given candidates, and then to emit their
    /// source code.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Print(SourceProductionContext context, ImmutableArray<Candidate> candidates)
    {
        var hierarchy = new Hierarchy(this);

        // Generating the hierarchy...
        foreach (var candidate in candidates)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (candidate is null) continue;

            Register(hierarchy, candidate);
        }

        // Generating the source code...
        foreach (var file in hierarchy.FileChildren)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (file.Validate(context))
            {
                file.Print(context, file);

                var code = file.GetTextCode();
                var name = file.FileName;
                context.AddSource(name + ".g.cs", code);
            }
        }
    }

    /// <summary>
    /// Registers the given candidate into the given hierarchy.
    /// </summary>
    /// <param name="hierarchy"></param>
    /// <param name="candidate"></param>
    void Register(Hierarchy hierarchy, Candidate candidate)
    {
        // File...
        FileNode fileNode;
        var fileName = candidate.GetFileName();
        var index = hierarchy.FileChildren.IndexOf(x => x.FileName == fileName);

        if (index >= 0) fileNode = hierarchy.FileChildren[index];
        else
        {
            fileNode = new(hierarchy, fileName);
            hierarchy.FileChildren.Add(fileNode);
        }

        // Namespaces...
        INode parent = fileNode;
        NamespaceNode nsNode = default!;
        ChildrenList<NamespaceNode> nsList = fileNode.NamespaceChildren;
        for (int nsIndex = 0; nsIndex < candidate.NamespaceSyntaxChain.Length; nsIndex++)
        {
            var syntax = candidate.NamespaceSyntaxChain[nsIndex];
            var name = syntax.Name.ToString();
            index = nsList.IndexOf(x => x.Name == name);

            if (index >= 0) nsNode = nsList[index];
            else
            {
                nsNode = CreateNode(parent, syntax);
                nsList.Add(nsNode);
            }

            nsList = nsNode.NamespaceChildren;
            parent = nsNode;
        }

        // Types...
        var comparer = SymbolEqualityComparer.Default;
        TypeNode tpNode = default!;
        ChildrenList<TypeNode> tpList = nsNode.TypeChildren;
        for (int tpIndex = 0; tpIndex < candidate.TypeSymbolChain.Length; tpIndex++)
        {
            var symbol = candidate.TypeSymbolChain[tpIndex];
            index = tpList.IndexOf(x => comparer.Equals(symbol, x.Symbol));

            if (index >= 0) tpNode = tpList[index];
            else
            {
                if (candidate is TypeCandidate typeCandidate &&
                    tpIndex == candidate.TypeSyntaxChain.Length - 1)
                {
                    tpNode = CreateNode(parent, typeCandidate);
                    tpList.Add(tpNode);
                    return;
                }
                else
                {
                    tpNode = new TypeNode(parent, (INamedTypeSymbol)symbol);
                    tpList.Add(tpNode);
                }
            }

            tpList = tpNode.TypeChildren;
            parent = tpNode;
        }

        // Properties...
        if (candidate is PropertyCandidate propertyCandidate)
        {
            var node = CreateNode(tpNode, propertyCandidate);
            tpNode.PropertyChildren.Add(node);
            return;
        }

        // Fields...
        if (candidate is FieldCandidate fieldCandidate)
        {
            var node = CreateNode(tpNode, fieldCandidate);
            tpNode.FieldChildren.Add(node);
            return;
        }

        // Methods...
        if (candidate is MethodCandidate methodCandidate)
        {
            var node = CreateNode(tpNode, methodCandidate);
            tpNode.MethodChildren.Add(node);
            return;
        }

        // Unknown...
        throw new InvalidOperationException("Unknown candidate type.").WithData(candidate);
    }
}