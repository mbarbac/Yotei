namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides core incremental source code generation capabilities. By default, instances of this
/// type firstly identify their relevant elements when they are decorated by any of the specified
/// attribute types (or attribute names), and then arrange them in a type-based hierarchy that is
/// used to emit one file per type. All these defaults can be modified and overriden as needed.
/// <para>
/// Derived types shall be be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is also expected that the <see cref="LanguageNames.CSharp"/>
/// value is used as the attribute argument.
/// </para>
/// </summary>
internal class CoreGenerator : IIncrementalGenerator
{
    static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Registering post-initialization actions....
        context.RegisterPostInitializationOutput(OnInitialize);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(SyntaxKindDiscriminator, CaptureCandidate)
            .Where(static x => x is not null)
            .Collect();

        // It seems that if we capture the compilation then we loose 'incremental' caching, so that
        // even the smallest change will execute the generator from scratch again.
        // var combined = context.CompilationProvider.Combine(items);

        // Registering source code emit actions...
        context.RegisterSourceOutput(/*combined*/ items, EmitCode);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types
    /// shall be emitted in the namespace of the derived generator.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool EmitTypeNullabilityHelpers => true;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. Derived types shall invoke this base method.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        if (EmitTypeNullabilityHelpers)
        {
            var ns = GetType().Namespace!;
            var str = $$"""
                using System;

                namespace {{ns}}
                {
                    /// <summary>
                    /// <inheritdoc cref="Yotei.Tools.CoreGenerator.IsNullable{T}"/>
                    /// </summary>
                    public class IsNullable<T> { }
                    
                    /// <summary>
                    /// <inheritdoc cref="IsNullable{T}"/>
                    /// </summary>
                    [AttributeUsage(AttributeTargets.All)]
                    public class IsNullableAttribute : Attribute { }
                }
                """;

            context.AddEmbeddedAttributeDefinition();
            context.AddSource("IsNullable[T].g.cs", str);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if type-alike syntax nodes shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseTypeKind => false;

    /// <summary>
    /// Determines if property-alike syntax nodes shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool UsePropertyKind => false;

    /// <summary>
    /// Determines if field-alike syntax nodes shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseFieldKind => false;

    /// <summary>
    /// Determines if method-alike syntax nodes shall be considered as potential candidates.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseMethodKind => false;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be consider as a potential
    /// candidate for source code generation, or not.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool SyntaxKindDiscriminator(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return
            (node is TypeDeclarationSyntax && UseTypeKind) ||
            (node is PropertyDeclarationSyntax && UsePropertyKind) ||
            (node is FieldDeclarationSyntax && UseFieldKind) ||
            (node is MethodDeclarationSyntax && UseMethodKind);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// types as source code generation candidates.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// properties as source code generation candidates.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// fields as source code generation candidates.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// methods as source code generation candidates.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated types as source code generation candidates.
    /// </summary>
    protected virtual List<string> TypeAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated properties as source code generation candidates.
    /// </summary>
    protected virtual List<string> PropertyAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated fields as source code generation candidates.
    /// </summary>
    protected virtual List<string> FieldAttributeNames { get; } = [];

    /// <summary>
    /// The collection of fully qualified attribute type names used by this generator, by default,
    /// to identify decorated methods as source code generation candidates.
    /// </summary>
    protected virtual List<string> MethodAttributeNames { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual TypeCandidate CreateCandidate(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new TypeCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual PropertyCandidate CreateCandidate(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new PropertyCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual FieldCandidate CreateCandidate(
        IFieldSymbol symbol,
        BaseFieldDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new FieldCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a source code generation candidate of the appropriate kind.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual MethodCandidate CreateCandidate(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new MethodCandidate(symbol) { Syntax = syntax };
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture a candidate for source code generation using the information from the
    /// given context. This method can also return <see langword="null"/> if the syntax node shall
    /// be ignored, or an error candidate to later report error conditions.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual ICandidate CaptureCandidate(
        GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        while (node is BaseTypeDeclarationSyntax syntax)
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, syntax, ats, model);
            return candidate;
        }

        // Properties...
        while (node is BasePropertyDeclarationSyntax syntax)
        {
            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, syntax, ats, model);
            return candidate;
        }

        // Fields...
        while (node is BaseFieldDeclarationSyntax syntax)
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol; // item!
                if (symbol is null) break;

                var atx = FindSyntaxAttributes(symbol, syntax);
                var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
                if (ats.Count == 0) break;

                var candidate = CreateCandidate(symbol, syntax, ats, model);
                return candidate;
            }
            break;
        }

        // Methods...
        while (node is BaseMethodDeclarationSyntax syntax)
        {
            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateCandidate(symbol, syntax, ats, model);
            return candidate;
        }

        // Finishing by ignoring the node...
        return null!;
    }

    /// <summary>
    /// Filters out the given collection of attributes by matching them with either the specified
    /// types or with the specified names.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="types"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    static List<AttributeData> FilterAttributes(
        IEnumerable<AttributeData> attributes,
        List<Type> types,
        List<string> names)
    {
        // By matching the found attributes against the given types...
        var list = attributes.Where(x =>
            x.AttributeClass != null &&
            x.AttributeClass.MatchAny(types)).ToList();

        // By matching the found attributes against the given names...
        foreach (var name in names)
        {
            var temps = attributes.Where(x => x.AttributeClass?.Name == name);
            foreach (var temp in temps)
                if (!list.Contains(temp)) list.Add(temp);
        }

        return list;
    }

    /// <summary>
    /// Obtains the collection of attributes that decorates the symbol through the ones found at
    /// the given syntax node. For whatever reasons, <see cref="ISymbol.GetAttributes"/> does not
    /// return all attributes when the symbol is defined in different places (ie: partial types).
    /// </summary>
    static IEnumerable<AttributeData> FindSyntaxAttributes(
        ISymbol symbol,
        MemberDeclarationSyntax syntax)
    {
        var atsyntaxes = syntax.AttributeLists.SelectMany(static x => x.Attributes);
        foreach (var atsyntax in atsyntaxes)
        {
            var atd = symbol.GetAttributes().FirstOrDefault(
                x => x.ApplicationSyntaxReference?.GetSyntax() == atsyntax);

            if (atd is not null) yield return atd;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(TypeCandidate candidate)
    {
        var item = new TypeNode(candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// <br/> This method DOES NOT add the newly created node to any parent collection.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(TypeNode parent, PropertyCandidate candidate)
    {
        var item = new PropertyNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// <br/> This method DOES NOT add the newly created node to any parent collection.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(TypeNode parent, FieldCandidate candidate)
    {
        var item = new FieldNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a new hierarchy node.
    /// <br/> This method DOES NOT add the newly created node to any parent collection.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(TypeNode parent, MethodCandidate candidate)
    {
        var item = new MethodNode(parent, candidate.Symbol);
        if (candidate.Syntax != null) item.SyntaxNodes.Add(candidate.Syntax);
        item.Attributes.AddRange(candidate.Attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate the source code of the given collection of candidates. This method,
    /// by default, arranges this collection in a type-based hierarchy that is used to emit one
    /// file per each type and its child elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    protected virtual void EmitCode(SourceProductionContext context, ImmutableArray<ICandidate> candidates)
    {
        List<TypeNode> types = [];

        // Processing candidates...
        foreach (var candidate in candidates)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            switch (candidate)
            {
                case ErrorCandidate item: item.Diagnostic.Report(context); break;
                case TypeCandidate item: CaptureHierarchy(types, item); break;
                case PropertyCandidate item: CaptureHierarchy(types, item); break;
                case FieldCandidate item: CaptureHierarchy(types, item); break;
                case MethodCandidate item: CaptureHierarchy(types, item); break;
            }
        }

        // Generating source code...
        foreach (var type in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!type.Validate(context)) continue;
            var cb = new CodeBuilder();
            type.Emit(context, cb);

            var code = cb.ToString();
            var name = type.GetFileName() + ".g.cs";
            context.AddSource(name, code);
        }
    }

    /// <summary>
    /// Invoked to capture in the hierarchy a node for the given candidate.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="candidate"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, TypeCandidate candidate)
    {
        var node = types.Find(x => Comparer.Equals(x.Symbol, candidate.Symbol));

        // We can create a brand new node...
        if (node is null)
        {
            node = CreateNode(candidate);
            types.Add(node);
        }
        else // We have to substitute the holder-only node, or augment the existing one...
        {
            if (node.IsHolderOnly)
            {
                var type = CreateNode(candidate);
                type.Augment(node);

                types.Remove(node);
                types.Add(type);
            }
            else node.Augment(candidate);
        }
    }

    /// <summary>
    /// Invoked to capture in the hierarchy a node for the given candidate.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="candidate"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, PropertyCandidate candidate)
    {
        var host = candidate.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));
        if (type is null)
        {
            type = new TypeNode(host) { IsHolderOnly = true };
            types.Add(type);
        }

        var node = type.ChildProperties.Find(x => Comparer.Equals(x.Symbol, candidate.Symbol));
        if (node is null)
        {
            node = CreateNode(type, candidate);
            type.ChildProperties.Add(node);
        }
        else node.Augment(candidate);
    }

    /// <summary>
    /// Invoked to capture in the hierarchy a node for the given candidate.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="candidate"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, FieldCandidate candidate)
    {
        var host = candidate.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));
        if (type is null)
        {
            type = new TypeNode(host) { IsHolderOnly = true };
            types.Add(type);
        }

        var node = type.ChildFields.Find(x => Comparer.Equals(x.Symbol, candidate.Symbol));
        if (node is null)
        {
            node = CreateNode(type, candidate);
            type.ChildFields.Add(node);
        }
        else node.Augment(candidate);
    }

    /// <summary>
    /// Invoked to capture in the hierarchy a node for the given candidate.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="candidate"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, MethodCandidate candidate)
    {
        var host = candidate.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));
        if (type is null)
        {
            type = new TypeNode(host) { IsHolderOnly = true };
            types.Add(type);
        }

        var node = type.ChildMethods.Find(x => Comparer.Equals(x.Symbol, candidate.Symbol));
        if (node is null)
        {
            node = CreateNode(type, candidate);
            type.ChildMethods.Add(node);
        }
        else node.Augment(candidate);
    }
}