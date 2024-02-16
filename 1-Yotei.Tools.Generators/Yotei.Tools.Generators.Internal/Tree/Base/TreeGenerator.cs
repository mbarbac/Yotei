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
    /// Invoked to determine if the given syntax node is valid for this source code generator.
    /// <br/> The default behavior is to consider valid those nodes that are decorated with at
    /// least one attribute whose name matches any of the names defined for that syntax node kind.
    /// <br/> You can override this method to use any other validation logic as you wish.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            if (TypeAttributes.Length == 0) return false;

            var ats = typeSyntax.AttributeLists.GetAttributes();
            return ats.Any(x => Compare(x.Name, TypeAttributes));
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            if (PropertyAttributes.Length == 0) return false;

            var ats = propertySyntax.AttributeLists.GetAttributes();
            return ats.Any(x => Compare(x.Name, PropertyAttributes));
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            if (FieldAttributes.Length == 0) return false;

            var ats = fieldSyntax.AttributeLists.GetAttributes();
            return ats.Any(x => Compare(x.Name, FieldAttributes));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            if (MethodAttributes.Length == 0) return false;

            var ats = methodSyntax.AttributeLists.GetAttributes();
            return ats.Any(x => Compare(x.Name, MethodAttributes));
        }

        // Not supported...
        return false;
    }

    static bool Compare(NameSyntax nameSyntax, string[] targets)
    {
        var name = nameSyntax.LongName();
        if (!name.EndsWith("Attribute")) name += "Attribute";

        for (int i = 0; i < targets.Length; i++) if (name == targets[i]) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of long attribute names that, if decorate a given type, identifies it
    /// as a candidate for source code generation.
    /// </summary>
    public virtual string[] TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of long attribute names that, if decorate a given property, identifies it
    /// as a candidate for source code generation.
    /// </summary>
    public virtual string[] PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of long attribute names that, if decorate a given field, identifies it
    /// as a candidate for source code generation.
    /// </summary>
    public virtual string[] FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of long attribute names that, if decorate a given method, identifies it
    /// as a candidate for source code generation.
    /// </summary>
    public virtual string[] MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// This private method transforms the validated nodes into candidates, by invoking the
    /// appropriate candidate creation method the generator implementor can override. Because
    /// the nature of the information (semantic mode, symbols), these candidates are hardly
    /// cacheable - but we need this information to generate the source code hierarchy, so
    /// we are going to accept this.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token)
                ?? throw new ArgumentException(
                "Cannot obtain symbol for type node.").WithData(typeSyntax, nameof(node));

            return CreateNode(model, typeSyntax, symbol);
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token)
                ?? throw new ArgumentException(
                "Cannot obtain symbol for property node.").WithData(propertySyntax, nameof(node));

            return CreateNode(model, propertySyntax, symbol);
        }
        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            foreach (var item in items)
            {
                if (model.GetDeclaredSymbol(item, token) is IFieldSymbol symbol)
                    return CreateNode(model, fieldSyntax, symbol);
            }

            throw new ArgumentException(
                "Cannot obtain symbol for field node.").WithData(fieldSyntax, nameof(node));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token)
                ?? throw new ArgumentException(
                "Cannot obtain symbol for method node.").WithData(methodSyntax, nameof(node));

            return CreateNode(model, methodSyntax, symbol);
        }

        // Not supported...
        throw new ArgumentException("Unsupported syntax node.").WithData(node);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Creates a node of the appropriate type.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual TypeNode CreateNode(
        SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        => new(model, syntax, symbol);

    /// <summary>
    /// Creates a node of the appropriate type.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual PropertyNode CreateNode(
        SemanticModel model, PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
        => new(model, syntax, symbol);

    /// <summary>
    /// Creates a node of the appropriate type.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual FieldNode CreateNode(
        SemanticModel model, FieldDeclarationSyntax syntax, IFieldSymbol symbol)
        => new(model, syntax, symbol);

    /// <summary>
    /// Creates a node of the appropriate type.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual MethodNode CreateNode(
        SemanticModel model, MethodDeclarationSyntax syntax, IMethodSymbol symbol)
        => new(model, syntax, symbol);

    // ----------------------------------------------------

    /// <summary>
    /// This private method is in charge of generating the source code generation hierarchy
    /// and then invoke the validation and print procedures in each.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Print(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        // Generating the hierarchy...
        context.CancellationToken.ThrowIfCancellationRequested();
        var hierarchy = new Hierarchy();
        hierarchy.RegisterTypeCandidates(candidates);
        hierarchy.RegisterOtherCandidates(candidates);

        // Validating and emitting...
        foreach (var file in hierarchy.ChildFiles)
        {
            if (!file.Validate(context)) continue;

            var cb = new CodeBuilder(); file.Emit(context, cb);
            var code = cb.GetTextCode();
            var name = file.FileName + ".g.cs";
            context.AddSource(name, code);
        }
    }
}