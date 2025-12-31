#pragma warning disable IDE0019

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

#if EMIT_ISNULLABLE_TYPE
    /// <summary>
    /// Invoked at initialization time to register register post-initialization actions, such as
    /// generating additional code for marker attributes, reading external files, etc. Inheritors
    /// must call this base method to emit the 'IsNullable{T}' class.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();
        context.AddSource("IsNullable[T].g.cs", IsNullableTypeCode);
    }
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
#else
    /// <summary>
    /// Invoked at initialization time to register register post-initialization actions, such as
    /// generating additional code for marker attributes, reading external files, etc.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }
#endif

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

        var combined = context.CompilationProvider.Combine(items);

        // Registering source code emit actions...
        context.RegisterSourceOutput(combined, Execute);
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
            or BasePropertyDeclarationSyntax
            or FieldDeclarationSyntax
            or BaseMethodDeclarationSyntax;
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
    {
        var item = new TypeCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new PropertyCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        FieldDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new FieldCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate. Inheritors can choose how much data
    /// to cache in the returned object.
    /// </summary>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new MethodCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

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
        while (syntax is BasePropertyDeclarationSyntax propertysyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertysyntax, token) as IPropertySymbol;
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
        while (syntax is BaseMethodDeclarationSyntax methodsyntax)
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
    protected virtual FileNode CreateFileNode(
        Compilation compilation, TypeNode node) => new(compilation, node);

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual TypeNode CreateNode(TypeCandidate candidate)
    {
        var item = new TypeNode(candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
    {
        var item = new PropertyNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
    {
        var item = new FieldNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node based upon the given candidate.
    /// </summary>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candidate)
    {
        var item = new MethodNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for the captured candidates.
    /// </summary>
    void Execute(SourceProductionContext context, (Compilation, ImmutableArray<ICandidate>) source)
    {
        var compilation = source.Item1;
        var candidates = source.Item2;
        var comparer = SymbolEqualityComparer.Default;
        List<FileNode> files = [];

        // Error candidates...
        candidates.ForEach(x =>
            x is ErrorCandidate,
            x => ((ErrorCandidate)x).Diagnostic.Report(context));

        // Creating hierarchy...
        candidates.ForEach(
            x => x is IValidCandidate,
            x => Capture((IValidCandidate)x));

        // Generating source code...
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
        /// Invoked to capture the hierarchy of the given candidate...
        /// </summary>
        void Capture(IValidCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            // Capturing file-alike element...
            var tpcandidate = candidate as TypeCandidate;
            var tpsymbol = candidate.Symbol is INamedTypeSymbol named
                ? named
                : candidate.Symbol.ContainingType;

            var file = files.Find(x => comparer.Equals(tpsymbol, x.Node.Symbol));
            if (file == null)
            {
                // Creating a type node, either identified or auto-generated...
                var node = tpcandidate is null
                    ? new TypeNode(tpsymbol) { IsAutoGenerated = true }
                    : CreateNode(tpcandidate);

                // Creating a file holder for that type node...
                file = CreateFileNode(compilation, node);
                files.Add(file);
            }
            else if (tpcandidate is not null) // Collision detected...
            {
                // Existing node was auto-generated...
                if (file.Node.IsAutoGenerated)
                {
                    var node = CreateNode(tpcandidate);
                    node.Augment(file.Node);

                    files.Remove(file);
                    files.Add(file = CreateFileNode(compilation, node));
                }

                // Existing already was a valid one...
                else
                {
                    file.Node.Augment(tpcandidate);
                }
            }

            // Capturing property-alike element...
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

            // Capturing field-alike element...
            if (candidate is FieldCandidate fieldCandidate)
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

            // Capturing method-alike element...
            if (candidate is MethodCandidate methodCandidate)
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