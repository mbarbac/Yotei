namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Base class for incremental source generators. Derived classes nust be decorated with the
/// <see cref="GeneratorAttribute"/> to be recognized by the compiler, using as its argument
/// <c>(LanguageNames.CSharp)</c>.
/// </summary>
internal abstract class BaseGenerator : IIncrementalGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        context.RegisterPostInitializationOutput(OnInitialized);

        var items = context.SyntaxProvider
            .CreateSyntaxProvider(Validate, Transform)
            .Collect();

        context.RegisterSourceOutput(items, Print);
    }

    /// <summary>
    /// Determines if this instance will try to launch a debugging session when invoked by the
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
    /// Invoked to determine if the given syntax node shall be tranformed into a candidate for
    /// source code generation, or not.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            if (TypeAttributes.Length > 0)
            {
                var atts = typeSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, TypeAttributes));
            }
            return false;
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            if (PropertyAttributes.Length > 0)
            {
                var atts = propertySyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, PropertyAttributes));
            }
            return false;
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            if (FieldAttributes.Length > 0)
            {
                var atts = fieldSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, FieldAttributes));
            }
            return false;
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            if (MethodAttributes.Length > 0)
            {
                var atts = methodSyntax.AttributeLists.GetAttributes();
                return atts.Any(x => Compare(x.Name, MethodAttributes));
            }
            return false;
        }

        // Not supported...
        return false;

        /// <summary>
        /// Invoked to determine if the given attribute name matches any of the target ones.
        /// </summary>
        static bool Compare(NameSyntax syntax, string[] targets)
        {
            var span = "Attribute".AsSpan();
            var source = syntax.ShortName().AsSpan().RemoveLast(span);

            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i].AsSpan().RemoveLast(span);
                if (source.CompareTo(target, StringComparison.Ordinal) == 0) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// The collection of attribute names that, if decorates a given type, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] TypeAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorates a given property, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] PropertyAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorates a given field, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] FieldAttributes { get; } = [];

    /// <summary>
    /// The collection of attribute names that, if decorates a given method, identify it as a
    /// candidate for source code generation.
    /// </summary>
    public virtual string[] MethodAttributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to transform the syntax node carried by the generation context into a suitable
    /// candidate for source code generation.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual Candidate Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = context.Node;
        var model = context.SemanticModel;

        // Types...
        if (node is TypeDeclarationSyntax typeSyntax)
        {
            var symbol = model.GetDeclaredSymbol(typeSyntax)
                ?? throw new InvalidOperationException(
                "Cannot obtain symbol for type node.").WithData(typeSyntax, nameof(node));
            
            return new TypeCandidate(model, typeSyntax, symbol);
        }

        // Properties...
        if (node is PropertyDeclarationSyntax propertySyntax)
        {
            var symbol = model.GetDeclaredSymbol(propertySyntax)
                ?? throw new InvalidOperationException(
                "Cannot obtain symbol for property node.").WithData(propertySyntax, nameof(node));
            
            return new PropertyCandidate(model, propertySyntax, symbol);
        }

        // Fields...
        if (node is FieldDeclarationSyntax fieldSyntax)
        {
            var items = fieldSyntax.Declaration.Variables;
            foreach (var item in items)
            {
                if (model.GetDeclaredSymbol(item) is IFieldSymbol symbol)
                    return new FieldCandidate(model, fieldSyntax, symbol);
            }
            throw new InvalidOperationException(
                "Cannot obtain symbol for field node.").WithData(fieldSyntax, nameof(node));
        }

        // Methods...
        if (node is MethodDeclarationSyntax methodSyntax)
        {
            var symbol = model.GetDeclaredSymbol(methodSyntax)
                ?? throw new InvalidOperationException(
                "Cannot obtain symbol for method node.").WithData(methodSyntax, nameof(node));

            return new MethodCandidate(model, methodSyntax, symbol);
        }

        // Not supported...
        throw new ArgumentException("Unsupported syntax node.").WithData(node, nameof(node));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual TypeNode CreateType(TypeCandidate candidate) => new(candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual PropertyNode CreateProperty(PropertyCandidate candidate) => new(candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual FieldNode CreateField(FieldCandidate candidate) => new(candidate);

    /// <summary>
    /// Invoked to create a node of the appropriate type.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public virtual MethodNode CreateMethod(MethodCandidate candidate) => new(candidate);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the collection of identified candidates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidates"></param>
    protected virtual void Print(SourceProductionContext context, ImmutableArray<Candidate> candidates)
    {
        var files = new Dictionary<string, FileBuilder>();

        // Generating the collection of files...
        foreach (var candidate in candidates)
        {
            if (candidate is null) continue;
            context.CancellationToken.ThrowIfCancellationRequested();

            var name = candidate.GetFileName();
            if (!files.TryGetValue(name, out var file))
            {
                file = new FileBuilder(name);
                files.Add(name, file);
            }
            file.Candidates.Add(candidate);
        }

        // Generating the source code...
        foreach (var file in files.Values)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            PrintHeader(file);
            PrintPragmas(file);
            PrintUsings(file);

            foreach (var candidate in file.Candidates)
            {
                Node node = candidate switch
                {
                    TypeCandidate item => CreateType(item),
                    PropertyCandidate item => CreateProperty(item),
                    FieldCandidate item => CreateField(item),
                    MethodCandidate item => CreateMethod(item),

                    _ => throw new ArgumentException("Unsupported candidate").WithData(candidate, nameof(candidate))
                };
                PrintNode(context, file, candidate, node);
            }

            var name = file.FileName + ".g.cs";
            var code = file.GetCode();
            context.AddSource(name, code);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the file headers.
    /// </summary>
    /// <param name="file"></param>
    void PrintHeader(FileBuilder file)
    {
        file.AppendLine("// <auto-generated/>");
        file.AppendLine("#nullable enable");
    }

    /// <summary>
    /// Emits the file-level pragmas.
    /// </summary>
    /// <param name="file"></param>
    void PrintPragmas(FileBuilder file)
    {
        var list = new CoreList<string>()
        {
            AllowDuplicate = (x, y) => false,
            Compare = (source, target) => source == target,
        };

        foreach (var candidate in file.Candidates)
        {
            var syntax = candidate.NamespaceSyntaxChain[0];
            var tree = syntax.SyntaxTree;
            if (tree.TryGetText(out var text))
            {
                foreach (var line in text.Lines)
                {
                    var str = line.ToString().Trim();
                    if (str.StartsWith("#pragma")) list.Add(str);
                    if (str.StartsWith("namespace")) break;
                }
            }
        }

        if (list.Count > 0)
        {
            file.AppendLine();
            foreach (var item in list) file.AppendLine(item);
        }
    }

    /// <summary>
    /// Emits the file-level usings.
    /// </summary>
    /// <param name="file"></param>
    void PrintUsings(FileBuilder file)
    {
        var list = new CoreList<string>()
        {
            AllowDuplicate = (x, y) => false,
            Compare = (source, target) => source == target,
        };

        foreach (var candidate in file.Candidates)
        {
            var root = candidate.NamespaceSyntaxChain[0];
            var comp = root.GetCompilationUnitSyntax();

            foreach (var item in comp.Usings)
            {
                var str = item.ToString();
                if (str != null && str.Length > 0) list.Add(str);
            }
        }

        if (list.Count > 0)
        {
            file.AppendLine();
            foreach (var item in list) file.AppendLine(item);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the given node.
    /// </summary>
    void PrintNode(
        SourceProductionContext context, FileBuilder file, Candidate candidate, Node node)
    {
        if (!node.Validate(context)) return;

        file.AppendLine();

        foreach (var ns in candidate.NamespaceSyntaxChain)
        {
            file.AppendLine($"namespace {ns.Name.LongName()}");
            file.AppendLine("{");
            file.IndentLevel++;

            PrintUsings(file, ns);
        }

        foreach (var tp in candidate.TypeSymbolChain)
        {
            var rec = tp.IsRecord ? "record " : string.Empty;
            var kind = rec + GetTypeKind(tp);
            var name = tp.GivenName(addNullable: false);

            file.AppendLine($"partial {kind} {name}");
            file.AppendLine("{");
            file.IndentLevel++;
        }

        node.Print(context, file);

        for (int i = 0; i < candidate.TypeSymbolChain.Length; i++)
        {
            file.IndentLevel--;
            file.AppendLine("}");
        }

        for (int i = 0; i < candidate.NamespaceSyntaxChain.Length; i++)
        {
            file.IndentLevel--;
            file.AppendLine("}");
        }
    }

    /// <summary>
    /// Invoked to print the namespace-level usings, if any.
    /// </summary>
    void PrintUsings(FileBuilder file, BaseNamespaceDeclarationSyntax ns)
    {
        var list = new CoreList<string>()
        {
            AllowDuplicate = (x, y) => false,
            Compare = (source, target) => source == target,
        };

        foreach (var item in ns.Usings)
        {
            var str = item.ToString();
            if (str != null && str.Length > 0) list.Add(str);
        }

        if (list.Count > 0)
        {
            foreach (var item in list) file.AppendLine(item);
            file.AppendLine();
        }
    }

    /// <summary>
    /// Invoked to get the kind of the given type.
    /// </summary>
    string GetTypeKind(ITypeSymbol symbol) => symbol.TypeKind switch
    {
        TypeKind.Class => "class",
        TypeKind.Struct => "struct",
        TypeKind.Interface => "interface",

        _ => throw new ArgumentException("Invalid type kind.").WithData(symbol, nameof(symbol))
    };
}