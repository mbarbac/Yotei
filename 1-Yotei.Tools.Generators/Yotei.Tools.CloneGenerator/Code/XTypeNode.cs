namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;

        if (!context.TypeIsNotRecord(Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicit implementation...
        if (HasMethod(Symbol) != null) return;

        // Emitting code...
        PrintDocumentation(cb);

        var options = new EasyNameOptions(
            fullTypeName: true,
            typeParameters: true,
            nullableAnnotation: false);
        var typeName = Symbol.EasyName(options);
        var modifiers = GetModifiers();

        // Type is an interface...
        if (Symbol.IsInterface())
        {
            cb.AppendLine($"{modifiers}{typeName} Clone();");
            return;
        }

        // Type is abstract...
        if (Symbol.IsAbstract)
        {
            cb.AppendLine($"public abstract {typeName} Clone();");
        }

        // Regular...
        else
        {
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
                cb.AppendLine($"var v_temp = new {typeName}(this);");
                cb.AppendLine("return v_temp;");
            }

            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            typeName = iface.EasyName(options);
            var type = iface.Name == "ICloneable" ? "object" : typeName;

            cb.AppendLine();
            cb.AppendLine(type);
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
            var prevent = GetPreventVirtual(out var temp) && temp;
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
                    if (HasMethod(type) != null) return true;
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
            Comparer = SymbolEqualityComparer.Default.Equals,
            CanInclude = (@this, item) => @this.IndexOf(item) < 0,
        };

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Captures all suitable interfaces in the inheritance chain...
        bool Populate(ITypeSymbol iface)
        {
            var done = false;

            if (iface.HasAttributes(CloneableAttr.LongName)) done = true;
            else if (iface.Name == "ICloneable") done = true;
            else if (HasMethod(iface) != null) done = true;

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
    IMethodSymbol? HasMethod(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IMethodSymbol>()
        .FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the effective value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(out bool value)
    {
        if (CloneableAttr.GetPreventVirtual(Symbol, out value)) return true;

        foreach (var parent in Symbol.AllBaseTypes())
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        foreach (var parent in Symbol.AllInterfaces)
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        return false;
    }
}