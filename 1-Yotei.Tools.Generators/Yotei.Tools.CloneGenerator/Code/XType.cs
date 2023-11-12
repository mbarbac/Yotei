namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XType : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    public XType(TypeCandidate candidate) : base(candidate) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XType(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!context.TypeIsNotRecord(Symbol)) return false;

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
        if (HasMethod(Symbol) != null) return; // Intercepting explicit implementation...

        PrintDocumentation(file);
        var modifiers = GetModifiers();
        if (modifiers != null) modifiers += " ";

        if (Symbol.IsInterface())
        {
            PrintInterface();
            return;
        }

        PrintRegular();

        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var name = iface.FullyQualifiedName(addNullable: false);
            var type = iface.Name == "ICloneable" ? "object" : name;

            file.AppendLine();
            file.AppendLine(type);
            file.AppendLine($"{name}.Clone() => Clone();");
        }

        /// <summary>
        /// Invoked to emit code when the host is an interface...
        /// </summary>
        void PrintInterface()
        {
            file.AppendLine($"{modifiers}{Symbol.Name} Clone();");
        }

        /// <summary>
        /// Invoked to emit code when the specs are the default ones...
        /// </summary>
        void PrintRegular()
        {
            if (Symbol.IsAbstract)
            {
                file.AppendLine($"public abstract {Symbol.Name} Clone();");
            }
            else
            {
                file.AppendLine($"{modifiers}{Symbol.Name} Clone()");
                file.AppendLine("{");
                file.IndentLevel++;

                var method = Symbol.GetCopyConstructor(true) ?? Symbol.GetCopyConstructor(false);
                if (method == null)
                {
                    file.AppendLine("throw new NotImplementedException();");
                    context.NoCopyConstructor(Symbol);
                }
                else
                {
                    file.AppendLine($"var v_temp = new {Symbol.Name}(this);");
                    file.AppendLine("return v_temp;");
                }

                file.IndentLevel--;
                file.AppendLine("}");
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the method documentation.
    /// </summary>
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
    string? GetModifiers()
    {
        // Interfaces...
        if (Symbol.IsInterface())
        {
            return Symbol.AllInterfaces.Any(x =>
                x.Name == "ICloneable" ||
                x.HasAttributes(CloneableAttr.LongName))
                ? "new"
                : null;
        }

        // Implementation...
        else
        {
            var prevent = TryGetPreventVirtual(out var temp) && temp;
            var appears = AppearsInChain(Symbol, true);
            if (appears)
            {
                return !prevent ? "public override" : "public new";
            }
            else
            {
                if (Symbol.IsSealed) return "public";
                return !prevent ? "public virtual" : "public";
            }
        }

        // Determines if appears in the chain...
        bool AppearsInChain(ITypeSymbol type, bool top)
        {
            if (!top)
            {
                if (HasMethod(type) != null) return true;
                if (type.HasAttributes(CloneableAttr.LongName)) return true;
            }
            var parent = type.BaseType;
            return parent == null ? false : AppearsInChain(parent, false);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the interfaces that need implementation.
    /// </summary>
    ITypeSymbol[] GetInterfacesToImplement()
    {
        var list = new CoreList<ITypeSymbol>()
        {
            AcceptDuplicate = (item) => false,
            Compare = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Needs to capture all suitable interfaces in the chain...
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
    /// Determines if the type implements a compatible method, or not.
    /// </summary>
    IMethodSymbol? HasMethod(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttr.PreventVirtual"/> setting.
    /// </summary>
    bool TryGetPreventVirtual(out bool value)
    {
        if (CloneableAttr.GetPreventVirtual(Symbol, out value)) return true;

        foreach (var type in Symbol.AllBaseTypes())
            if (CloneableAttr.GetPreventVirtual(type, out value)) return true;

        foreach (var type in Symbol.AllInterfaces)
            if (CloneableAttr.GetPreventVirtual(type, out value)) return true;

        return false;
    }
}