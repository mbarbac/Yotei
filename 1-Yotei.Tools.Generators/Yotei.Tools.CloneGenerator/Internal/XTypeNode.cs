namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (Symbol.IsRecord)
        {
            TreeDiagnostics.KindNotSupported(Symbol).Report(context);
            return false;
        }

        return base.Validate(context);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitAsInterface(context, cb);
        else if (Symbol.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    void EmitAsInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        // Gets the method modifiers, or null if any...
        string? GetModifiers()
        {
            var found = Symbol.AllInterfaces.Any(x =>
                FindMethod(x) != null ||
                FindCloneableAttribute(x) != null);

            return found ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract type.
    /// </summary>
    void EmitAsAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        EmitInterfaceItems(context, cb);

        // Gets the method modifiers, or null if any...
        string? GetModifiers()
        {
            if (Symbol.BaseType != null)
            {
                var method = FindMethod(Symbol.BaseType, chain: true); // Only chain...
                if (method != null)
                {
                    var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                    return isvirtual ? $"{access}abstract override " : $"{access}abstract ";
                }

                var at = FindCloneableAttribute(Symbol.BaseType, chain: true); // Only chain...
                if (at != null)
                {
                    return "public abstract override ";
                }
            }

            return "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete type.
    /// </summary>
    void EmitAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.GetCopyConstructor(strict: true);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return;
        }

        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

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

        EmitInterfaceItems(context, cb);

        // Gets the method modifiers, or null if any...
        string? GetModifiers()
        {
            var prevent = GetPreventVirtualValue(Symbol, out var value, true, true) && value;

            var host = Symbol.BaseType;
            if (host != null)
            {
                var method = FindMethod(host, chain: true); // Only chain...
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private)
                    {
                        return null;
                    }

                    var str = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                    return isvirtual switch
                    {
                        true => $"{str}override ",
                        false => $"{str}new ",
                    };
                }

                var at = FindCloneableAttribute(host, true, true); // Chain and ifaces...
                if (at != null)
                {
                    prevent = GetPreventVirtualValue(at, out value) && value;
                    return prevent ? "public new " : "public override ";
                }
            }

            var issealed = Symbol.IsSealed;
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the elements of the interfaces of the type need explicit implementation. This
    /// method shall not be used with interface types, because they do not need explicit
    /// implementation.
    /// </summary>
    void EmitInterfaceItems(SourceProductionContext context, CodeBuilder cb)
    {
        var ifaces = GetInterfaceItems();

        foreach (var iface in ifaces)
        {
            var typename = iface.EasyName(RoslynNameOptions.Full with { UseTypeNullable = false });
            var valuename = iface.Name == "ICloneable" ? "object" : typename;

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone() => ({typename})Clone();");
        }
    }

    /// <summary>
    /// Returns a list with the interfaces that the type needs to implement explicitly. This
    /// method shall not be used with interface types, because they do not need explicit
    /// implementation.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetInterfaceItems()
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
                FindCloneableAttribute(iface) != null;

            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            return found;
        }
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{AttributeDoc}}
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find an appropriate method in the given type, including its inheritance and
    /// interfaces chains if requested. Returns null if no method is found.
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

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the <see cref="CloneableAttribute"/> in the given type, including its
    /// inheritance and interfaces chains if requested. Returns null if no attribute is found.
    /// </summary>
    AttributeData? FindCloneableAttribute(
        ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var at = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();

        if (at == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((at = FindCloneableAttribute(temp)) != null) break;
        }

        if (at == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((at = FindCloneableAttribute(temp)) != null) break;
        }

        return at;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.PreventVirtual"/> setting
    /// from the given attribute data. Returns false if the setting is not found, or otherwise
    /// true and the actual value in the out argument.
    /// </summary>
    bool GetPreventVirtualValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.PreventVirtual), out var arg))
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
    /// Tries to get the value of the <see cref="CloneableAttribute.PreventVirtual"/> setting
    /// from the attributes applied to the given type, including its inheritance and interfaces
    /// chains if requested. Returns false if the setting is not found, or otherwise true and
    /// the actual value in the out argument.
    /// </summary>
    bool GetPreventVirtualValue(
        ITypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        value = false;

        var at = FindCloneableAttribute(type);
        if (at is not null)
        {
            if (GetPreventVirtualValue(at, out value)) return value;
        }

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetPreventVirtualValue(temp, out value)) return value;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetPreventVirtualValue(temp, out value)) return value;
        }

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.AddICloneable"/> setting
    /// from the given attribute data. Returns false if the setting is not found, or otherwise
    /// true and the actual value in the out argument.
    /// </summary>
    bool GetAddICloneableValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.AddICloneable), out var arg))
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
    /// Tries to get the value of the <see cref="CloneableAttribute.AddICloneable"/> setting
    /// from the attributes applied to the given type, including its inheritance and interfaces
    /// chains if requested. Returns false if the setting is not found, or otherwise true and
    /// the actual value in the out argument.
    /// </summary>
    bool GetAddICloneableValue(
        ITypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        value = false;

        var at = FindCloneableAttribute(type);
        if (at is not null)
        {
            if (GetAddICloneableValue(at, out value)) return value;
        }

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetAddICloneableValue(temp, out value)) return value;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetAddICloneableValue(temp, out value)) return value;
        }

        return false;
    }
}