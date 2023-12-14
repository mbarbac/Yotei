namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    public XTypeNode(TypeCandidate candidate) : base(candidate) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!Symbol.TypeIsNotRecord(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        // Intercepting explicit implementation...
        if (HasMethod(Symbol) != null) return;

        // Interface...
        if (Symbol.IsInterface())
        {
            var name = Symbol.GivenName(addNullable: false);
            var modifiers = GetModifiers();

            PrintDocumentation(file);
            file.AppendLine($"{modifiers}{name} Clone();");
            return;
        }

        // Abstract...
        if (Symbol.IsAbstract)
        {
            var name = Symbol.GivenName(addNullable: false);
            var modifiers = "public abstract ";

            PrintDocumentation(file);
            file.AppendLine($"{modifiers}{name} Clone();");
        }

        // Regular...
        else
        {
            var name = Symbol.GivenName(addNullable: false);
            var modifiers = GetModifiers();

            PrintDocumentation(file);
            file.AppendLine($"{modifiers}{name} Clone()");
            file.AppendLine("{");
            file.IndentLevel++;

            var method = Symbol.GetCopyConstructor(true) ?? Symbol.GetCopyConstructor(false);
            if (method == null)
            {
                file.AppendLine("throw new NotImplementedException();");
                Symbol.NoCopyConstructor(context);
            }
            else
            {
                file.AppendLine($"var v_temp = new {name}(this);");
                file.AppendLine("return v_temp;");
            }

            file.IndentLevel--;
            file.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var name = iface.FullyQualifiedName(addNullable: false);
            var type = iface.Name == "ICloneable" ? "object" : name;

            file.AppendLine();
            file.AppendLine(type);
            file.AppendLine($"{name}.Clone() => Clone();");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="file"></param>
    void PrintDocumentation(FileBuilder file) => file.AppendLine($$"""
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
            AcceptDuplicate = (x, y) => false,
            Compare = SymbolEqualityComparer.Default.Equals,
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
    /// <returns></returns>
    IMethodSymbol? HasMethod(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IMethodSymbol>()
        .FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting.
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