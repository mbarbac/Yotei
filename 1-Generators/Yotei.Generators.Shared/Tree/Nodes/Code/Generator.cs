using System.Net.Http.Headers;

namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="IGenerator"/>
/// </summary>
internal abstract class Generator : IGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (LaunchDebugger && !Debugger.IsAttached) Debugger.Launch();

        context.RegisterPostInitializationOutput(PostInitialization);

        var items = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName,
            ValidateNode,
            TransformNode)
            .Collect();

        context.RegisterSourceOutput(items, CodeGenerator);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool LaunchDebugger { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void PostInitialization(
        IncrementalGeneratorPostInitializationContext context)
    { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract string AttributeName { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract CaptureLevel CaptureLevel { get; }

    /// <summary>
    /// Invoked to create a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual ITypeNode CreateType(
        INode parent,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel model)
        => new TypeNode(parent, syntax, symbol, model);

    /// <summary>
    /// Invoked to create a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual IPropertyNode CreateProperty(
        ITypeNode parent,
        PropertyDeclarationSyntax syntax, IPropertySymbol symbol, SemanticModel model)
        => new PropertyNode(parent, syntax, symbol, model);

    /// <summary>
    /// Invoked to create a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual IFieldNode CreateField(
        ITypeNode parent,
        FieldDeclarationSyntax syntax, IFieldSymbol symbol, SemanticModel model)
        => new FieldNode(parent, syntax, symbol, model);

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

        switch (CaptureLevel)
        {
            case CaptureLevel.Type: if (ForType()) return true; break;
            case CaptureLevel.Property: if (ForProperty()) return true; break;
            case CaptureLevel.Field: if (ForField()) return true; break;
            case CaptureLevel.PropertyOrField: if (ForPropertyOrField()) return true; break;

            default: throw new UnreachableException($"Unknown Level: {CaptureLevel}");
        }

        // Cannot validate...
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

    // ----------------------------------------------------

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
        switch (CaptureLevel)
        {
            case CaptureLevel.Type: if ((captured = ToType()) != null) return captured; break;
            case CaptureLevel.Property: if ((captured = ToProperty()) != null) return captured; break;
            case CaptureLevel.Field: if ((captured = ToField()) != null) return captured; break;
            case CaptureLevel.PropertyOrField: if ((captured = ToPropertyOrField()) != null) return captured; break;

            default: throw new UnreachableException($"Unknown level: {CaptureLevel}");
        }

        return null;

        // Creates a type-alike element...
        ICapturedType? ToType()
        {
            var itemSyntax = syntax as TypeDeclarationSyntax;
            if (itemSyntax == null) return null;

            var itemSymbol = symbol as INamedTypeSymbol;
            if (itemSymbol == null) return null;

            return new CapturedType(this, itemSyntax, itemSymbol, semantic!);
        }

        // Creates a property-alike element...
        ICapturedProperty? ToProperty()
        {
            var itemSyntax = syntax as PropertyDeclarationSyntax;
            if (itemSyntax == null) return null;

            var itemSymbol = symbol as IPropertySymbol;
            if (itemSymbol == null) return null;

            var tpSyntax = itemSyntax.Parent as TypeDeclarationSyntax;
            if (tpSyntax == null) return null;

            var tpSymbol = semantic.GetDeclaredSymbol(tpSyntax);
            if (tpSymbol == null) return null;

            var type = new CapturedType(this, tpSyntax, tpSymbol, semantic!);
            return new CapturedProperty(this, type, itemSyntax, itemSymbol, semantic!);
        }

        // Creates a field-alike element...
        ICapturedField? ToField()
        {
            var itemSyntax = syntax as FieldDeclarationSyntax;
            if (itemSyntax == null) return null;

            var itemSymbol = symbol as IFieldSymbol;
            if (itemSymbol == null) return null;

            var tpSyntax = itemSyntax.Parent as TypeDeclarationSyntax;
            if (tpSyntax == null) return null;

            var tpSymbol = semantic.GetDeclaredSymbol(tpSyntax);
            if (tpSymbol == null) return null;

            var type = new CapturedType(this, tpSyntax, tpSymbol, semantic!);
            return new CapturedField(this, type, itemSyntax, itemSymbol, semantic!);
        }

        // Creates a property-alike or a field-alike element...
        ICaptured? ToPropertyOrField()
        {
            return (ICaptured?)ToProperty() ?? ToField();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the collection of captured elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="items"></param>
    void CodeGenerator(SourceProductionContext context, ImmutableArray<ICaptured?> items)
    {
        var files = new List<IFileNode>();
        IFileNode Locate(string name)
        {
            var comp = StringComparison.OrdinalIgnoreCase;
            var temp = files!.Find(x => string.Compare(name, x.Name, comp) == 0);
            if (temp == null)
            {
                temp = new FileNode(this, name);
                files.Add(temp);
            }
            return temp;
        }

        foreach (var item in items)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (item == null) continue;

            var name = GetFileName(item);
            var node = Locate(name);
            node.Register(item);
        }

        foreach (var node in files)
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

    /// <summary>
    /// Invoked to obtain the appropriate file name where the source code for the given captured
    /// element will be emitted.
    /// </summary>
    /// <param name="captured"></param>
    /// <returns></returns>
    public abstract string GetFileName(ICaptured captured);

    /// <summary>
    /// Obtains the file name associated with the top namespace of the given element.
    /// </summary>
    /// <param name="captured"></param>
    /// <returns></returns>
    public static string GetTopNamespaceFileName(ICaptured captured)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var type = captured.AsCapturedType();
        var nsChain = type.NamespaceSyntaxChain;

        var name = nsChain[0].Name.LongName();
        var parts = name.Split('.');
        Array.Reverse(parts);
        return string.Join(".", parts);
    }

    /// <summary>
    /// Obtains the file name associated with the tail type of the given element.
    /// </summary>
    /// <param name="captured"></param>
    /// <returns></returns>
    public static string GetTailTypeFileName(ICaptured captured)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var type = captured.AsCapturedType();
        var nsChain = type.NamespaceSyntaxChain;
        var tpChain = type.TypeSyntaxChain;

        var list = new List<string>();
        for (int i = 0; i < nsChain.Length; i++) list.Add(nsChain[i].Name.LongName());
        for (int i = 0; i < tpChain.Length; i++) list.Add(tpChain[i].Identifier.Text);

        list.Reverse();
        return string.Join(".", list);
    }
}