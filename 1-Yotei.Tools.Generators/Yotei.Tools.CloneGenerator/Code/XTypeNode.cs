namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
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
    /// <param name="builder"></param>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder builder)
    {
        // Intercepting explicit implementation...
        if (HasMethod(Symbol) != null) return;

        // Common variables...
        var name = Symbol.GivenName(addNullable: false);
        PrintDocumentation(builder);

        // Interface...
        if (Symbol.IsInterface())
        {
            var modifiers = GetModifiers();

            builder.AppendLine($"{modifiers}{name} Clone();");
            return;
        }

        // Abstract...
        if (Symbol.IsAbstract)
        {
            var modifiers = "public abstract ";

            builder.AppendLine($"{modifiers}{name} Clone();");
        }

        // Regular...
        else
        {
            var modifiers = GetModifiers();

            builder.AppendLine($"{modifiers}{name} Clone()");
            builder.AppendLine("{");
            builder.IndentLevel++;

            var method = Symbol.GetCopyConstructor(true) ?? Symbol.GetCopyConstructor(false);
            if (method == null)
            {
                builder.AppendLine("throw new NotImplementedException();");
                context.NoCopyConstructor(Symbol);
            }
            else
            {
                builder.AppendLine($"var v_temp = new {name}(this);");
                builder.AppendLine("return v_temp;");
            }

            builder.IndentLevel--;
            builder.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            name = iface.FullyQualifiedName(addNullable: false);
            var type = iface.Name == "ICloneable" ? "object" : name;

            builder.AppendLine();
            builder.AppendLine(type);
            builder.AppendLine($"{name}.Clone() => Clone();");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="builder"></param>
    void PrintDocumentation(CodeBuilder builder) => builder.AppendLine($$"""
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
            var prevent = TryGetPreventVirtual(out var temp) && temp;
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
        var list = new CustomList<ITypeSymbol>
        {
            OnAcceptDuplicate = (x, y) => false,
            OnCompare = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Need to capture all suitable interfaces to implement in the inheritance chain...
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
    /// Tries to get the value of the <see cref="CloneableAttr.PreventVirtual"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetPreventVirtual(out bool value)
    {
        if (CloneableAttr.GetPreventVirtual(Symbol, out value)) return true;

        foreach (var parent in Symbol.AllBaseTypes())
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        foreach (var parent in Symbol.AllInterfaces)
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        return false;
    }
}