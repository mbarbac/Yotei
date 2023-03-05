namespace Yotei.Generators.Cloneable;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XType : Tree.TypeNode
{
    public XType(
        Tree.INode parent,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel model)
        : base(parent, syntax, symbol, model) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Must have a get accessor...
        if (Syntax is
            not InterfaceDeclarationSyntax and
            not ClassDeclarationSyntax and
            not StructDeclarationSyntax)
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                "Kerosene",
                "Type kind not supported.",
                "The type '{0}' must be a class, a struct or an interface.",
                "Build",
                DiagnosticSeverity.Error,
                true),
                Syntax.GetLocation(),
                new object[] { Name }));

            return false;
        }

        // Validations passed...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override string GetNameAndInterfaces()
    {
        return
            AddICloneable ||
            Symbol.Interfaces.Any(x => x.Name == nameof(ICloneable))
            ? $"{Name} : System.ICloneable"
            : Name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // If already implemented, we don't know what we're doing here...
        if (CloneMethod != null) return;

        // Common documentation...
        cb.AppendLine("/// <summary>");
        cb.AppendLine("/// <inheritdoc cref=\"System.ICloneable\"/>");
        cb.AppendLine("/// </summary>");
        cb.AppendLine("/// <returns><inheritdoc/></returns>");

        // We might need a 'new' modifier...
        var neo = NeedsNew ? "new " : string.Empty;

        // Interfaces...
        if (IsInterface)
        {
            cb.AppendLine($"{neo}{Name} Clone();");
        }

        // Implementation...
        else
        {
            cb.AppendLine($"public {neo}{Name} Clone()");
            cb.AppendLine("{");
            cb.Tabs++;

            var builder = new NewBuilder(Symbol);
            var done = builder.Print("item", cb);
            if (done) cb.AppendLine("return item;");
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                    "Kerosene",
                    "Cannot generate a new instance.",
                    "Cannot generate a new instance of type '{0}'.",
                    "Build",
                    DiagnosticSeverity.Error,
                    true),
                    Syntax.GetLocation(),
                    new object[] { Symbol.Name }));
            }

            cb.Tabs--;
            cb.AppendLine("}");

            foreach (var iface in InterfacesToImplement())
            {
                var name = iface.FullyQualifiedName();
                var type = iface.Name == nameof(ICloneable) ? "object" : name;

                cb.AppendLine();
                cb.AppendLine(type);
                cb.AppendLine($"{name}.Clone() => Clone();");
            }
        }

    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the symbol of the <see cref="ICloneable"/> interface.
    /// </summary>
    INamedTypeSymbol ICloneableType => _ICloneableType ?? FindICloneableType();
    INamedTypeSymbol? _ICloneableType = null;
    INamedTypeSymbol FindICloneableType()
    {
        return
            SemanticModel.Compilation.GetTypeByMetadataName("System.ICloneable") ??
            throw new UnexpectedException($"Cannot find the symbol of the 'ICloneable' interface.");
    }

    /// <summary>
    /// Determines if this type is marked for explicit mode, or not. This property is always
    /// false if the type is an interface.
    /// </summary>
    bool ExplicitMode => _ExplicitMode ?? GetExplicitMode(Symbol, IsInterface);
    bool? _ExplicitMode = null;
    bool GetExplicitMode(INamedTypeSymbol type, bool isInterface)
    {
        if (!isInterface)
        {
            if (type.HasAttribute(CloneableTypeSource.AttributeLongName, out var data))
            {
                var arg = data.GetNamedArgument(CloneableTypeSource.ExplicitMode);
                if (arg != null &&
                    arg.Value.Value is bool value && value) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines if this type is marked to add the <see cref="ICloneable"/> interface, or not.
    /// </summary>
    bool AddICloneable => _AddICloneable ?? GetAddICloneable(Symbol, IsInterface);
    bool? _AddICloneable = null;
    bool GetAddICloneable(INamedTypeSymbol type, bool isInterface)
    {
        if (type.HasAttribute(CloneableTypeSource.AttributeLongName, out var data))
        {
            var arg = data.GetNamedArgument(CloneableTypeSource.AddICloneable);

            if (arg == null) return true;
            if (arg.Value.Value is bool value && value) return true;
        }
        return false;
    }

    /// <summary>
    /// The symbol of the clone method declared by this type, or null if any.
    /// </summary>
    IMethodSymbol? CloneMethod => _CloneMethod ??= GetCloneMethod(Symbol);
    IMethodSymbol? _CloneMethod = null;
    IMethodSymbol? GetCloneMethod(INamedTypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().SingleOrDefault(x =>
            x.Name == nameof(ICloneable.Clone) &&
            x.Parameters.Length == 0 &&
            x.IsStatic == false && (
            x.DeclaredAccessibility == Accessibility.Public ||
            x.DeclaredAccessibility == Accessibility.Protected));
    }

    /// <summary>
    /// Determines if the method needs a "new" modifier, or not.
    /// </summary>
    bool NeedsNew => _NeedsNew ?? GetNeedsNew();
    bool? _NeedsNew = null;
    bool GetNeedsNew()
    {
        if (AddICloneable) return true;

        var parent = Symbol.BaseType;
        if (parent != null && GetNeedsNew(parent)) return true;

        var ifaces = Symbol.Interfaces;
        foreach (var iface in ifaces) if (GetNeedsNew(iface)) return true;

        return false;
    }
    bool GetNeedsNew(INamedTypeSymbol type)
    {
        if (GetCloneMethod(type) != null) return true;
        if (type.HasAttribute(CloneableTypeSource.AttributeLongName)) return true;

        var parent = type.BaseType;
        if (parent != null && GetNeedsNew(parent)) return true;

        var ifaces = type.Interfaces;
        foreach (var iface in ifaces) if (GetNeedsNew(iface)) return true;

        return false;
    }

    /// <summary>
    /// Gets the collection of interfaces to implement, or an empty one is this is an interface.
    /// </summary>
    /// <returns></returns>
    IEnumerable<INamedTypeSymbol> InterfacesToImplement()
    {
        if (IsInterface) return Array.Empty<INamedTypeSymbol>();

        var comp = SymbolEqualityComparer.Default;
        var list = new NoDuplicatesList<INamedTypeSymbol>(comp);

        if (AddICloneable) list.Add(ICloneableType);
        Populate(list, Symbol);
        return list;

        static void Populate(NoDuplicatesList<INamedTypeSymbol> list, INamedTypeSymbol type)
        {
            var items = type.Interfaces.Where(x =>
                x.Name == nameof(ICloneable) ||
                x.HasAttribute(CloneableTypeSource.AttributeLongName))
                .ToArray();

            foreach (var item in items) list.Add(item);

            var ifaces = type.Interfaces;
            foreach (var iface in ifaces) Populate(list, iface);
        }
    }
}