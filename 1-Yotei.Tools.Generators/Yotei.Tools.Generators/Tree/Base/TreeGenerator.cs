using System.CodeDom.Compiler;

namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents the base class for tree-oriented incremental source generators.
/// <br/> Derived classes must be decorated with the <see cref="GeneratorAttribute"/> attribute,
/// with the <see cref="LanguageNames.CSharp"/> argument, to be invoked by the compiler.
/// </summary>
internal class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if the generator tries to launch a debug session, or not.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Invoked after initialization to register constant post-initialization actions, such as
    /// generating additional code, or reading external files.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types used by the generator to identify type candidates.
    /// </summary>
    protected virtual Type[] TypeAttributes { get; } = [];
    string[] TypeAttributeNames = default!;

    /// <summary>
    /// The collection of attribute types used by the generator to identify property candidates.
    /// </summary>
    protected virtual Type[] PropertyAttributes { get; } = [];
    string[] PropertyAttributeNames = default!;

    /// <summary>
    /// The collection of attribute types used by the generator to identify field candidates.
    /// </summary>
    protected virtual Type[] FieldAttributes { get; } = [];
    string[] FieldAttributeNames = default!;

    /// <summary>
    /// The collection of attribute types used by the generator to identify method candidates.
    /// </summary>
    protected virtual Type[] MethodAttributes { get; } = [];
    string[] MethodAttributeNames = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to initialize this generator and register generation steps via callbacks.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Register post-initialization constant actions....
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering capturing and transformation actions...
        var comparer = new CandidateComparer();
        CaptureAttributeTypeNames();

        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(x => x != null)
            //.WithComparer(comparer)
            .Collect();

        // Registering source code emission...
        context.RegisterSourceOutput(items, Execute);
    }

    /// <summary>
    /// Captures the names of the types that are assumed to be the attributes applied to each
    /// kind of supported syntax nodes.
    /// </summary>
    void CaptureAttributeTypeNames()
    {
        TypeAttributeNames = Capture(TypeAttributes);
        PropertyAttributeNames = Capture(PropertyAttributes);
        FieldAttributeNames = Capture(FieldAttributes);
        MethodAttributeNames = Capture(MethodAttributes);

        // Returns the array of names corresponding to the given array of types.
        string[] Capture(Type[] types)
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
                    else name += attribute;
                }

                array[i] = name;
            }
            return array;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine if the given node shall be considered as a potential candidate for
    /// source code generation, or not. This method just tries to quickly compare the attributes
    /// applied to the node with any of the requested ones for that node kind.
    /// </summary>
    bool Predicate(SyntaxNode node, CancellationToken token)
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
        /// of the requested types, using a quick match between the names of the attributes on
        /// that node and the requested ones for its kind.
        /// </summary>
        static bool Match(MemberDeclarationSyntax syntax, string[] types)
        {
            var ats = syntax.AttributeLists.GetAttributes();
            var attribute = "Attribute";

            foreach (var at in ats)
            {
                var name = at.Name.ShortName(); if (!name.EndsWith(attribute)) name += attribute;
                var arity = at.Name.Arity; if (arity > 0) name += $"`{arity}";

                foreach (var type in types) if (name == type) return true;
            }

            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform the syntax node carried by the given context into a candidate for
    /// source code generation.
    /// </summary>
    ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token);
            if (symbol == null) throw null;

            var atts = Matches(symbol.GetAttributes(), TypeAttributes);
            if (atts.Length != 0) return new TypeCandidate(atts, model, typeSyntax, symbol);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol == null) throw null;

            var atts = Matches(symbol.GetAttributes(), PropertyAttributes);
            if (atts.Length != 0) return new PropertyCandidate(atts, model, propertySyntax, symbol);
        }

        // Fields...
        else if (syntax is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol == null) continue;

                var atts = Matches(symbol.GetAttributes(), FieldAttributes);
                if (atts.Length != 0) return new FieldCandidate(atts, model, fieldSyntax, symbol);
            }
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol == null) throw null;

            var atts = Matches(symbol.GetAttributes(), MethodAttributes);
            if (atts.Length != 0) return new MethodCandidate(atts, model, methodSyntax, symbol);
        }

        return null!;
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

        return items.ToImmutableArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for the given captured nodes by creating the appropriate
    /// hierarchy and then invoking the nodes created out of the candidates.
    /// </summary>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
    }
}