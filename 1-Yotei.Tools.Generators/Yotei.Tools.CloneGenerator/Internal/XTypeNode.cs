namespace Yotei.Tools.CloneGenerator;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override bool IsSupportedKind()
    {
        if (!base.IsSupportedKind()) return false;

        if (Symbol.IsRecord) return false;
        return true;
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Implemented explicitly...
        if (FindMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(EasyNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        // Gets the appropriate method modifiers, or null if any...
        string? GetModifiers()
        {
            var found = Symbol.AllInterfaces.Any(x =>
                FindMethod(x) != null ||
                FindAttribute(x) != null);

            return found ? "new " : null;
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(EasyNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        EmitNeededInterfaces(context, cb);

        // Gets the appropriate method modifiers, or null if any...
        string? GetModifiers()
        {
            var parent = Symbol.BaseType;
            var method = parent == null ? null : FindMethod(parent, chain: true, ifaces: true);
            var attr = parent == null ? null : FindAttribute(parent, chain: true, ifaces: true);

            if (method == null)
            {
                return attr != null ? "public abstract override " : "public abstract ";
            }
            else
            {
                var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;
                return isvirtual ? "public abstract override " : "public abstract ";
            }
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a concrete type.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.GetCopyConstructor();
        if (ctor == null)
        {
            context.ReportDiagnostic(TreeDiagnostics.NoCopyConstructor(Symbol));
            return;
        }

        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(EasyNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_temp = new {typename}(this);");
            cb.AppendLine("return v_temp;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitNeededInterfaces(context, cb);

        // Gets the appropriate method modifiers, or null if any...
        string? GetModifiers()
        {
            var parent = Symbol.BaseType;
            var method = parent == null ? null : FindMethod(parent, chain: true, ifaces: true);
            var attr = parent == null ? null : FindAttribute(parent, chain: true, ifaces: true);

            if (method == null)
            {
                if (attr != null) return "public override ";
                return Symbol.IsSealed ? "public " : "public virtual ";
            }
            else
            {
                var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;
                return Symbol.IsSealed switch
                {
                    true => isvirtual ? "public protected override " : "public override ",
                    false => isvirtual ? "public override " : "public new "
                };
            }
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Emits the interfaces that need explicit implementation.
    /// </summary>
    void EmitNeededInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var ifaces = GetNeededInterfaces();
        foreach (var iface in ifaces)
        {
            var typename = iface.EasyName(EasyNameOptions.Full with { UseTypeNullable = false });
            var valuename = iface.Name == "ICloneable" ? "object" : typename;

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone() => ({typename})Clone();");
        }
    }

    /// <summary>
    /// Returns a list with the interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetNeededInterfaces()
    {
        var comparer = SymbolComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list;

        // Tries to populate the list with the given interface...
        bool Populate(ITypeSymbol iface)
        {
            // If any child, this iface also needs implementation...
            var found = false; foreach (var child in iface.Interfaces)
            {
                var temp = Populate(child);
                if (temp) found = true;
            }

            // This iface...
            if (!found) found = FindAt(iface);

            // Adding if needed...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            // Informs if found at this iface level...
            return found;
        }

        // Determines if the given interface needs implementation, at its level only...
        bool FindAt(ITypeSymbol iface) =>
            iface.Name == "ICloneable" ||
            FindMethod(iface) != null ||
            FindAttribute(iface) != null;
    }

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(Tools.CloneGenerator)}}", "{{GeneratedVersion}}")]
        """;

    /// <summary>
    /// Emits the documentation.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{GeneratedAttribute}}
        """);

    // -----------------------------------------------------

    /// <summary>
    /// Finds a compatible method on the given type, or in its inheritance and interface chains
    /// if such is requested.
    /// </summary>
    IMethodSymbol? FindMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var temp = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

        if (temp == null && chain)
            foreach (var item in type.AllBaseTypes())
                if ((temp = FindMethod(item, false, false)) != null) break;

        if (temp == null && ifaces)
            foreach (var item in type.AllInterfaces)
                if ((temp = FindMethod(item, false, false)) != null) break;

        return temp;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the attribute on the given type, or in its inheritance and interface chains if such
    /// is requested.
    /// </summary>
    AttributeData? FindAttribute(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var temps = type.GetAttributes(typeof(CloneableAttribute));
        var temp = temps.Count == 1 ? temps[0] : null;

        if (temps == null && chain)
            foreach (var item in type.AllBaseTypes())
                if ((temp = FindAttribute(item, false, false)) != null) break;

        if (temps == null && ifaces)
            foreach (var item in type.AllInterfaces)
                if ((temp = FindAttribute(item, false, false)) != null) break;

        return temp;
    }
}