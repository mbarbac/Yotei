namespace Yotei.Tools.CoreGenerator;

// It seems that if we capture the 'Compilation' object then we'll loose the incremental nature of
// the generator, so that even the smallest change will cause a full generator execution. Therefore,
// we shall not use it here. What I don't understand is why the 'context.CompilationProvider' can
// be used if such can happen.

// ========================================================
/// <summary>
/// Represents the base class for tree-oriented incremental source code generators that arrange
/// their captured elements in a hierarchical tree structure where each top node corresponds to
/// a single type, along with its child elements (if any), and its emitted in its own file.
/// <para>
/// Derived types shall be be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is also expected that the <see cref="LanguageNames.CSharp"/>
/// value is used as that attribute's argument.
/// </para>
/// </summary>
internal class TreeGenerator
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
            .CreateSyntaxProvider(SyntaxPredicate, CaptureNode)
            .Where(static x => x is not null)
            .Collect();

        // Registering source code emit actions...
        // var combined = context.CompilationProvider.Combine(items);
        context.RegisterSourceOutput(/*combined*/ items, EmitNodes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance tries to launch a compile-time debug session when compiling.
    /// <br/> The default value of this property is <see langword="false"/>.
    /// </summary>
    protected virtual bool LaunchDebugger => false;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types
    /// shall be emitted in the namespace of the derived generator.
    /// <br/> The default value of this property is <see langword="true"/>.
    protected virtual bool EmitNullabilityHelpers => true;

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. Derived types shall invoke this base method.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        if (EmitNullabilityHelpers)
        {
            var ns = GetType().Namespace!;
            var str = $$"""
                using System;

                namespace {{ns}}
                {
                    /// <summary>
                    /// Used to wrap types for which nullability information shall be persisted.
                    /// <para>
                    /// Nullable annotations on value types are always translated by the compiler into instances of
                    /// the <see cref="Nullable{T}"/> struct. By contrast, nullable annotations on reference types are
                    /// just syntactic sugar used by the compiler but, in general, either they are not persisted in
                    /// metadata or in custom attributes, or they are not allowed in certain contexts (e.g., generic
                    /// type arguments).
                    /// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be used
                    /// as workarounds when there is the need to persist nullability information for their associated
                    /// types, or when there is the need to specify it in those not-allowed contexts.
                    /// </para>
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
            context.AddSource(ns + '.' + "IsNullable[T].g.cs", str);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if supported type-alike nodes shall be considered as potential candidates.
    /// <br/> The default value of this setting is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseTypeKind => false;

    /// <summary>
    /// Determines if supported property-alike nodes shall be considered as potential candidates.
    /// <br/> The default value of this setting is <see langword="false"/>.
    /// </summary>
    protected virtual bool UsePropertyKind => false;

    /// <summary>
    /// Determines if supported field-alike nodes shall be considered as potential candidates.
    /// <br/> The default value of this setting is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseFieldKind => false;

    /// <summary>
    /// Determines if supported method-alike nodes shall be considered as potential candidates.
    /// <br/> The default value of this setting is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseMethodKind => false;

    /// <summary>
    /// Determines if supported event-alike nodes shall be considered as potential candidates.
    /// <br/> The default value of this setting is <see langword="false"/>.
    /// </summary>
    protected virtual bool UseEventKind => false;

    /// <summary>
    /// Invoked to quickly determine if the given syntax node shall be consider as a potential
    /// candidate for source code generation, or not. By default this method filters out those
    /// elements whose syntax kind is not allowed.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return
            (UseTypeKind && node is EnumDeclarationSyntax) ||
            (UseTypeKind && node is TypeDeclarationSyntax) ||
            (UsePropertyKind && node is IndexerDeclarationSyntax) ||
            (UsePropertyKind && node is PropertyDeclarationSyntax) ||
            (UseFieldKind && node is FieldDeclarationSyntax) ||
            (UseMethodKind && node is ConstructorDeclarationSyntax) ||
            (UseMethodKind && node is DestructorDeclarationSyntax) ||
            (UseMethodKind && node is MethodDeclarationSyntax) ||
            (UseMethodKind && node is ConversionOperatorDeclarationSyntax) ||
            (UseMethodKind && node is OperatorDeclarationSyntax) ||
            (UseEventKind && node is EventDeclarationSyntax) ||
            (UseEventKind && node is EventFieldDeclarationSyntax);
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// types (enums and regular ones) as source code generation elements.
    /// </summary>
    protected virtual List<Type> TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// properties (indexed and regular ones) as source code generation elements.
    /// </summary>
    protected virtual List<Type> PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// fields as source code generation elements.
    /// </summary>
    protected virtual List<Type> FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// methods (constructors, destructors, conversions, operators and regular ones) as source code
    /// generation elements.
    /// </summary>
    protected virtual List<Type> MethodAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute types this generator uses, by default, to identify decorated
    /// events (field and regular ones) as source code generation elements.
    /// </summary>
    protected virtual List<Type> EventAttributes { get; } = [];

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
    /// Invoked to create a detached node for source code generation purposes.
    /// <br/> The symtax must be among the supported ones.
    /// </summary>
    protected virtual TypeNode CreateNode(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new TypeNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node for source code generation purposes.
    /// <br/> The symtax must be among the supported ones.
    /// </summary>
    protected virtual PropertyNode CreateNode(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new PropertyNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node for source code generation purposes.
    /// <br/> The symtax must be among the supported ones.
    /// </summary>
    protected virtual FieldNode CreateNode(
        IFieldSymbol symbol,
        FieldDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new FieldNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node for source code generation purposes.
    /// <br/> The symtax must be among the supported ones.
    /// </summary>
    protected virtual MethodNode CreateNode(
        IMethodSymbol symbol,
        BaseMethodDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new MethodNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// Invoked to create a detached node for source code generation purposes.
    /// <br/> The symtax must be among the supported ones.
    /// </summary>
    protected virtual EventNode CreateNode(
        IEventSymbol symbol,
        MemberDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new EventNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture a element for source code generation purposes. This method can also
    /// return <see langword="null"/> if the syntax node shall be ignored, or an error candidate
    /// to report error conditions at code generation time.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    protected virtual INode CaptureNode(
        GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        while (node is BaseTypeDeclarationSyntax syntax)
        {
            if (syntax is not EnumDeclarationSyntax and not TypeDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Properties...
        while (node is BasePropertyDeclarationSyntax syntax)
        {
            if (syntax is not IndexerDeclarationSyntax and not PropertyDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token) as IPropertySymbol;
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }
        
        // Fields...
        while (node is FieldDeclarationSyntax syntax)
        {
            var items = syntax.Declaration.Variables;
            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol; // item!
                if (symbol is null) break;

                var atx = FindSyntaxAttributes(symbol, syntax);
                var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
                if (ats.Count == 0) break;

                var candidate = CreateNode(symbol, syntax, ats, model);
                return candidate;
            }
            break;
        }

        // Methods...
        while (node is BaseMethodDeclarationSyntax syntax)
        {
            if (syntax is not MethodDeclarationSyntax
                and not ConstructorDeclarationSyntax and not DestructorDeclarationSyntax
                and not OperatorDeclarationSyntax and not ConversionOperatorDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token);
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Events...
        while (node is MemberDeclarationSyntax syntax)
        {
            if (syntax is not EventDeclarationSyntax and not EventFieldDeclarationSyntax)
                break;

            var symbol = model.GetDeclaredSymbol(syntax, token) as IEventSymbol;
            if (symbol is null) break;

            var atx = FindSyntaxAttributes(symbol, syntax);
            var ats = FilterAttributes(atx, TypeAttributes, TypeAttributeNames);
            if (ats.Count == 0) break;

            var candidate = CreateNode(symbol, syntax, ats, model);
            return candidate;
        }

        // Finishing by ignoring the node...
        return null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Filters out the given collection of attributes by matching them with either the specified
    /// types or with the specified names.
    /// </summary>
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
    /// Obtains the collection of attributes decorating the given syntax and transform them in the
    /// ones decorating the given symbol. For whatever reasons, <see cref="ISymbol.GetAttributes"/>
    /// does not return all attributes when the symbol is defined in different places (ie: partial
    /// types).
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
    /// Invoked to generate the source code of the given collection of nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nodes"></param>
    protected virtual void EmitNodes(
        SourceProductionContext context, ImmutableArray<INode> nodes)
    {
        List<TypeNode> types = [];

        // Capturing hierarchy and reporting errors...
        foreach (var node in nodes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            switch (node)
            {
                case ErrorNode item: item.Diagnostic.Report(context); break;
                case TypeNode item: CaptureHierarchy(types, item); break;
                case PropertyNode item: CaptureHierarchy(types, item); break;
                case FieldNode item: CaptureHierarchy(types, item); break;
                case MethodNode item: CaptureHierarchy(types, item); break;
                case EventNode item: CaptureHierarchy(types, item); break;
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
            var name = GetFileName(type.Symbol) + ".g.cs";
            context.AddSource(name, code);
        }
    }

    /// <summary>
    /// Invoked to obtain a suitable file name for the given type.
    /// </summary>
    static string GetFileName(INamedTypeSymbol symbol)
    {
        var options = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        var name = symbol.ThrowWhenNull().ToDisplayString(options);

        List<int> dots = [];
        int depth = 0;
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == '<') { depth++; continue; }
            if (name[i] == '>') { depth--; continue; }
            if (name[i] == '.' && depth == 0) dots.Add(i);
        }

        List<string> parts = [];
        int last = 0;

        foreach (var dot in dots)
        {
            parts.Add(name[last..dot]);
            last = dot + 1;
        }
        parts.Add(name[last..]);

        parts.Reverse();
        return string.Join(".", parts);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="node"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, TypeNode node)
    {
        var type = types.Find(x => Comparer.Equals(x.Symbol, node.Symbol));

        if (type is null) // Capturing the given node...
        {
            types.Add(node);
        }
        else
        {
            if (type.IsChildsOnly) // Substituting the existing one...
            {
                node.Augment(type);

                types.Remove(type);
                types.Add(node);
            }
            else // Augmenting the existing one...
            {
                type.Augment(node);
            }
        }
    }

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="node"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, PropertyNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { IsChildsOnly = true };
            types.Add(type);
        }

        var item = type.ChildProperties.Find(x => Comparer.Equals(x.Symbol, node.Symbol));
        
        if (item is null) // Adding a new child...
        {
            type.ChildProperties.Add(node);
            node.ParentNode = type;
        }
        else // Augmenting the existing one...
        {
            item.Augment(node);
        }
    }

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="node"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, FieldNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { IsChildsOnly = true };
            types.Add(type);
        }

        var item = type.ChildFields.Find(x => Comparer.Equals(x.Symbol, node.Symbol));

        if (item is null) // Adding a new child...
        {
            type.ChildFields.Add(node);
            node.ParentNode = type;
        }
        else // Augmenting the existing one...
        {
            item.Augment(node);
        }
    }

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="node"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, MethodNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { IsChildsOnly = true };
            types.Add(type);
        }

        var item = type.ChildMethods.Find(x => Comparer.Equals(x.Symbol, node.Symbol));

        if (item is null) // Adding a new child...
        {
            type.ChildMethods.Add(node);
            node.ParentNode = type;
        }
        else // Augmenting the existing one...
        {
            item.Augment(node);
        }
    }

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="node"></param>
    protected virtual void CaptureHierarchy(List<TypeNode> types, EventNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { IsChildsOnly = true };
            types.Add(type);
        }

        var item = type.ChildEvents.Find(x => Comparer.Equals(x.Symbol, node.Symbol));

        if (item is null) // Adding a new child...
        {
            type.ChildEvents.Add(node);
            node.ParentNode = type;
        }
        else // Augmenting the existing one...
        {
            item.Augment(node);
        }
    }
}