namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents the base class of tree-oriented incremental source code generators. Derived ones
/// must be decorated with the <see cref="GeneratorAttribute"/> attribute to be recognized by
/// the compiler, using '<c>(LanguageNames.CSharp)</c>' as its argument.
/// </summary>
internal abstract class BaseGenerator : IIncrementalGenerator
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
    /// Determines if this instance will try to launch a debugging session when invoked by the
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
    /// Invoked to determine if the given syntax node shall be considered a valid one, or not.
    /// <br/> By default, this method validates a node when the name of any of its attributes
    /// match any of the names defined for its node type.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            if (TypeAttributes.Length > 0)
            {
                var atts = typeSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, TypeAttributes));
            }
            return false;
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            if (PropertyAttributes.Length > 0)
            {
                var atts = propertySyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, PropertyAttributes));
            }
            return false;
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            if (FieldAttributes.Length > 0)
            {
                var atts = fieldSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, FieldAttributes));
            }
            return false;
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            if (MethodAttributes.Length > 0)
            {
                var atts = methodSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, MethodAttributes));
            }
            return false;
        }

        // Not supported...
        return false;

        /// <summary>
        /// Invoked to determine if the given attribute name matches any of the target ones.
        /// </summary>
        static bool Compare(NameSyntax syntax, string[] targets)
        {
            var span = "Attribute".AsSpan();
            var source = syntax.ShortName().AsSpan().RemoveLast(span);

            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i].AsSpan().RemoveLast(span);
                if (source.CompareTo(target, StringComparison.Ordinal) == 0) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// The collection of attribute names that, if decorates a given type, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorates a given property, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorates a given field, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorates a given method, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform the given syntax node into a candidate for source generation.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual Candidate Transform(GeneratorSyntaxContext context, CancellationToken token)
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
    /// Invoked to emit the source code of the validated candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    protected virtual void Print(SourceProductionContext context, ImmutableArray<Candidate> candidates)
    {
        var files = new Dictionary<string, FileBuilder>();

        // Generating the collection of files...
        foreach (var candidate in candidates)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (candidate is null) continue;

            var name = candidate.GetFileName();
            if (!files.TryGetValue(name, out var file))
            {
                file = new FileBuilder(this, name);
                files.Add(name, file);
            }
            file.Register(candidate);
        }

        // Generating the source code...
        foreach (var kvp in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var name = kvp.Key;
            var file = kvp.Value;

            if (file.Validate(context))
            {
                var builder = new CodeBuilder();
                file.Print(context, builder);

                var code = builder.ToString();
                context.AddSource(name + ".g.cs", code);
            }
        }
    }
}