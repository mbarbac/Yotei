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

        /// <summary>
        /// Gets the appropriate method modifiers, or null if any...
        /// </summary>
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

        /// <summary>
        /// Gets the appropriate method modifiers, or null if any...
        /// </summary>
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

        /// <summary>
        /// Gets the appropriate method modifiers, or null if any...
        /// </summary>
        string? GetModifiers()
        {
            var host = Symbol.BaseType;
            if (host == null) return "public ";

            var method = FindMethod(host, chain: true);
            if (method == null)
            {
                var attr = FindAttribute(host, chain: true, ifaces: true);
                if (attr != null) return "public override ";

                var prevent = GetPreventVirtual(Symbol, out var temp) && temp;
                return prevent || Symbol.IsSealed ? "public " : "public virtual ";
            }

            else
            {
                var access = method.DeclaredAccessibility;
                var accstr = access.ToCSharpString(addspace: true);
                var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                return isvirtual switch
                {
                    true => $"{accstr}override ",
                    false => $"{accstr}new ",
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
            var found = false;

            foreach (var child in iface.Interfaces) // If any child,  iface needs implementation...
            {
                var temp = Populate(child);
                if (temp) found = true;
            }

            // This iface...
            found = found ||
                iface.Name == "ICloneable" ||
                FindMethod(iface) != null ||
                FindAttribute(iface) != null;

            if (found) // Adding if needed...
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }
            return found; // Informs if found at this iface level...
        }
    }

    // -----------------------------------------------------

    string GeneratedVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string GeneratedAttribute => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{GeneratedVersion}}")]
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
        var member = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

        if (member == null && chain)
            foreach (var item in type.AllBaseTypes())
                if ((member = FindMethod(item, false, false)) != null) break;

        if (member == null && ifaces)
            foreach (var item in type.AllInterfaces)
                if ((member = FindMethod(item, false, false)) != null) break;

        return member;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the <see cref="CloneableAttribute"/> attribute on the given type, or in its
    /// inheritance and interface chains if requested.
    /// </summary>
    AttributeData? FindAttribute(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var attrs = type.GetAttributes(typeof(CloneableAttribute));
        var attr = attrs.Count == 1 ? attrs[0] : null;

        if (attr == null && chain)
            foreach (var item in type.AllBaseTypes())
                if ((attr = FindAttribute(item, false, false)) != null) break;

        if (attr == null && ifaces)
            foreach (var item in type.AllInterfaces)
                if ((attr = FindAttribute(item, false, false)) != null) break;

        return attr;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Determines if the <see cref="CloneableAttribute.PreventVirtual"/> setting is enabled,
    /// in the given instance or in its inheritance and interface chains if requested.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    bool GetPreventVirtual(ITypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var attrs = type.GetAttributes(typeof(CloneableAttribute));
        foreach (var attr in attrs)
        {
            if (attr.GetNamedArgument(nameof(CloneableAttribute.PreventVirtual), out var arg) &&
                !arg.Value.IsNull &&
                arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        if (chain)
            foreach (var item in type.AllBaseTypes())
                if (GetPreventVirtual(item, out value, false, false)) return true;

        if (ifaces)
            foreach (var item in type.AllInterfaces)
                if (GetPreventVirtual(item, out value, false, false)) return true;

        value = false;
        return false;
    }
}