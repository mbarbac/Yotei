#pragma warning disable IDE0019

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

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(
        TypeNode parent, PropertyCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(
        TypeNode parent, FieldCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(
        TypeNode parent, MethodCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Gets the name of the file where the given candidate will emit its source code.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual string GetFileName(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nschain,
        ImmutableArray<INamedTypeSymbol> tpchain)
        => GetFileNameByTailType(nschain, tpchain);

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
        static string[] Capture(Type[] types)
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
            if (symbol == null)
                return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(typeSyntax));

            var atts = Matches(symbol.GetAttributes(), TypeAttributes);
            if (atts.Length != 0)
                return new TypeCandidate(atts, model, typeSyntax, symbol);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol == null)
                return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(propertySyntax));

            var atts = Matches(symbol.GetAttributes(), PropertyAttributes);
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
                    var atts = Matches(symbol.GetAttributes(), FieldAttributes);
                    if (atts.Length != 0)
                        return new FieldCandidate(atts, model, fieldSyntax, symbol);
                }
            }
            if (!found) return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(fieldSyntax));
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol == null)
                return new ErrorCandidate(TreeDiagnostics.SymbolNotFound(methodSyntax));

            var atts = Matches(symbol.GetAttributes(), MethodAttributes);
            if (atts.Length != 0)
                return new MethodCandidate(atts, model, methodSyntax, symbol);
        }

        // Represents a valid kind with no attribute matches...
        return null!;
    }

    /// <summary>
    /// Extracts the attributes that match any of the given types.
    /// </summary>
    static ImmutableArray<AttributeData> Matches(
        IEnumerable<AttributeData> attributes, Type[] types)
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
        var files = new ChildFiles();
        var nschain = ImmutableArray<BaseNamespaceDeclarationSyntax>.Empty;
        var tpchain = ImmutableArray<INamedTypeSymbol>.Empty;
        var comparer = SymbolComparer.Default;
        INode parent = default!;

        // Reporting errors...
        candidates.OfType<IErrorCandidate>().ForEach(x => context.ReportDiagnostic(x.Diagnostic));

        // Creatinh hierarchy...
        candidates.OfType<TypeCandidate>().ForEach(CaptureHierarchy);
        candidates.OfType<INodeCandidate>().ForEach(x => x is not TypeCandidate, CaptureHierarchy);

        // Emitting source code...
        foreach (var file in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!file.Validate(context)) continue;

            var cb = new CodeBuilder(); file.Emit(context, cb);
            var code = cb.ToString();
            var name = file.FileName + ".g.cs";
            context.AddSource(name, code);
        }

        /// <summary>
        /// Invoked to emit the hierarchy for the given candidate.
        /// </summary>
        void CaptureHierarchy(INodeCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            nschain = candidate.Syntax.GetNamespaceSyntaxChain();
            tpchain = candidate.Symbol.GetTypeSymbolChain();

            if (!CaptureFileLevel(candidate)) return;
            if (!CaptureNamespaceLevel(candidate)) return;
            if (!CaptureTypeLevel(candidate)) return;
            if (!CapturePropertyLevel(candidate)) return;
            if (!EmitFieldLevel(candidate)) return;
            if (!CaptureMethodLevel(candidate)) return;
        }

        /// <summary>
        /// Invoked to create the hierarchy at file level.
        /// <br/> Returns false if any error happened that prevent further execution.
        /// </summary>
        bool CaptureFileLevel(INodeCandidate candidate)
        {
            var name = GetFileName(nschain, tpchain);
            var node = files.Find(x => string.Compare(x.FileName, name, ignoreCase: true) == 0);

            if (node == null)
            {
                node = new FileNode(name);
                files.Add(node);
            }

            parent = node;
            return true;
        }

        /// <summary>
        /// Invoked to create the hierarchy at namespace level.
        /// <br/> Returns false if any error happened that prevent further execution.
        /// </summary>
        bool CaptureNamespaceLevel(INodeCandidate candidate)
        {
            var list = ((FileNode)parent).ChildNamespaces;
            var len = nschain.Length;

            for (int index = 0; index < len; index++)
            {
                var syntax = nschain[index];
                var name = syntax.Name.ToString();
                var node = list.Find(x => string.Compare(x.Name, name) == 0);

                if (node == null)
                {
                    node = new(parent, syntax);
                    list.Add(node);
                }

                parent = node;
                list = node.ChildNamespaces;
            }

            return true;
        }

        /// <summary>
        /// Invoked to create the hierarchy at type level.
        /// <br/> Returns false if any error happened that prevent further execution.
        /// </summary>
        bool CaptureTypeLevel(INodeCandidate candidate)
        {
            var list = ((NamespaceNode)parent).ChildTypes;
            var len = tpchain.Length;

            for (int index = 0; index < len; index++)
            {
                var symbol = tpchain[index];

                // Find using symbol comparer, as may have same name but different type arguments...
                var node = list.Find(x => comparer.Equals(x.Symbol, symbol));

                // Need to create a new node...
                if (node == null)
                {
                    // Creatig custom node...
                    if (candidate is TypeCandidate element && index == (len - 1))
                    {
                        node = CreateNode(parent, element);
                        list.Add(node);
                    }

                    // Creating structural...
                    else
                    {
                        node = new(parent, symbol);
                        list.Add(node);
                    }
                }

                // Validating consistency...
                else if (candidate is TypeCandidate element && index == (len - 1))
                {
                    if (node.GetType() == typeof(TypeNode)) // Substitute structural...
                    {
                        var temp = CreateNode(parent, element);
                        foreach (var child in node.ChildTypes) temp.ChildTypes.Add(child);
                        foreach (var child in node.ChildProperties) temp.ChildProperties.Add(child);
                        foreach (var child in node.ChildFields) temp.ChildFields.Add(child);
                        foreach (var child in node.ChildMethods) temp.ChildMethods.Add(child);

                        list.Remove(node);
                        list.Add(temp);
                    }
                    else // Validating existing custom node...
                    {
                        if (!comparer.Equals(node.Symbol, symbol))
                        {
                            context.ReportDiagnostic(
                                TreeDiagnostics.InconsistentHierarchy(node, candidate));

                            return false;
                        }
                    }
                }

                parent = node;
                list = node.ChildTypes;
            }

            return true;
        }

        /// <summary>
        /// Invoked to create the hierarchy at property level.
        /// <br/> Returns false if any error happened that prevent further execution.
        /// </summary>
        bool CapturePropertyLevel(INodeCandidate candidate)
        {
            if (candidate is PropertyCandidate item)
            {
                var tpnode = (TypeNode)parent;
                var list = tpnode.ChildProperties;
                var symbol = item.Symbol;
                var node = list.Find(x => x.Symbol.Name == symbol.Name);

                // Creating a new custom node...
                if (node == null)
                {
                    node = CreateNode(tpnode, item);
                    list.Add(node);
                }

                // Validating consistency...
                else
                {
                    if (node.GetType() == typeof(PropertyNode)) // Substitute structural...
                    {
                        var temp = CreateNode(tpnode, item);
                        list.Remove(node);
                        list.Add(temp);
                    }
                    else // Validating existing custom node...
                    {
                        if (!comparer.Equals(node.Symbol, symbol))
                        {
                            context.ReportDiagnostic(
                                TreeDiagnostics.InconsistentHierarchy(node, candidate));

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Invoked to create the hierarchy at field level.
        /// <br/> Returns false if any error happened that prevent further execution.
        /// </summary>
        bool EmitFieldLevel(INodeCandidate candidate)
        {
            if (candidate is FieldCandidate item)
            {
                var tpnode = (TypeNode)parent;
                var list = tpnode.ChildFields;
                var symbol = item.Symbol;
                var node = list.Find(x => x.Symbol.Name == symbol.Name);

                // Creating a new custom node...
                if (node == null)
                {
                    node = CreateNode(tpnode, item);
                    list.Add(node);
                }

                // Validating consistency...
                else
                {
                    if (node.GetType() == typeof(FieldNode)) // Substitute structural...
                    {
                        var temp = CreateNode(tpnode, item);
                        list.Remove(node);
                        list.Add(temp);
                    }
                    else // Validating existing custom node...
                    {
                        if (!comparer.Equals(node.Symbol, symbol))
                        {
                            context.ReportDiagnostic(
                                TreeDiagnostics.InconsistentHierarchy(node, candidate));

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Invoked to create the hierarchy at method level.
        /// <br/> Returns false if any error happened that prevent further execution.
        /// </summary>
        bool CaptureMethodLevel(INodeCandidate candidate)
        {
            if (candidate is MethodCandidate item)
            {
                var tpnode = (TypeNode)parent;
                var list = tpnode.ChildMethods;
                var symbol = item.Symbol;
                var node = list.Find(x => x.Symbol.Name == symbol.Name);

                // Creating a new custom node...
                if (node == null)
                {
                    node = CreateNode(tpnode, item);
                    list.Add(node);
                }

                // Validating consistency...
                else
                {
                    if (node.GetType() == typeof(MethodNode)) // Substitute structural...
                    {
                        var temp = CreateNode(tpnode, item);
                        list.Remove(node);
                        list.Add(temp);
                    }
                    else // Validating existing custom node...
                    {
                        if (!comparer.Equals(node.Symbol, symbol))
                        {
                            context.ReportDiagnostic(
                                TreeDiagnostics.InconsistentHierarchy(node, candidate));

                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a suitable file name, without extensions, based on the tail-most namespace.
    /// </summary>
    /// <param name="nschain"></param>
    /// <param name="tpchain"></param>
    /// <returns></returns>
    protected virtual string GetFileNameByTailNamespace(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nschain)
    {
        nschain.ThrowWhenNull();

        List<string> parts = [];

        foreach (var ns in nschain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }

    /// <summary>
    /// Returns a suitable file name, without extensions, based on the tail-most type.
    /// </summary>
    /// <param name="nschain"></param>
    /// <param name="tpchain"></param>
    /// <returns></returns>
    protected virtual string GetFileNameByTailType(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nschain,
        ImmutableArray<INamedTypeSymbol> tpchain)
    {
        nschain.ThrowWhenNull();
        tpchain.ThrowWhenNull();

        List<string> parts = [];

        foreach (var ns in nschain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        foreach (var tp in tpchain)
        {
            var name = tp.Name;

            if (name.Length == 0) name = "$";
            else
            {
                var index = name.IndexOf('`');
                if (index > 0) name = name[..index];
            }
            parts.Add(name);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }
}