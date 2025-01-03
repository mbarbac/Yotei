namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents the base class of tree-oriented incremental source code generators. Derived
/// classes must be decorated with the <see cref="GeneratorAttribute"/> attribute, with a
/// <see cref="LanguageNames.CSharp"/> argument, to be invoked by the compiler.
/// </summary>
internal class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if this instance tries to launch a debug session when it is invoked by the
    /// compiler, or not.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked after initialization to register constant post-initialization actions, such as
    /// generating aditional code, or reading extenal files.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialized(IncrementalGeneratorPostInitializationContext context) { }

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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked automatically by the compiler to initialize the generator and to register source
    /// code generation steps via callbacks on the context. Although this method is public, it is
    /// just infrastructure and shall not be called from user code.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Register post-initialization constant actions....
        context.RegisterPostInitializationOutput(OnInitialized);

        // Capturing attribute names...
        TypeAttributeNames = CaptureNames(TypeAttributes);
        PropertyAttributeNames = CaptureNames(PropertyAttributes);
        FieldAttributeNames = CaptureNames(FieldAttributes);
        MethodAttributeNames = CaptureNames(MethodAttributes);

        // Registering actions...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(x => x != null)
            .Collect();

        // Registering source code emission...
        context.RegisterSourceOutput(items, Execute);

        /// <summary>
        /// Invoked to capture the type names of the attributes specified in this instance.
        /// We assume each type derives from the <see cref="Attribute"/> class.
        /// </summary>
        static string[] CaptureNames(Type[] types)
        {
            var attribute = "Attribute";
            var array = new string[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var name = type.Name;

                if (!name.Contains(attribute))
                {
                    var index = name.IndexOf('`');
                    if (index > 0)
                    {
                        var gens = name[index..];
                        name = name.Replace(gens, "");
                        name += attribute;
                        name += gens;
                    }
                    else
                    {
                        name += attribute;
                    }
                }

                array[i] = name;
            }

            return array;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation or not. By default, this method just compares if any
    /// of the attributes applied to the given node match with any of the specified ones, for the
    /// kind of that node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual bool Predicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node switch
        {
            TypeDeclarationSyntax item => Match(item, TypeAttributeNames),
            PropertyDeclarationSyntax item => Match(item, PropertyAttributeNames),
            FieldDeclarationSyntax item => Match(item, FieldAttributeNames),
            MethodDeclarationSyntax item => Match(item, MethodAttributeNames),

            _ => false
        };

        /// <summary>
        /// Determines if the given syntax node has at least one attribute that matches with any
        /// of the given ones, using a quick comparison among the names of those collection of
        /// attributes.
        /// </summary>
        static bool Match(MemberDeclarationSyntax syntax, string[] types)
        {
            var ats = syntax.AttributeLists.GetAttributes();
            var attribute = "Attribute";

            foreach (var at in ats)
            {
                var name = at.Name.ShortName();
                if (!name.EndsWith(attribute)) name += attribute;

                var arity = at.Name.Arity;
                if (arity > 0) name += $"`{arity}";

                foreach (var type in types) if (name == type) return true;
            }

            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform the syntax node carried by the given context into a valid candidate
    /// for source code generation purposes.
    /// <br/> Note that if this method returns a null instance, then it will simply be ignored
    /// and not be not taken into consideration for that purposes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token)
                ?? throw new ArgumentException(
                    "Cannot find symbol of the given type syntax.")
                    .WithData(typeSyntax);

            var atts = Matches(symbol.GetAttributes(), TypeAttributes).ToImmutableArray();
            if (atts.Length != 0)
                return new TypeCandidate(atts, model, typeSyntax, symbol);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token)
                ?? throw new ArgumentException(
                    "Cannot find symbol of the given property syntax.")
                    .WithData(propertySyntax);

            var atts = Matches(symbol.GetAttributes(), PropertyAttributes).ToImmutableArray();
            if (atts.Length != 0)
                return new PropertyCandidate(atts, model, propertySyntax, symbol);
        }

        // Fields...
        else if (syntax is FieldDeclarationSyntax fieldSyntax)
        {
            var found = false;
            var items = fieldSyntax.Declaration.Variables;

            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol != null)
                {
                    found = true;
                    var atts = Matches(symbol.GetAttributes(), FieldAttributes).ToImmutableArray();
                    if (atts.Length != 0)
                        return new FieldCandidate(atts, model, fieldSyntax, symbol);
                }
            }

            if (!found) throw new ArgumentException(
                "Cannot find symbol of the given field syntax.").WithData(fieldSyntax);
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token)
                ?? throw new ArgumentException(
                    "Cannot find symbol of the given method syntax.")
                    .WithData(methodSyntax);

            var atts = Matches(symbol.GetAttributes(), PropertyAttributes).ToImmutableArray();
            if (atts.Length != 0)
                return new MethodCandidate(atts, model, methodSyntax, symbol);
        }

        // Finishing with no transformation...
        throw null!;
    }

    /// <summary>
    /// Extracts the attributes that match any of the given types.
    /// </summary>
    static List<AttributeData> Matches(
        IEnumerable<AttributeData> attributes,
        Type[] types)
    {
        List<AttributeData> items = [];

        foreach (var attribute in attributes)
            foreach (var type in types)
                if (attribute.Match(type)) items.Add(attribute);

        return items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the given collection of captured candidates, by
    /// creating the appropriate hierarchy and then invoking the appropriate methods on the
    /// created nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        throw null;
    }
}