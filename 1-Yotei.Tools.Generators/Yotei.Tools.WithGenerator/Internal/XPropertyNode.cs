#pragma warning disable IDE0075

namespace Yotei.Tools.WithGenerator;

// =========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // -----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (Host.IsRecord)
        {
            context.ReportDiagnostic(WithDiagnostics.RecordsNotSupported(Host));
            return false;
        }
        if (Symbol.IsIndexer)
        {
            context.ReportDiagnostic(WithDiagnostics.IndexerNotSupported(Symbol));
            return false;
        }
        if (!Symbol.HasGetter())
        {
            context.ReportDiagnostic(TreeDiagnostics.NoGetter(Symbol));
            return false;
        }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit())
        {
            context.ReportDiagnostic(TreeDiagnostics.NoSetter(Symbol));
            return false;
        }
        return base.Validate(context);
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        // Implemented explicitly...
        if (FindMethod(Host) != null) return;

        // Dispatching...
        if (Host.IsInterface()) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostConcrete(context, cb);
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = Host.EasyName(EasyNameOptions.Default);
        var memberType = Symbol.Type.EasyName(EasyNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");

        /// <summary>
        /// Gets the method modifiers, or null if any...
        /// </summary>
        string? GetModifiers()
        {
            foreach (var iface in Host.AllInterfaces)
            {
                var member = FindDecoratedMember(iface);
                if (member != null) return "new ";

                var method = FindMethod(iface);
                if (method != null) return "new ";
            }

            return null;
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = Host.EasyName(EasyNameOptions.Default);
        var memberType = Symbol.Type.EasyName(EasyNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");

        EmitNeededInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers, or null if any...
        /// </summary>
        string? GetModifiers()
        {
            if (Host.BaseType == null) return "public abstract ";

            var method = FindMethod(Host.BaseType, chain: true);
            if (method != null)
            {
                var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                return $"{access}abstract override ";
            }
            else
            {
                return "public abstract ";
            }
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the symbol is a concrete type.
    /// </summary>
    void EmitHostConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Host.GetCopyConstructor();
        if (ctor == null)
        {
            context.ReportDiagnostic(TreeDiagnostics.NoCopyConstructor(Host));
            return;
        }

        var modifiers = GetModifiers();
        var parentType = Host.EasyName(EasyNameOptions.Default);
        var memberType = Symbol.Type.EasyName(EasyNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var xtemp = "x_temp";
            cb.AppendLine($"var {xtemp} = new {parentType}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{Symbol.Name} = {ArgumentName}");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
            cb.AppendLine($"return {xtemp};");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitNeededInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers, or null if any...
        /// </summary>
        string? GetModifiers()
        {
            var prevent = GetPreventVirtual(Host, out var value, true, true) && value;
            var host = Host.BaseType;
            
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
                        var accstr = access.ToCSharpString(addspace: true);
                        var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                        return isvirtual switch
                        {
                            true => $"{accstr}override ",
                            false => $"{accstr}new ",
                        };
                    }
                }

                while (host != null)
                {
                    var member = FindMember(host);
                    if (member != null)
                    {
                        var atr = FindAttribute(host, chain: true, ifaces: true);
                        if (atr != null)
                        {
                            prevent = GetPreventVirtual(atr, out value) && value;
                            return prevent ? "public new " : "public override ";
                        }
                    }

                    host = host.BaseType;
                }
            }

            var issealed = ParentNode.Symbol.IsSealed;
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // -----------------------------------------------------

    /// <summary>
    /// Emits the interfaces that need explicit implementation.
    /// </summary>
    void EmitNeededInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        var ifaces = GetNeededInterfaces();
        foreach (var iface in ifaces)
        {
            var member = FindMember(iface);
            if (member == null)
            {
                foreach (var temp in iface.AllInterfaces)
                {
                    member = FindMember(temp);
                    if (member != null) break;
                }
                if (member == null)
                {
                    context.ReportDiagnostic(WithDiagnostics.MemberNotFoundInInterface(Symbol, iface));
                    return;
                }
            }

            var parentType = iface.EasyName(EasyNameOptions.Full with { UseTypeNullable = false });
            var memberType = member.Type.EasyName(EasyNameOptions.Full);

            cb.AppendLine();
            cb.AppendLine($"{parentType}");
            cb.Append($"{parentType}.{MethodName}({memberType} value)");
            cb.AppendLine($" => {MethodName}(value);");
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

        foreach (var iface in Host.Interfaces) Capture(iface);
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
                FindMethod(iface) != null ||
                FindDecoratedMember(iface) != null;

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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{GeneratedVersion}}")]
        """;

    /// <summary>
    /// Emits the documentation.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the host type where the value of the decorated member has been
        /// replaced by the new given one.
        /// </summary>
        /// <param name ="{{ArgumentName}}"></param>
        {{GeneratedAttribute}}
        """);

    // -----------------------------------------------------

    /// <summary>
    /// Finds a compatible method.
    /// </summary>
    IMethodSymbol? FindMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

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

    /// <summary>
    /// Finds a compatible member.
    /// </summary>
    IPropertySymbol? FindMember(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.Parameters.Length == 0 &&
            Symbol.Type.IsAssignableTo(x.Type));

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((item = FindMember(temp)) != null) break;
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((item = FindMember(temp)) != null) break;
        }

        return item;
    }

    /// <summary>
    /// Finds a compatible decorated member.
    /// </summary>
    IPropertySymbol? FindDecoratedMember(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.Parameters.Length == 0 &&
            Symbol.Type.IsAssignableTo(x.Type) &&
            x.HasAttributes(typeof(WithAttribute)));

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((item = FindDecoratedMember(temp)) != null) break;
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((item = FindDecoratedMember(temp)) != null) break;
        }

        return item;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the effective <see cref="WithAttribute"/> or <see cref="InheritWithsAttribute"/>.
    /// </summary>
    AttributeData? FindAttribute(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var member = FindMember(type);
        var item = member?.GetAttributes(typeof(WithAttribute)).FirstOrDefault();

        item ??= type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((item = FindAttribute(temp)) != null) break;
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((item = FindAttribute(temp)) != null) break;
        }

        return item;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Finds the value of <see cref="WithAttribute.PreventVirtual"/> or of the
    /// <see cref="InheritWithsAttribute.PreventVirtual"/>.
    /// </summary>
    bool GetPreventVirtual(AttributeData attr, out bool value)
    {
        if (attr.GetNamedArgument(nameof(WithAttribute.PreventVirtual), out var arg))
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
    /// Finds the effective value of <see cref="WithAttribute.PreventVirtual"/> or of
    /// <see cref="InheritWithsAttribute.PreventVirtual"/>.
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
}