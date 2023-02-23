namespace Yotei.Generators.MemberWith;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XProperty : Tree.PropertyNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public XProperty(
        Tree.ITypeNode parent,
        PropertyDeclarationSyntax syntax, IPropertySymbol symbol, SemanticModel model)
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
        if (Symbol.GetMethod == null)
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                "Kerosene",
                "Property has not a get accessor.",
                "The property '{0}' has not a get accessor.",
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        var parentType = Parent.Name;
        var methodName = $"With{Name}";
        var itemType = Symbol.Type.FullyQualifiedName();
        var itemNullable = Symbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
        if (itemNullable) itemType += "?";

        cb.AppendLine($"/// <summary>");
        cb.AppendLine($"/// Returns a new instance where the original value of the <see cref=\"{Name}\"/>");
        cb.AppendLine($"/// property is replaced by the new given one.");
        cb.AppendLine($"/// </summary>");
        cb.AppendLine($"/// <param name=\"value\"></param>");
        cb.AppendLine($"/// <returns></returns>");

        // For declaration purposes...
        if (Parent.IsInterface)
        {
            cb.AppendLine($"{parentType} {methodName}({itemType} value);");
        }

        // For implementation purposes...
        else
        {
            cb.AppendLine($"public {parentType} {methodName}({itemType} value)");
            cb.AppendLine("{");
            cb.Tabs++;

            var builder = new NewItemBuilder(Parent.Syntax, Parent.Symbol);
            var arg = new NewItemArgument("value", Name, Symbol.Type);
            var done = builder.Print("item", context, cb, arg);
            if (done) cb.AppendLine($"return item;");

            cb.Tabs--;
            cb.AppendLine("}");

            foreach (var iface in InterfacesToImplement())
            {
                parentType = iface.FullyQualifiedName();

                cb.AppendLine();
                cb.AppendLine(parentType);
                cb.AppendLine($"{parentType}.{methodName}({itemType} value) => {methodName}(value);");
            }
        }
    }

    /// <summary>
    /// Gets the interfaces to implement. Assumes it is invoked from a non-interface type.
    /// </summary>
    IEnumerable<INamedTypeSymbol> InterfacesToImplement()
    {
        var comp = SymbolEqualityComparer.Default;
        var list = new NoDuplicatesList<INamedTypeSymbol>(comp);

        var ifaces = Parent.Symbol.Interfaces;
        foreach (var iface in ifaces) Populate(list, iface);
        return list;

        void Populate(NoDuplicatesList<INamedTypeSymbol> list, INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.Name == Name &&
                x.HasAttribute(MemberWithSource.AttributeLongName))
                .ToArray();

            if (members.Length > 0) list.Add(type);

            var ifaces = type.Interfaces;
            foreach (var iface in ifaces) Populate(list, iface);
        }
    }
}