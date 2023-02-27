namespace Yotei.Generators.Cloneable;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XType : Tree.TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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

        // Type must be of a supported kind...
        if (Syntax is
            not InterfaceDeclarationSyntax and
            not ClassDeclarationSyntax and
            not StructDeclarationSyntax)
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                "Kerosene",
                "Type kind not supported",
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
        var name = Name;

        if (AddICloneable ||
            Symbol.Interfaces.Any(x => x.Name == nameof(ICloneable)))
            name += $" : System.{nameof(ICloneable)}";

        return name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // We don't need to reimplement and already implemented method...
        if (CloneMethod != null) return;

        cb.AppendLine("/// <summary>");
        cb.AppendLine("/// <inheritdoc cref=\"System.ICloneable\"/>");
        cb.AppendLine("/// </summary>");

        var neo = NeedsNew ? "new " : string.Empty;

        if (IsInterface) cb.AppendLine($"{neo}{Name} Clone();");
        else
        {
            cb.AppendLine($"public {neo}{Name} Clone()");
            cb.AppendLine("{");
            cb.Tabs++;

            var builder = new MyItemBuilder(this);
            var props = builder.GetWriteProperties(builder.GetReadProperties(builder.GetProperties())).ToDebugArray();
            var fields = builder.GetWriteFields(builder.GetReadFields(builder.GetFields())).ToDebugArray();

            var args = new List<NewItemArgument>();
            foreach (var prop in props)
            {
                var arg = new NewItemArgument(prop, prop.IsDeep());
                args.Add(arg);
            }
            foreach (var field in fields)
            {
                var arg = new NewItemArgument(field, field.IsDeep());
                args.Add(arg);
            }

            var done = builder.Print("item", context, cb, args.ToArray());
            if (done) cb.AppendLine($"return item;");

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

    internal class MyItemBuilder : NewItemBuilder
    {
        public MyItemBuilder(XType master) : base(master.Syntax, master.Symbol) => Master = master;
        readonly XType Master;

        public override IEnumerable<IPropertySymbol> GetProperties() =>
            base.GetProperties().Where(x =>
            x.IsIgnore() == false &&
            (Master.ExplicitMode ?
                x.HasAttribute(CloneableMemberSource.AttributeLongName)
                : true));

        public override IEnumerable<IFieldSymbol> GetFields() =>
            base.GetFields().Where(x =>
            x.IsIgnore() == false &&
            (Master.ExplicitMode ?
                x.HasAttribute(CloneableMemberSource.AttributeLongName)
                : true));
    }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the <see cref="ICloneable"/> interface.
    /// </summary>
    INamedTypeSymbol ICloneableType => _ICloneableType ??= FindICloneableType();
    INamedTypeSymbol? _ICloneableType = null;
    INamedTypeSymbol FindICloneableType()
    {
        return
            SemanticModel.Compilation.GetTypeByMetadataName("System.ICloneable") ??
            throw new UnexpectedException("Cannot find the ICloneable interface.");
    }

    /// <summary>
    /// Determines if this type is marked for explicit mode, or not.
    /// </summary>
    bool ExplicitMode => _ExplicitMode ??= GetExplicitMode(Symbol, IsInterface);
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
    /// Determines if this type is marked for adding the <see cref="ICloneable"/> interface,
    /// or not.
    /// </summary>
    bool AddICloneable => _AddICloneable ??= GetAddICloneable(Symbol, IsInterface);
    bool? _AddICloneable = null;
    bool GetAddICloneable(INamedTypeSymbol type, bool isInterface)
    {
        if (type.HasAttribute(CloneableTypeSource.AttributeLongName, out var data))
        {
            var arg = data.GetNamedArgument(CloneableTypeSource.AddICloneable);
            if (arg != null &&
                arg.Value.Value is bool value && value) return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the Clone() method declared by this type, or null if any.
    /// </summary>
    IMethodSymbol? CloneMethod => _CloneMethod ??= GetCloneMethod(Symbol);
    IMethodSymbol? _CloneMethod = null;
    IMethodSymbol? GetCloneMethod(INamedTypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().SingleOrDefault(x =>
            x.Name == nameof(ICloneable.Clone) &&
            x.Parameters.Length == 0 &&
            !x.IsStatic && (
            x.DeclaredAccessibility == Accessibility.Public ||
            x.DeclaredAccessibility == Accessibility.Protected));
    }

    /// <summary>
    /// Gets the interfaces to implement. Assumes it is invoked from a non-interface type.
    /// </summary>
    IEnumerable<INamedTypeSymbol> InterfacesToImplement()
    {
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

    /// <summary>
    /// Determines if the generated 'Clone()' method needs a 'new' modifier, or not.
    /// </summary>
    bool NeedsNew => _NeedsNew ??= GetNeedsNew();
    bool? _NeedsNew = null;
    bool GetNeedsNew()
    {
        if (IsInterface && AddICloneable) return true;
        return Chain(Symbol, IsInterface);

        bool Chain(INamedTypeSymbol type, bool isInterface)
        {
            if (!isInterface)
            {
                ISymbol? parent = type.BaseType;
                while (parent != null)
                {
                    if (parent is INamedTypeSymbol named)
                    {
                        if (GetCloneMethod(named) != null) return true;
                        parent = named.BaseType;
                    }
                    else parent = null;
                }
            }

            var ifaces = type.AllInterfaces.Where(x =>
                x.Name == nameof(ICloneable) ||
                x.HasAttribute(CloneableTypeSource.AttributeLongName) ||
                GetCloneMethod(x) != null)
                .ToDebugArray();

            if (ifaces.Any()) return true;
            return false;
        }
    }
}