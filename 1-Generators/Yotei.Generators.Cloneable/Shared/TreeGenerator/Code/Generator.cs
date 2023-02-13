namespace Yotei.Generators.Tree;

// ========================================================
/// <inheritdoc cref="IGenerator">
/// </inheritdoc>
public abstract class Generator : IGenerator
{
    /// <inheritdoc>
    /// </inheritdoc>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        context.RegisterPostInitializationOutput(PostInitialization);

        var items = context.SyntaxProvider.ForAttributeWithMetadataName(
            ValidatedAttributeName,
            ValidateNode,
            TransformNode)
            .Collect();

        context.RegisterSourceOutput(items, CodeGenerator);
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public abstract bool LaunchDebugger { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual void PostInitialization(IncrementalGeneratorPostInitializationContext context) { }

    /// <inheritdoc>
    /// </inheritdoc>
    public abstract string AttributeName { get; }

    string ValidatedAttributeName => _ValidatedAttributeName ??= GetValidatedAttributeName();
    string? _ValidatedAttributeName = null;
    string GetValidatedAttributeName()
    {
        var name = AttributeName.NotNullNotEmpty(nameof(AttributeName));
        if (!name.EndsWith(ATTRIBUTE)) name += ATTRIBUTE;
        return name;
    }

    private const string ATTRIBUTE = "Attribute";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate if the given syntax node can be considered as a potential candidate
    /// for this generator, or not.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    bool ValidateNode(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        switch (TreeLevel)
        {
            case TreeLevel.Type: if (ForType()) return true; break;
            case TreeLevel.Property: if (ForProperty()) return true; break;
            case TreeLevel.Field: if (ForField()) return true; break;
            case TreeLevel.PropertyOrField: if (ForPropertyOrField()) return true; break;

            default: throw new UnreachableException($"Invalid capture level: {TreeLevel}");
        }

        return false;

        // Validates a type-alike node...
        bool ForType() => node is
            InterfaceDeclarationSyntax or ClassDeclarationSyntax or
            StructDeclarationSyntax or RecordDeclarationSyntax;

        // Validates a property-alike node...
        bool ForProperty() => node is PropertyDeclarationSyntax;

        // Validates a field-alike node...
        bool ForField() => node is FieldDeclarationSyntax;

        // Validates a property-alike or field-alike node...
        bool ForPropertyOrField() => ForProperty() || ForField();
    }

    /// <summary>
    /// Invoked to transform a candidate syntax node into a valid captured element for code
    /// generation purposes, or returns null if such transformation is not possible.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ICaptured? TransformNode(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var syntax = context.TargetNode;
        var symbol = context.TargetSymbol;
        var semantic = context.SemanticModel;
        ICaptured? captured;

        switch (TreeLevel)
        {
            case TreeLevel.Type: if ((captured = ToType()) != null) return captured; break;
            case TreeLevel.Property: if ((captured = ToProperty()) != null) return captured; break;
            case TreeLevel.Field: if ((captured = ToField()) != null) return captured; break;
            case TreeLevel.PropertyOrField: if ((captured = ToPropertyOrField()) != null) return captured; break;

            default: throw new UnreachableException($"Invalid capture level: {TreeLevel}");
        }

        return null;

        // Captures a type-alike element...
        ICapturedType? ToType()
        {
            var typeSyntax = syntax as TypeDeclarationSyntax; if (typeSyntax == null) return null;
            var typeSymbol = symbol as INamedTypeSymbol; if (typeSymbol == null) return null;

            return CreateCapturedType(semantic, this, typeSyntax, typeSymbol);
        }

        // Captures a property-alike element...
        ICapturedProperty? ToProperty()
        {
            var propSyntax = syntax as PropertyDeclarationSyntax; if (propSyntax == null) return null;
            var propSymbol = symbol as IPropertySymbol; if (propSymbol == null) return null;

            var typeSyntax = propSyntax.Parent as TypeDeclarationSyntax; if (typeSyntax == null) return null;
            var typeSymbol = semantic.GetDeclaredSymbol(typeSyntax); if (typeSymbol == null) return null;

            var type = CreateCapturedType(semantic, this, typeSyntax, typeSymbol);
            return CreateCapturedProperty(type, propSyntax, propSymbol);
        }

        // Captures a field-alike element...
        ICapturedField? ToField()
        {
            var fieldSyntax = syntax as FieldDeclarationSyntax; if (fieldSyntax == null) return null;
            var fieldSymbol = symbol as IFieldSymbol; if (fieldSymbol == null) return null;

            var typeSyntax = fieldSyntax.Parent as TypeDeclarationSyntax; if (typeSyntax == null) return null;
            var typeSymbol = semantic.GetDeclaredSymbol(typeSyntax); if (typeSymbol == null) return null;

            var type = CreateCapturedType(semantic, this, typeSyntax, typeSymbol);
            return CreateCapturedField(type, fieldSyntax, fieldSymbol);
        }

        // Captures a property-alike or a field-alike element...
        ICaptured? ToPropertyOrField()
        {
            return (ICaptured?)ToProperty() ?? ToField();
        }
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public abstract TreeLevel TreeLevel { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual ICapturedType CreateCapturedType(
        SemanticModel semanticModel, IGenerator generator,
        TypeDeclarationSyntax typeSyntax, INamedTypeSymbol typeSymbol)
        => new CapturedType(semanticModel, generator, typeSyntax, typeSymbol);

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual ICapturedProperty CreateCapturedProperty(
        ICapturedType capturedType,
        PropertyDeclarationSyntax propSyntax, IPropertySymbol propSymbol)
        => new CapturedProperty(capturedType, propSyntax, propSymbol);

    /// <inheritdoc>
    /// </inheritdoc>
    public virtual ICapturedField CreateCapturedField(
        ICapturedType capturedType,
        FieldDeclarationSyntax fieldSyntax, IFieldSymbol fieldSymbol)
        => new CapturedField(capturedType, fieldSyntax, fieldSymbol);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the collection of captured elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="items"></param>
    void CodeGenerator(SourceProductionContext context, ImmutableArray<ICaptured?> items)
    {
        var nodes = new List<FileNode>();

        foreach (var item in items)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (item == null) continue;

            var type = item.AsCapturedType();
            var name = FileName(type.NamespaceSyntaxChain, type.TypeSyntaxChain);
            var comp = StringComparison.OrdinalIgnoreCase;
            var node = nodes.Find(x => string.Compare(name, x.Name, comp) == 0);

            if (node == null) nodes.Add(node = new FileNode(name));
            node.Register(item);
        }

        foreach (var node in nodes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (node.Validate(context))
            {
                var cb = new CodeBuilder();
                node.Print(context, cb);
            }
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public abstract string FileName(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nsSyntaxChain,
        ImmutableArray<TypeDeclarationSyntax> tpSyntaxChain);

    /// <summary>
    /// Returns a file name for the top namespace in the given chain.
    /// </summary>
    /// <param name="nsSyntaxChain"></param>
    /// <returns></returns>
    protected string TopNamespaceFileName(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nsSyntaxChain)
    {
        nsSyntaxChain = nsSyntaxChain.ThrowIfNull(nameof(nsSyntaxChain));
        if (nsSyntaxChain.Length == 0) throw new ArgumentException("Namespace syntax chain is empty.");

        var name = nsSyntaxChain[0].Name.LongName();
        var parts = name.Split('.');
        Array.Reverse(parts);
        return string.Join(".", parts);
    }

    /// <summary>
    /// Returns a file name for the tail type in the given chain chains.
    /// </summary>
    /// <param name="nsSyntaxChain"></param>
    /// <param name="tpSyntaxChain"></param>
    /// <returns></returns>
    protected string TailTypeFileName(
        ImmutableArray<BaseNamespaceDeclarationSyntax> nsSyntaxChain,
        ImmutableArray<TypeDeclarationSyntax> tpSyntaxChain)
    {
        nsSyntaxChain = nsSyntaxChain.ThrowIfNull(nameof(nsSyntaxChain));
        tpSyntaxChain = tpSyntaxChain.ThrowIfNull(nameof(tpSyntaxChain));

        if (nsSyntaxChain.Length == 0) throw new ArgumentException("Namespace syntax chain is empty.");
        if (tpSyntaxChain.Length == 0) throw new ArgumentException("Type syntax chain is empty.");

        var list = new List<string>();
        for (int i = 0; i < nsSyntaxChain.Length; i++) list.Add(nsSyntaxChain[i].Name.LongName());
        for (int i = 0; i < tpSyntaxChain.Length; i++) list.Add(tpSyntaxChain[i].Identifier.Text);

        list.Reverse();
        return string.Join(".", list);
    }
}