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
    /// Invoked to create an appropriate node using the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create an appropriate node using the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual PropertyNode CreateNode(
        TypeNode parent, PropertyCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create an appropriate node using the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual FieldNode CreateNode(
        TypeNode parent, FieldCandidate candidate) => new(parent, candidate);

    /// <summary>
    /// Invoked to create an appropriate node using the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    protected virtual MethodNode CreateNode(
        TypeNode parent, MethodCandidate candidate) => new(parent, candidate);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the name of the file where the code of the tail type in the chain of types will be
    /// emitted into.
    /// </summary>
    /// <param name="nschain"></param>
    /// <param name="tpchain"></param>
    /// <returns></returns>
    protected virtual string GetFileName(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nschain,
        ImmutableArray<INamedTypeSymbol> tpchain)
    {
        return GetFileNameByTailType(nschain, tpchain);
    }

    /// <summary>
    /// Returns a suitable file name using the given chain of namespaces.
    /// </summary>
    /// <param name="nschain"></param>
    /// <returns></returns>
    protected string GetFileNameByTailNamespace(
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
    /// Returns a suitable file name using the given namespace and type chains.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked automatically by the compiler to initialize the generator and to register source
    /// code generation steps via callbacks on the context.
    /// <br/> Although this method is public, it is just infrastructure and shall not be called
    /// from user code.
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
    /// of the attributes applied to the given node match with any of the specified ones for the
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
    /// <br/> Note that if this method returns null, then it will simply be ignored and not be not
    /// taken into consideration for those purposes.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0019")]
    public virtual ICandidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (syntax is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax, token);
            if (symbol == null)
                return new ErrorCandidate(TreeDiagnostics.SymbolNotFoundForSyntax(typeSyntax));

            var atts = Matches(symbol.GetAttributes(), TypeAttributes).ToImmutableArray();
            if (atts.Length != 0)
                return new TypeCandidate(atts, model, typeSyntax, symbol);
        }

        // Properties...
        else if (syntax is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax, token);
            if (symbol == null)
                return new ErrorCandidate(TreeDiagnostics.SymbolNotFoundForSyntax(propertySyntax));

            var atts = Matches(symbol.GetAttributes(), PropertyAttributes).ToImmutableArray();
            if (atts.Length != 0)
                return new PropertyCandidate(atts, model, propertySyntax, symbol);
        }

        // Fields...
        else if (syntax is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;

            foreach (var item in items)
            {
                var symbol = model.GetDeclaredSymbol(item, token) as IFieldSymbol;
                if (symbol != null)
                {
                    var atts = Matches(symbol.GetAttributes(), FieldAttributes).ToImmutableArray();
                    if (atts.Length != 0)
                        return new FieldCandidate(atts, model, fieldSyntax, symbol);
                }
            }

            return new ErrorCandidate(TreeDiagnostics.SymbolNotFoundForSyntax(fieldSyntax));
        }

        // Methods...
        else if (syntax is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax, token);
            if (symbol == null)
                return new ErrorCandidate(TreeDiagnostics.SymbolNotFoundForSyntax(methodSyntax));

            var atts = Matches(symbol.GetAttributes(), PropertyAttributes).ToImmutableArray();
            if (atts.Length != 0)
                return new MethodCandidate(atts, model, methodSyntax, symbol);
        }

        // Finishing with no transformation...
        return new ErrorCandidate(TreeDiagnostics.UnknownSyntax(syntax));
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
        var files = new ChildFiles();
        var nschain = ImmutableArray<BaseNamespaceDeclarationSyntax>.Empty;
        var tpchain = ImmutableArray<INamedTypeSymbol>.Empty;
        var comparer = SymbolComparer.Default;
        INode parent = default!;

        candidates.OfType<ErrorCandidate>().ForEach(x => x.Diagnostic.Report(context));
        candidates.OfType<TypeCandidate>().ForEach(CaptureHierarchy);
        candidates.OfType<IValidCandidate>().ForEach(x => x is not TypeCandidate, CaptureHierarchy);

        foreach (var file in files)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!file.Validate(context)) continue;

            var cb = new CodeBuilder(); file.Emit(context, cb);
            var code = cb.ToString();
            var name = file.FileName + ".g.cs";
            context.AddSource(name, code);
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given candidate.
        /// </summary>
        void CaptureHierarchy(IValidCandidate candidate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            nschain = candidate.Syntax.GetNamespaceSyntaxChain();
            tpchain = candidate.Symbol.GetTypeSymbolChain();

            if (!CaptureFile(candidate)) return;
            if (!CaptureNamespace(candidate)) return;
            if (!CaptureType(candidate)) return;
            if (!CaptureProperty(candidate)) return;
            if (!CaptureField(candidate)) return;
            if (!CaptureMethod(candidate)) return;
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given file-level candidate.
        /// Returns <c>false</c> if errors are detected that prevents further execution.
        /// </summary>
        bool CaptureFile(IValidCandidate candidate)
        {
            var name = GetFileName(nschain, tpchain);
            var node = files.Find(x => string.Compare(x.FileName, name, ignoreCase: true) == 0);

            if (node == null) // No node, let's create a new one...
            {
                var comp = candidate.SemanticModel.Compilation;

                node = new FileNode(name, comp);
                files.Add(node);
            }

            parent = node;
            return true;
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given namespace-level candidate.
        /// Returns <c>false</c> if errors are detected that prevents further execution.
        /// </summary>
        bool CaptureNamespace(IValidCandidate candidate)
        {
            var list = ((FileNode)parent).ChildNamespaces;
            var len = nschain.Length;

            for (int index = 0; index < len; index++)
            {
                var syntax = nschain[index];
                var name = syntax.Name.ToString();
                var node = list.Find(x => string.Compare(x.Name, name) == 0);

                if (node == null) // No node, let's create a new one...
                {
                    node = new(parent, syntax);
                    list.Add(node);
                }

                parent = node;
                list = node.ChildNamespaces;
            }

            return true;
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given type-level candidate.
        /// Returns <c>false</c> if errors are detected that prevents further execution.
        /// </summary>
        bool CaptureType(IValidCandidate candidate)
        {
            var list = ((NamespaceNode)parent).ChildTypes;
            var len = tpchain.Length;

            // Main loop...
            for (int index = 0; index < len; index++)
            {
                // Find using symbol comparer, as we may have same name but different type args...
                var symbol = tpchain[index];
                var node = list.Find(x => comparer.Equals(x.Symbol, symbol));

                // If no type node, let's create a new one...
                if (node == null)
                {
                    // Create custom node...
                    if (candidate is TypeCandidate element && index == (len - 1))
                    {
                        node = CreateNode(parent, element);
                        list.Add(node);
                    }

                    // Create a structural one...
                    else
                    {
                        node = new(parent, symbol);
                        list.Add(node);
                    }
                }

                // Otherwise, validate consistency...
                else if (candidate is TypeCandidate element && index == (len - 1))
                {
                    // Substitute structural node if needed...
                    if (node.GetType() == typeof(TypeNode))
                    {
                        var temp = CreateNode(parent, element);
                        foreach (var child in node.ChildTypes) temp.ChildTypes.Add(child);
                        foreach (var child in node.ChildProperties) temp.ChildProperties.Add(child);
                        foreach (var child in node.ChildFields) temp.ChildFields.Add(child);
                        foreach (var child in node.ChildMethods) temp.ChildMethods.Add(child);

                        list.Remove(node);
                        list.Add(temp);
                    }

                    // Otherwise, validate custom node...
                    else
                    {
                        if (!comparer.Equals(node.Symbol, symbol))
                        {
                            TreeDiagnostics.InconsistentHierarchy(node, candidate).Report(context);
                            return false;
                        }
                    }
                }

                parent = node;
                list = node.ChildTypes;
            }

            return true;
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given property-level candidate.
        /// Returns <c>false</c> if errors are detected that prevents further execution.
        /// </summary>
        bool CaptureProperty(IValidCandidate candidate)
        {
            if (candidate is PropertyCandidate item)
            {
                var tpnode = (TypeNode)parent;
                var list = tpnode.ChildProperties;
                var symbol = item.Symbol;
                var node = list.Find(x => x.Symbol.Name == symbol.Name);

                if (node == null) // No node, let's create a new one...
                {
                    node = CreateNode(tpnode, item);
                    list.Add(node);
                }

                else // Validating consistency...
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
                            TreeDiagnostics.InconsistentHierarchy(node, candidate).Report(context);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given field-level candidate.
        /// Returns <c>false</c> if errors are detected that prevents further execution.
        /// </summary>
        bool CaptureField(IValidCandidate candidate)
        {
            if (candidate is FieldCandidate item)
            {
                var tpnode = (TypeNode)parent;
                var list = tpnode.ChildFields;
                var symbol = item.Symbol;
                var node = list.Find(x => x.Symbol.Name == symbol.Name);

                if (node == null) // No node, let's create a new one...
                {
                    node = CreateNode(tpnode, item);
                    list.Add(node);
                }

                else // Validating consistency...
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
                            TreeDiagnostics.InconsistentHierarchy(node, candidate).Report(context);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        // ------------------------------------------------
        /// <summary>
        /// Invoked to emit the hierarchy of the given method-level candidate.
        /// Returns <c>false</c> if errors are detected that prevents further execution.
        /// </summary>
        bool CaptureMethod(IValidCandidate candidate)
        {
            if (candidate is MethodCandidate item)
            {
                var tpnode = (TypeNode)parent;
                var list = tpnode.ChildMethods;
                var symbol = item.Symbol;

                // Special comparer for method elements...
                var xcomparer = SymbolComparer.Full with { UseNullability = false };
                var node = list.Find(x => xcomparer.Equals(x.Symbol, symbol));

                if (node == null) // No node, let's create a new one...
                {
                    node = CreateNode(tpnode, item);
                    list.Add(node);
                }

                else // Validating consistency...
                {
                    if (node.GetType() == typeof(FieldNode)) // Substitute structural...
                    {
                        var temp = CreateNode(tpnode, item);
                        list.Remove(node);
                        list.Add(temp);
                    }

                    else // Validating existing custom node...
                    {
                        if (!xcomparer.Equals(node.Symbol, symbol)) // Use special comparer...
                        {
                            TreeDiagnostics.InconsistentHierarchy(node, candidate).Report(context);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}