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

    /// <summary>
    /// Invoked at initialization time to register register post-initialization actions, such as
    /// generating additional code for marker attributes, reading external files, etc. Inheritors
    /// must invoke first this base implementation.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();
        context.AddSource("IsNullable[T].g.cs", IsNullableTypeCode);
    }

    // Source code for the 'IsNullable<T>' type...
    private readonly static string IsNullableTypeCode = """
        namespace Yotei.Tools.CoreGenerator;
        /// <summary>
        /// Used to specify that the wrapped type shall be treated as a nullable one when either
        /// the compiler prevents nullable annotations, or when these annotations are not persisted
        /// in metadata (for instance, when used with reference types).
        /// </summary>
        [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
        public partial class IsNullable<T> { }
        """;

    /// <summary>
    /// <inheritdoc/>. This method is infrastructure only, not for public usage.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (TypeAttributes is null) throw new ArgumentNullException(nameof(TypeAttributes));
        if (PropertyAttributes is null) throw new ArgumentNullException(nameof(PropertyAttributes));
        if (FieldAttributes is null) throw new ArgumentNullException(nameof(FieldAttributes));
        if (MethodAttributes is null) throw new ArgumentNullException(nameof(MethodAttributes));

        if (TypeAttributesNames is null) throw new ArgumentNullException(nameof(TypeAttributesNames));
        if (PropertyAttributesNames is null) throw new ArgumentNullException(nameof(PropertyAttributesNames));
        if (FieldAttributesNames is null) throw new ArgumentNullException(nameof(FieldAttributesNames));
        if (MethodAttributesNames is null) throw new ArgumentNullException(nameof(MethodAttributesNames));

        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Registering post-initialization actions....
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering filtering and transforming actions...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(static x => x != null)
            .Collect();

        // Registering source code emit actions...
        context.RegisterSourceOutput(items, Execute);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types used by this generator to identify type candidates.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by this generator to identify property candidates.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by this generator to identify field candidates.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types used by this generator to identify method candidates.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute full qualified type names used by this generator to identify
    /// type candidates.
    /// </summary>
    protected virtual List<string> TypeAttributesNames { get; } = [];

    /// <summary>
    /// The collection of attribute full qualified type names used by this generator to identify
    /// property candidates.
    /// </summary>
    protected virtual List<string> PropertyAttributesNames { get; } = [];

    /// <summary>
    /// The collection of attribute full qualified type names used by this generator to identify
    /// field candidates.
    /// </summary>
    protected virtual List<string> FieldAttributesNames { get; } = [];

    /// <summary>
    /// The collection of attribute full qualified type names used by this generator to identify
    /// method candidates.
    /// </summary>
    protected virtual List<string> MethodAttributesNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be considered as a potential
    /// candidate for source code generation, or not.
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
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => throw null;

    /// <summary>
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual TypeCandidate CreateCandidate(
        IPropertySymbol symbol,
        PropertyDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => throw null;

    /// <summary>
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual TypeCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => throw null;

    /// <summary>
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual TypeCandidate CreateCandidate(
        IMethodSymbol symbol,
        MethodDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
        => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform the syntax node carried by the given context into a valid source code
    /// generation candidate. This method may also  return '<c>null</c>' if the syntax node shall
    /// be ignored, or an error candidate to report an error diagnostic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        while (syntax is TypeDeclarationSyntax typesyntax)
        {
            var symbol = model.GetDeclaredSymbol(typesyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, typesyntax, TypeAttributes, TypeAttributesNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, typesyntax, ats, model);
            return candidate;
        }

        // Properties...
        while (syntax is PropertyDeclarationSyntax propertysyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertysyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, propertysyntax, PropertyAttributes, PropertyAttributesNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, propertysyntax, ats, model);
            return candidate;
        }

        // Properties...
        while (syntax is PropertyDeclarationSyntax propertysyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertysyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, propertysyntax, PropertyAttributes, PropertyAttributesNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, propertysyntax, ats, model);
            return candidate;
        }

        // Properties...
        while (syntax is FieldDeclarationSyntax fieldsyntax)
        {
            var items = fieldsyntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol is null) break;

                var ats = FindAttributes(symbol, fieldsyntax, FieldAttributes, FieldAttributesNames);
                if (ats.Count == 0) break;

                var candidate = CreateCandidate(symbol, fieldsyntax, ats, model);
                return candidate;
            }
            break;
        }

        // Properties...
        while (syntax is MethodDeclarationSyntax methodsyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodsyntax, token);
            if (symbol is null) break;

            var ats = FindAttributes(symbol, methodsyntax, MethodAttributes, MethodAttributesNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, methodsyntax, ats, model);
            return candidate;
        }

        // Finishing by ignoring the node...
        return null!;
    }

    /// <summary>
    /// Returns the collection of attributes that decorate the given element, either from the
    /// given collection of types, or from the given collection of full qualified type names.
    /// </summary>
    static List<AttributeData> FindAttributes(
        ISymbol symbol,
        MemberDeclarationSyntax syntax,
        List<Type> types, List<string> names)
    {
        var ats = symbol.GetAttributes(syntax);

        // By matching the decorating attributes with the given types...
        var list = ats.Where(x =>
            x.AttributeClass is not null &&
            x.AttributeClass.MatchAny(types)).ToList();

        // By matching the decorating attributes with the given names...
        foreach (var name in names)
        {
            var temps = ats.Where(x => x.AttributeClass?.Name == name);
            foreach (var temp in temps) if (!list.Contains(temp)) list.Add(temp);
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new file to hold the given type node.
    /// </summary>
    protected virtual FileNode CreateFileNode(TypeNode node) => throw null;

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual TypeNode CreateNode(TypeCandidate candidate) => throw null;

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candiate) => throw null;

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candiate) => throw null;

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candiate) => throw null;

    /// <summary>
    /// Invoked to emit the source code for the captured candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    void Execute(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        throw null;
    }
}