namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITypeNode"/>
/// </summary>
internal class TypeNode : ITypeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public TypeNode(
        INode parent,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel model)
    {
        Parent =
            parent is INamespaceNode ns ? ns :
            parent is ITypeNode tp ? tp :
            parent is null ? throw new ArgumentNullException(nameof(parent)) :
            throw new ArgumentException($"Invalid parent node: {parent}");

        Syntax = syntax.ThrowIfNull(nameof(syntax));
        Symbol = symbol.ThrowIfNull(nameof(symbol));
        SemanticModel = model.ThrowIfNull(nameof(model));
        Name = symbol.ShortName();

        if (symbol.IsNamespace) throw new ArgumentException(
            $"Symbol is a namespace not a type one: {symbol}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Type: {Name}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IGenerator Generator => Parent.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode Parent { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TypeDeclarationSyntax Syntax { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SemanticModel SemanticModel { get; }

    /// <summary>
    /// Determines if this instance represents an interface, or not.
    /// </summary>
    public bool IsInterface => Syntax is InterfaceDeclarationSyntax;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<ITypeNode> ChildTypes => _ChildTypes;
    readonly List<ITypeNode> _ChildTypes = new();

    ITypeNode LocateType(ICaptured captured, int tpIndex)
    {
        var type = captured.AsCapturedType();
        var syntax = type.TypeSyntaxChain[tpIndex];
        var symbol = type.TypeSymbolChain[tpIndex];
        var name = symbol.ShortName();
        var len = type.TypeSyntaxChain.Length;

        var node = _ChildTypes.Find(x => x.Name == name);
        if (node == null)
        {
            node = tpIndex < (len - 1)
                ? new TypeNode(this, syntax, symbol, captured.SemanticModel)
                : captured.Generator.CreateType(this, syntax, symbol, captured.SemanticModel);

            _ChildTypes.Add(node);
        }
        return node;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<IPropertyNode> ChildProperties => _ChildProperties;
    readonly List<IPropertyNode> _ChildProperties = new();

    IPropertyNode LocateProperty(ICapturedProperty captured)
    {
        var node = _ChildProperties.Find(x => x.Name == captured.Name);
        if (node == null)
        {
            var syntax = captured.Syntax;
            var symbol = captured.Symbol;
            var model = captured.SemanticModel;

            node = captured.Generator.CreateProperty(this, syntax, symbol, model);
            _ChildProperties.Add(node);
        }
        return node;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<IFieldNode> ChildFields => _ChildFields;
    readonly List<IFieldNode> _ChildFields = new();

    IFieldNode LocateField(ICapturedField captured)
    {
        var node = _ChildFields.Find(x => x.Name == captured.Name);
        if (node == null)
        {
            var syntax = captured.Syntax;
            var symbol = captured.Symbol;
            var model = captured.SemanticModel;

            node = captured.Generator.CreateField(this, syntax, symbol, model);
            _ChildFields.Add(node);
        }
        return node;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Register(ICaptured captured, int tpIndex)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var type = captured.AsCapturedType();
        var tpLen = type.TypeSyntaxChain.Length;
        var level = captured.Generator.CaptureLevel;

        // Terminal type...
        if (tpIndex == (tpLen - 1) && level == CaptureLevel.Type)
        {
            var node = captured.Generator.CreateType(this, type.Syntax, type.Symbol, type.SemanticModel);
            _ChildTypes.Add(node);
            return;
        }

        // Intermediate type...
        if (tpIndex < tpLen)
        {
            var node = LocateType(captured, tpIndex);
            node.Register(captured, tpIndex + 1);
        }

        // Terminal member...
        else
        {
            switch (level)
            {
                case CaptureLevel.Property: if (ForProperty()) return; break;
                case CaptureLevel.Field: if (ForField()) return; break;
                case CaptureLevel.PropertyOrField: if (ForPropertyOrField()) return; break;
            }
            throw new ArgumentException($"Invalid element: {captured}");

            bool ForProperty()
            {
                if (captured is ICapturedProperty item)
                {
                    LocateProperty(item);
                    return true;
                }
                return false;
            }

            bool ForField()
            {
                if (captured is ICapturedField item)
                {
                    LocateField(item);
                    return true;
                }
                return false;
            }

            bool ForPropertyOrField() => ForProperty() || ForField();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Add(ICapturedProperty captured)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var item = _ChildProperties.Find(x => x.Name == captured.Name);
        if (item != null) throw new DuplicateException(
            $"A property with the same name is alredy registered: {captured}");

        var syntax = captured.Syntax;
        var symbol = captured.Symbol;
        var model = captured.SemanticModel;
        var node = captured.Generator.CreateProperty(this, syntax, symbol, model);
        _ChildProperties.Add(node);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Add(ICapturedField captured)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var item = _ChildFields.Find(x => x.Name == captured.Name);
        if (item != null) throw new DuplicateException(
            $"A field with the same name is alredy registered: {captured}");

        var syntax = captured.Syntax;
        var symbol = captured.Symbol;
        var model = captured.SemanticModel;
        var node = captured.Generator.CreateField(this, syntax, symbol, model);
        _ChildFields.Add(node);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public virtual bool Validate(SourceProductionContext context)
    {
        // Partial type...
        if (!Syntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                "Kerosene",
                "Type must be partial",
                "The type '{0}' is not a partial one.",
                "Build",
                DiagnosticSeverity.Error,
                true),
                Syntax.GetLocation(),
                new object[] { Name }));

            return false;
        }

        // General validations...
        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Print(SourceProductionContext context, CodeBuilder cb)
    {
        var kind = Syntax.Keyword.ToString();
        var name = GetNameAndInterfaces();
        cb.AppendLine($"partial {kind} {name}");
        cb.AppendLine("{");
        cb.Tabs++;

        var num = 0;

        foreach (var node in ChildFields)
        {
            if (num > 0) cb.AppendLine(); num++;
            node.Print(context, cb);
        }

        foreach (var node in ChildProperties)
        {
            if (num > 0) cb.AppendLine(); num++;
            node.Print(context, cb);
        }

        OnPrint(context, cb);

        foreach (var node in ChildTypes)
        {
            if (num > 0) cb.AppendLine(); num++;
            node.Print(context, cb);
        }

        cb.Tabs--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to obtain the name of this instance or, if it needs to implement any interfaces,
    /// then that name followed by these interfaces using a " : comma-separated-names" format.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetNameAndInterfaces() => Name;

    /// <summary>
    /// Invoked to print the code of the contents of this instance.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void OnPrint(SourceProductionContext context, CodeBuilder cb) { }
}