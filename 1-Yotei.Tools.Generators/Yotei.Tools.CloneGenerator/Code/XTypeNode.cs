namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    public XTypeNode(Node parent, TypeCandidate candidate) : base(parent, candidate) { }
    public XTypeNode(Node parent, ITypeSymbol symbol) : base(parent, symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;
        if (!Symbol.ValidateNotRecord(context)) return false;
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
        // Intercepting explicit elements...
        if (HasMethod(Symbol) != null) return;

        // Initiating...
        var valueName = $"v_{Symbol.Name}";
        var modifiers = GetModifiers(); if (modifiers != null) modifiers += " ";
        PrintDocumentation(cb);

        // Interfaces...
        if (IsInterface)
        {
            cb.AppendLine($"{modifiers}{Symbol.Name} Clone();");
            return;
        }

        // Abstract...
        if (Symbol.IsAbstract)
        {
            cb.AppendLine($"public abstract {Symbol.Name} Clone();");
        }

        // Regular...
        else
        {
            var builder = new TypeBuilder(context, Symbol);
            var underscores = ObtainIncludeUnderscores(out var utemp, out _) && utemp;
            var specs = ObtainSpecs(out var stemp, out _) ? stemp : null;
            var receiver = "v_temp";
            var code = builder.GetCode(receiver, specs, null, underscores);

            cb.AppendLine($"{modifiers}{Symbol.Name} Clone()");
            cb.AppendLine("{");
            cb.IndentLevel++;

            if (code == null)
            {
                cb.AppendLine("throw new NotImplementedException();");
            }
            else
            {
                cb.Append(code);
            }

            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var name = iface.FullyQualifiedName(addNullable: false);
            var type = iface.Name == "ICloneable" ? "object" : name;

            cb.AppendLine();
            cb.AppendLine(type);
            cb.AppendLine($"{name}.Clone() => Clone();");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the appropriate documentation.
    /// </summary>
    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        /// <returns></returns>
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the appropriate modifiers, or null if any.
    /// </summary>
    /// <returns></returns>
    string? GetModifiers()
    {
        // Interfaces...
        if (IsInterface)
        {
            return Symbol.AllInterfaces.Any(x =>
                x.Name == "ICloneable" ||
                x.HasAttributes(CloneableAttr.LongName))
                ? "new"
                : string.Empty;
        }

        // Implementation...
        else
        {
            var prevent = ObtainPreventVirtual(out var temp, out _) && temp;
            var times = GetChainTimes(Symbol, true);
            if (times)
            {
                return !prevent ? "public override" : "public new";
            }

            if (Symbol.IsSealed) return "public";
            return !prevent ? "public virtual" : "public";
        }

        // Computes the number of times in the chain...
        bool GetChainTimes(ITypeSymbol type, bool top)
        {
            if (!top)
            {
                var method = HasMethod(type); if (method != null) return true;
                if (type.HasAttributes(CloneableAttr.LongName)) return true;
            }
            var parent = type.BaseType;
            return parent == null ? false : GetChainTimes(parent, false);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked, for implementation purposes, to get a list with the interfaces to implement.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetInterfacesToImplement()
    {
        var list = new CoreList<ITypeSymbol>()
        {
            AcceptDuplicate = (item) => false,
            Compare = SymbolEqualityComparer.Default.Equals,
        };
        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToList();

        // Recursively traverse the inheritance chain to treat all interfaces...
        bool Populate(ITypeSymbol iface)
        {
            var done = false;

            if (iface.Name == "ICloneable") done = true;
            if (HasMethod(iface) != null) done = true;
            if (iface.HasAttributes(CloneableAttr.LongName)) done = true;

            foreach (var child in iface.Interfaces) if (Populate(child)) done = true;

            if (done) list.Add(iface);
            return done;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type has a compatible 'With' member, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? HasMethod(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttr.Specs"/> setting, provided the
    /// symbol is decorated appropriately.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inherited"></param>
    /// <returns></returns>
    bool ObtainSpecs(out string? value, out bool inherited)
    {
        inherited = false;
        if (CloneableAttr.GetSpecs(Symbol, out value)) return true;

        inherited = false;
        foreach (var type in Symbol.AllBaseTypes()) if (CloneableAttr.GetSpecs(type, out value)) return true;
        foreach (var type in Symbol.AllInterfaces) if (CloneableAttr.GetSpecs(type, out value)) return true;

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttr.IncludeUnderscores"/> setting,
    /// provided the symbol is decorated appropriately.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inherited"></param>
    /// <returns></returns>
    bool ObtainIncludeUnderscores(out bool value, out bool inherited)
    {
        inherited = false;
        if (CloneableAttr.GetIncludeUnderscores(Symbol, out value)) return true;

        inherited = false;
        foreach (var type in Symbol.AllBaseTypes()) if (CloneableAttr.GetIncludeUnderscores(type, out value)) return true;
        foreach (var type in Symbol.AllInterfaces) if (CloneableAttr.GetIncludeUnderscores(type, out value)) return true;

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttr.PreventVirtual"/> setting,
    /// provided the symbol is decorated appropriately.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inherited"></param>
    /// <returns></returns>
    bool ObtainPreventVirtual(out bool value, out bool inherited)
    {
        inherited = false;
        if (CloneableAttr.GetPreventVirtual(Symbol, out value)) return true;

        inherited = false;
        foreach (var type in Symbol.AllBaseTypes()) if (CloneableAttr.GetPreventVirtual(type, out value)) return true;
        foreach (var type in Symbol.AllInterfaces) if (CloneableAttr.GetPreventVirtual(type, out value)) return true;

        return false;
    }
}