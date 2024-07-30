#pragma warning disable IDE0075

namespace Yotei.Tools.CloneGenerator;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // -----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (Symbol.IsRecord)
        {
            context.ReportDiagnostic(TreeDiagnostics.KindNotSupported(Symbol));
        }
        return base.Validate(context);
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
        /// Gets the method modifiers, or null if any...
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
        /// Gets the method modifiers, or null if any...
        /// </summary>
        string? GetModifiers()
        {
            if (Symbol.BaseType == null) return "public abstract ";

            var method = FindMethod(Symbol.BaseType, chain: true);
            if (method != null)
            {
                var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                return isvirtual ? $"{access}abstract override " : $"{access}abstract ";
            }

            var atr = FindAttribute(Symbol.BaseType, chain: true);
            if (atr != null)
            {
                return "public abstract override ";
            }

            return "public abstract ";
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
        /// Gets the method modifiers, or null if any...
        /// </summary>
        string? GetModifiers()
        {
            var prevent = GetPreventVirtual(Symbol, out var value, true, true) && value;
            var host = Symbol.BaseType;

            if (host != null)
            {
                var method = FindMethod(host, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private)
                    {
                        return null;
                    }
                    else
                    {
                        var str = access.ToCSharpString(addspace: true);
                        var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                        return isvirtual switch
                        {
                            true => $"{str}override ",
                            false => $"{str}new ",
                        };
                    }
                }

                var atr = FindAttribute(host, chain: true, ifaces: true);
                if (atr != null)
                {
                    prevent = GetPreventVirtual(atr, out value) && value;
                    return prevent ? "public new " : "public override ";
                }
            }

            var issealed = Symbol.IsSealed;
            return prevent || issealed ? "public " : "public virtual ";
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

        foreach (var iface in Symbol.Interfaces) Capture(iface);
        return list;

        // Tries to capture the given interface...
        bool Capture(ITypeSymbol iface)
        {
            var found = false;

            foreach (var child in iface.Interfaces)
            {
                var temp = Capture(child);
                if (temp) found = true;
            }

            found = found ||
                iface.Name == "ICloneable" ||
                FindMethod(iface) != null ||
                FindAttribute(iface) != null;

            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            return found;
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
    /// Finds a compatible method.
    /// </summary>
    IMethodSymbol? FindMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((item = FindMethod(temp)) != null) break;
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((item = FindMethod(temp)) != null) break;
        }

        return item;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the value of <see cref="CloneableAttribute.PreventVirtual"/>.
    /// </summary>
    bool GetPreventVirtual(AttributeData attr, out bool value)
    {
        if (attr.GetNamedArgument(nameof(CloneableAttribute.PreventVirtual), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = false;
        return false;
    }

    /// <summary>
    /// Finds the effective value of <see cref="CloneableAttribute.PreventVirtual"/>.
    /// </summary>
    bool GetPreventVirtual(ITypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        value = false;

        var attr = FindAttribute(type);
        var found = attr != null ? GetPreventVirtual(attr, out value) : false;
        if (found) return true;

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                found = GetPreventVirtual(temp, out value);
                if (found) return true;
            }
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                found = GetPreventVirtual(temp, out value);
                if (found) return true;
            }
        }

        return false;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the effective <see cref="CloneableAttribute"/>.
    /// </summary>
    AttributeData? FindAttribute(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var atr = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();

        if (atr == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((atr = FindAttribute(temp)) != null) break;
        }

        if (atr == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((atr = FindAttribute(temp)) != null) break;
        }

        return atr;
    }
}