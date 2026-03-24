namespace Yotei.Tools.CoreGenerator;

// ========================================================
partial class TreeGenerator
{
    static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Although this method is public, it is INFRASTRUCTURE ONLY and shall not be called
    /// by application code.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Launching a compile-time debug session if needed...
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        // Registering post-initialization actions....
        context.RegisterPostInitializationOutput(OnInitializeCore);

        // Registering pipeline steps...
        var items = context.SyntaxProvider
            .CreateSyntaxProvider(FastPredicate, CreateNode)
            .Where(static x => x is not null)
            .Collect();

        // Note: it seems that if we capture the 'Compilation' object we'll loose the incremental
        // nature if the generator, so that the smallest change will trigger a full source code
        // generation. Therefore, we won't do that. What I cannot understand is why then it would
        // be possible...

        // Registering source code emit actions...
        // var combined = context.CompilationProvider.Combine(items);
        context.RegisterSourceOutput(/*combined*/ items, EmitNodes);
    }

    /// <summary>
    /// Invoked to register post-initialization actions.
    /// </summary>
    /// <param name="context"></param>
    void OnInitializeCore(IncrementalGeneratorPostInitializationContext context)
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
                [Microsoft.CodeAnalysis.Embedded]
                public class IsNullable<T> { }
                
                /// <summary>
                /// <inheritdoc cref="IsNullable{T}"/>
                /// </summary>
                [Microsoft.CodeAnalysis.Embedded]
                [AttributeUsage(AttributeTargets.All)]
                public class IsNullableAttribute : Attribute { }
            }
            """;

            context.AddEmbeddedAttributeDefinition();

            var name = "Yotei.Tools.cs"; // ns + '.' + "IsNullable[T].g.cs";
            context.AddSource(name, str);
        }

        OnInitialize(context); // Invoking now what the inheritor may want to do...
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
            x.AttributeClass.MatchAny([.. types])).ToList();

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
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    void OnCaptureHierarchy(List<TypeNode> types, TypeNode node)
    {
        var type = types.Find(x => Comparer.Equals(x.Symbol, node.Symbol));

        if (type is null) // Capturing the given node...
        {
            types.Add(node);
        }
        else
        {
            if (type.ChildsOnly) // Substituting the existing one...
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    void OnCaptureHierarchy(List<TypeNode> types, PropertyNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { ChildsOnly = true };
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    void OnCaptureHierarchy(List<TypeNode> types, FieldNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { ChildsOnly = true };
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    void OnCaptureHierarchy(List<TypeNode> types, MethodNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { ChildsOnly = true };
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the given node into the source code generation hierarchy.
    /// </summary>
    void OnCaptureHierarchy(List<TypeNode> types, EventNode node)
    {
        var host = node.Symbol.ContainingType;
        var type = types.Find(x => Comparer.Equals(x.Symbol, host));

        if (type is null) // Creating a childs-only instance...
        {
            type = new TypeNode(host) { ChildsOnly = true };
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain a suitable file name for the given type.
    /// </summary>
    string GetFileName(INamedTypeSymbol symbol)
    {
        // First, we will dot-separate but not inside '<...>' portions...
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

        // While capturing the parts, we substitute '<>' by '[]' (the <> are not valid file chars)...
        List<string> parts = [];
        int last = 0;
        foreach (var dot in dots)
        {
            parts.Add(name[last..dot].Replace('<', '[').Replace('>', ']'));
            last = dot + 1;
        }
        parts.Add(name[last..].Replace('<', '[').Replace('>', ']'));

        // Organizing in a flat folder...
        if (FlatFileNames)
        {
            // We just need to reverse and return...
            parts.Reverse();
            var str = string.Join(".", parts);
            return str;
        }

        // Organizing in a structure using all but the last part as a folder...
        else
        {
            if (parts.Count == 1) return parts[0];
            else
            {
                var fname = parts[^1]; parts.RemoveAt(parts.Count - 1);
                parts.Reverse();
                var nspart = string.Join(".", parts);
                var str = string.Join("/", nspart, fname);
                return str;
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate the source code of the given collection of nodes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="nodes"></param>
    void EmitNodes(SourceProductionContext context, ImmutableArray<INode> nodes)
    {
        List<TypeNode> hierarchy = [];

        // Capturing and reporting errors...
        foreach (var node in nodes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (node is ErrorNode error) error.Diagnostic.Report(context);
            else CaptureNode(context, hierarchy, node);
        }

        // Generating source code...
        foreach (var type in hierarchy)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var cb = new CodeBuilder();
            var done = type.Emit(context, cb);
            if (done)
            {
                var code = cb.ToString();
                var name = GetFileName(type.Symbol) + ".g.cs";
                context.AddSource(name, code);
            }
        }
    }
}