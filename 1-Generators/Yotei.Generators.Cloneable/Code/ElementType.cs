namespace Yotei.Generators.Cloneable;

// ========================================================
/// <inheritdoc cref="Tree.CapturedType">
/// </inheritdoc>
public class ElementType : Tree.CapturedType
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="semantic"></param>
    /// <param name="generator"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public ElementType(
        SemanticModel semantic, Tree.IGenerator generator,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        : base(semantic, generator, syntax, symbol) { }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Type must be of a supported kind...
        if (TypeSyntax is
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
                TypeSyntax.GetLocation(),
                new object[] { TypeSyntax.Identifier.Text }));

            return false;
        }

        // Validations passed...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the appropriate documentation.
    /// </summary>
    void AddDocumentation(CodeBuilder cb)
    {
        cb.AppendLine("/// <summary>");
        cb.AppendLine("/// <inheritdoc cref=\"System.ICloneable\"/>");
        cb.AppendLine("/// </summary>");
    }

    /// <inheritdoc>
    /// </inheritdoc>
    protected override string GetNameAndInterfaces()
    {
        var name = Name;

        if (!PreventAddICloneable &&
            !TypeSymbol.Interfaces.Any(x => x.Name == nameof(ICloneable)))
            name += $" : System.{nameof(ICloneable)}";

        return name;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // Interfaces...
        if (IsInterface)
        {
            // If already declared, we need not to re-implement...
            if (CloneMethod == null)
            {
                var neo = NeedsNew ? "new " : string.Empty;

                AddDocumentation(cb);
                cb.AppendLine($"{neo}{Name} Clone();");
            }
        }

        // Concrete types...
        else
        {
            // If already declared, we need not to re-implement...
            if (CloneMethod == null)
            {
                var neo = NeedsNew ? "new " : string.Empty;

                AddDocumentation(cb);
                cb.AppendLine($"public {neo}{Name} Clone()");
                cb.AppendLine("{");
                cb.Tabs++;

                var name = "temp";
                var done = new MyBuilder(this).Print(name, cb);

                if (done) cb.AppendLine($"return {name};");
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                        "Kerosene",
                        "Cannot build a new instance",
                        "Cannot find a suitable constructor for cloning type '{0}'.",
                        "Build",
                        DiagnosticSeverity.Error,
                        true),
                        TypeSyntax.GetLocation(),
                        new object[] { Name }));
                }

                cb.Tabs--;
                cb.AppendLine("}");
            }

            // Implementing needed interfaces...
            foreach (var iface in InterfacesToImplement)
            {
                var name = iface.FullyQualifiedName();
                var type = iface.Name == nameof(ICloneable) ? "object" : name;

                cb.AppendLine();
                cb.AppendLine($"{type}");
                cb.AppendLine($"{name}.Clone() => Clone();");
            }
        }
    }

    /// <inheritdoc>
    /// </inheritdoc>
    internal class MyBuilder : NamedTypeBuilder
    {
        public MyBuilder(ElementType master) : base(master.TypeSymbol) => Master = master;
        ElementType Master;

        public override IEnumerable<IPropertySymbol> GetReadProperties() =>
            base.GetReadProperties().Where(x =>
                x.IgnoreClone() == false &&
                (Master.ExplicitMode ? x.HasAttribute(CloneableTypeSource.Name) : true));

        public override IEnumerable<IFieldSymbol> GetReadFields() =>
            base.GetReadFields().Where(x =>
                x.IgnoreClone() == false &&
                (Master.ExplicitMode ? x.HasAttribute(CloneableTypeSource.Name) : true));

        public override string GetCode(IPropertySymbol symbol) => symbol.GetCode();

        public override string GetCode(IFieldSymbol symbol) => symbol.GetCode();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The symbol of the <see cref="ICloneable"/> interface.
    /// </summary>
    INamedTypeSymbol ICloneableType => _ICloneableType ??= FindICloneableType();
    INamedTypeSymbol? _ICloneableType = null;
    INamedTypeSymbol FindICloneableType()
    {
        return
            SemanticModel.Compilation.GetTypeByMetadataName("System.ICloneable") ??
            throw new InvalidOperationException("Cannot find the ICloneable interface.");
    }

    /// <summary>
    /// Determines if the type is marked for explicit mode, or not.
    /// </summary>
    bool ExplicitMode => _ExplicitMode ??= GetExplicitMode(TypeSymbol, IsInterface);
    bool? _ExplicitMode = null;
    static bool GetExplicitMode(INamedTypeSymbol type, bool isInterface)
    {
        if (!isInterface) // Interfaces ignore being marked for explicit mode...
        {
            if (type.HasAttribute(CloneableTypeSource.Name, out var data))
            {
                var arg = data.GetNamedArgument(CloneableTypeSource.ExplicitMode);
                if (arg != null &&
                    arg.Value.Value is bool value && value) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines if the type is marked to prevent adding the <see cref="ICloneable"/> interface
    /// to the list of interfaces it implements, or not.
    /// </summary>
    bool PreventAddICloneable => _PreventAddICloneable ??= GetPreventAddICloneable(TypeSymbol);
    bool? _PreventAddICloneable = null;
    static bool GetPreventAddICloneable(INamedTypeSymbol type)
    {
        if (type.HasAttribute(CloneableTypeSource.Name, out var data))
        {
            var arg = data.GetNamedArgument(CloneableTypeSource.PreventAddICloneable);
            if (arg != null &&
                arg.Value.Value is bool value && value) return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the symbol of the 'Clone()' method declared by this type, or null if any.
    /// </summary>
    IMethodSymbol? CloneMethod => _CloneMethod ??= GetCloneMethod();
    IMethodSymbol? _CloneMethod = null;
    IMethodSymbol? GetCloneMethod() => GetCloneMethod(TypeSymbol);
    static IMethodSymbol? GetCloneMethod(INamedTypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().SingleOrDefault(x =>
            x.Name == nameof(ICloneable.Clone) &&
            x.Parameters.Length == 0 &&
            !x.IsStatic && (
            x.DeclaredAccessibility == Accessibility.Public ||
            x.DeclaredAccessibility == Accessibility.Protected));
    }

    /// <summary>
    /// Determines if the type needs to decorate the generated 'Clone()' method with 'new',
    /// or not.
    /// </summary>
    bool NeedsNew => _NeedsNew ??= GetNeedsNew();
    bool? _NeedsNew = null;
    bool GetNeedsNew()
    {
        if (IsInterface &&
            !PreventAddICloneable) return true;

        return GetChainNeedsNew(TypeSymbol, IsInterface);
    }
    static bool GetChainNeedsNew(INamedTypeSymbol type, bool isInterface)
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
            x.HasAttribute(CloneableTypeSource.Name) ||
            GetCloneMethod(x) != null)
            .ToDebugArray();

        if (ifaces.Any()) return true;
        return false;
    }

    /// <summary>
    /// The collection of interfaces to implement. It is assumed this property is only used
    /// by concrete types.
    /// </summary>
    ImmutableArray<INamedTypeSymbol> InterfacesToImplement => _InterfacesToImplement ??= GetInterfacesToImplement();
    ImmutableArray<INamedTypeSymbol>? _InterfacesToImplement = null;
    ImmutableArray<INamedTypeSymbol> GetInterfacesToImplement()
    {
        var comp = SymbolEqualityComparer.Default;
        var list = new NoDuplicatesList<INamedTypeSymbol>(comp);

        if (!PreventAddICloneable) list.Add(ICloneableType);

        var ifaces = TypeSymbol.AllInterfaces.Where(x =>
            x.Name == nameof(ICloneable) ||
            x.HasAttribute(CloneableTypeSource.Name))
            .ToDebugArray();

        list.AddRange(ifaces);
        return list.ToImmutableArray();
    }
}