namespace Yotei.Generators.MemberWith;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XProperty : Tree.PropertyNode
{
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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        var parentType = Parent.Name;
        var methodName = $"With{Name}";
        var itemType = Symbol.Type.FullyQualifiedName();
        var nullable = Symbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
        if (nullable) itemType += "?";

        cb.AppendLine($"/// <summary>");
        cb.AppendLine($"/// Returns a new instance where the original value of <see cref=\"{Name}\"/>");
        cb.AppendLine($"/// property is replaced by the new given one.");
        cb.AppendLine($"/// </summary>");
        cb.AppendLine($"/// <param name=\"value\"></param>");
        cb.AppendLine($"/// <returns></returns>");

        var newstr = NeedNew() ? "new " : string.Empty;

        // Interfaces...
        if (Parent.IsInterface)
        {
            cb.AppendLine($"{newstr}{parentType} {methodName}({itemType} value);");
        }

        // Implementation...
        else
        {
            cb.AppendLine($"public {newstr}{parentType} {methodName}({itemType} value)");
            cb.AppendLine("{");
            cb.Tabs++;

            var arg = new NewArgument(Symbol.Type, "value", Symbol.Name);
            var builder = new NewBuilder(Parent.Symbol);
            var done = builder.Print("item", cb, arg);
            if (done) cb.AppendLine("return item;");
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                    "Kerosene",
                    "Cannot generate a new instance.",
                    "Cannot generate a new instance of type '{0}' with arguments '{1}'.",
                    "Build",
                    DiagnosticSeverity.Error,
                    true),
                    Syntax.GetLocation(),
                    new object[] { Symbol.Name, arg.ArgumentName }));
            }

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
    /// Determines if the method needs a "new" modifier, or not.
    /// </summary>
    /// <returns></returns>
    bool NeedNew()
    {
        var parent = Parent.Symbol.BaseType;
        if (parent != null && NeedNew(parent)) return true;

        foreach (var iface in Parent.Symbol.Interfaces) if (NeedNew(iface)) return true;
        return false;
    }
    bool NeedNew(INamedTypeSymbol type)
    {
        var members = type.GetMembers().OfType<IPropertySymbol>().ToDebugArray();
        var member = members.SingleOrDefault(x => x.Name == Name);
        if (member != null)
        {
            var done = member.HasAttribute(MemberWithSource.AttributeLongName);
            if (done) return true;
        }

        var ifaces = type.Interfaces;
        foreach (var iface in ifaces) if (NeedNew(iface)) return true;

        var parent = type.BaseType;
        if (parent != null && NeedNew(parent)) return true;

        return false;
    }

    /// <summary>
    /// Gets the collection of interfaces to implement, when it is invoked for a non-interface
    /// type.
    /// </summary>
    /// <returns></returns>
    IEnumerable<INamedTypeSymbol> InterfacesToImplement()
    {
        if (Parent.IsInterface) return Array.Empty<INamedTypeSymbol>();

        var comp = SymbolEqualityComparer.Default;
        var list = new NoDuplicatesList<INamedTypeSymbol>(comp);

        var ifaces = Parent.Symbol.Interfaces;
        foreach (var iface in ifaces) Populate(list, iface, Name);
        return list;

        static void Populate(
            NoDuplicatesList<INamedTypeSymbol> list, INamedTypeSymbol type, string name)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.Name == name &&
                x.HasAttribute(MemberWithSource.AttributeLongName))
                .ToArray();

            if (members.Length > 0) list.Add(type);

            var ifaces = type.Interfaces;
            foreach (var iface in ifaces) Populate(list, iface, name);
        }
    }
}