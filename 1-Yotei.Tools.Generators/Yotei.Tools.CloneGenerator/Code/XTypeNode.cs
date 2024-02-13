namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;

        if (!context.TypeIsNotRecord(Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicit implementation...
        if (HasMethod(Symbol, recursive: false)) return;

        // Emitting code...
        PrintDocumentation(cb);
        if (Symbol.IsInterface()) PrintInterface(context, cb);
        else if (Symbol.IsAbstract) PrintAbstract(context, cb);
        else PrintRegular(context, cb);
    }

    /// <summary>
    /// Case: type is an interface...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typeName = Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));

        cb.AppendLine("[Yotei.Tools.CloneGenerator.YoteiGenerated]");
        cb.AppendLine($"{modifiers}{typeName} Clone();");
    }

    /// <summary>
    /// Case: type is abstract...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var typeName = Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));

        cb.AppendLine("[Yotei.Tools.CloneGenerator.YoteiGenerated]");
        cb.AppendLine($"public abstract {typeName} Clone();");
        PrintNeededInterfaces(context, cb);
    }

    /// <summary>
    /// Case: standard case...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typeName = Symbol.EasyName(new EasyNameOptions(
            typeFullName: false,
            typeGenerics: true,
            typeNullable: false));

        cb.AppendLine("[Yotei.Tools.CloneGenerator.YoteiGenerated]");
        cb.AppendLine($"{modifiers}{typeName} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var method = Symbol.GetCopyConstructor(true) ?? Symbol.GetCopyConstructor(false);
        if (method == null)
        {
            cb.AppendLine("throw new NotImplementedException();");
            context.NoCopyConstructor(Symbol);
        }
        else
        {
            cb.AppendLine($"var v_temp = new {typeName}(this);"); // Copy constructor...
            cb.AppendLine("return v_temp;");
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
        PrintNeededInterfaces(context, cb);
    }

    /// <summary>
    /// Emits interfaces that need implementation...
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void PrintNeededInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var typeName = iface.EasyName(new EasyNameOptions(
                typeFullName: true,
                typeGenerics: true,
                typeNullable: false));

            var valueName = iface.Name == "ICloneable" ? "object" : typeName;

            cb.AppendLine();
            cb.AppendLine(valueName);
            cb.AppendLine($"{typeName}.Clone() => Clone();");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="cb"></param>
    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate method modifiers, or null if any.
    /// </summary>
    /// <returns></returns>
    string? GetModifiers()
    {
        // Interfaces...
        if (Symbol.IsInterface())
        {
            return Symbol.AllInterfaces.Any(x =>
                x.Name == "ICloneable" ||
                x.HasAttributes(CloneableAttr.LongName))
                ? "new "
                : null;
        }

        // Implementation...
        else
        {
            var prevent = GetPreventVirtual(Symbol, out var temp) && temp;
            var appears = AppearsInChain(Symbol, true);
            if (appears)
            {
                return prevent ? "public new " : "public override ";
            }
            else
            {
                return Symbol.IsSealed || prevent
                    ? "public "
                    : "public virtual ";
            }

            // Determines if appears in chain...
            bool AppearsInChain(ITypeSymbol type, bool top)
            {
                if (!top)
                {
                    if (HasMethod(type, recursive: false)) return true;
                    if (type.HasAttributes(CloneableAttr.LongName)) return true;
                }
                var parent = type.BaseType;
                return parent != null && AppearsInChain(parent, false);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of interfaces that need implementation.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    ITypeSymbol[] GetInterfacesToImplement()
    {
        var list = new CustomList<ITypeSymbol>()
        {
            Compare = SymbolEqualityComparer.Default.Equals,
            CanInclude = (item, x) => !SymbolEqualityComparer.Default.Equals(item, x),
        };

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Captures all suitable interfaces in the inheritance chain...
        bool Populate(ITypeSymbol iface)
        {
            var done = false;

            if (iface.HasAttributes(CloneableAttr.LongName)) done = true;
            else if (iface.Name == "ICloneable") done = true;
            else if (HasMethod(iface, recursive: false)) done = true;

            foreach (var child in iface.Interfaces) if (Populate(child)) done = true;

            if (done) list.Add(iface);
            return done;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type implements a compatible method, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? GetTypeMethod(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IMethodSymbol>()
        .FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

    bool HasMethod(ITypeSymbol type, bool recursive)
    {
        if (GetTypeMethod(type) != null) return true;
        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return HasMethod(parent, recursive);
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the effective value of the <see cref="WithGeneratorAttr.PreventVirtual"/>
    /// setting, starting at the given type and up through its inheritance chain.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(ITypeSymbol type, out bool value)
    {
        if (CloneableAttr.GetPreventVirtual(type, out value)) return true;

        foreach (var parent in type.AllBaseTypes())
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        foreach (var parent in type.AllInterfaces)
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        return false;
    }
}