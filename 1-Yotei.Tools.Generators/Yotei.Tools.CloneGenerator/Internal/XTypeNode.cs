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
        var r = true;

        if (Symbol.IsRecord)
        {
            TreeDiagnostics.KindNotSupported(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var add = GetAddICloneableValue(Symbol, out var temp, chain: true, ifaces: true) && temp;

        if (add) head += " : ICloneable";
        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented exlicitly...
        if (FindMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitCoreAsInterface(context, cb);
        else if (Symbol.IsAbstract) EmitCoreAsAbstract(context, cb);
        else EmitCoreAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    protected void EmitCoreAsInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        /// <summary>
        /// Gets the method modifiers or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var found = false;

            if (!found) found = Symbol.AllInterfaces.Any(x => x.Name == "ICloneable");
            if (!found) found = GetAddICloneableValue(Symbol, out var value, ifaces: true) && value;
            if (!found) found = FindMethod(Symbol, top: false, ifaces: true) != null;
            if (!found) found = FindCloneableAttribute(Symbol, top: false, ifaces: true) != null;

            return found ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitCoreAsAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName(RoslynNameOptions.Default);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        EmitCoreExplicitInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers or null if any.
        /// </summary>
        string? GetModifiers()
        {
            // Case when symbol is a derived one...
            if (Symbol.BaseType != null && Symbol.BaseType.Name != "Object")
            {
                // There might be already a base method...
                var method = FindMethod(Symbol, top: false, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual ? $"{access}abstract override " : $"{access}abstract ";
                }

                // Or being implemented...
                var at = FindCloneableAttribute(Symbol, top: false, chain: true, ifaces: true);
                if (at != null)
                {
                    return "public abstract override ";
                }
            }

            // Default for abstract types...
            return "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitCoreAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        // We need a copy constructor...
        var ctor = Symbol.GetCopyConstructor(strict: true);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return;
        }

        // Emitting...
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

        EmitCoreExplicitInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var prevent = GetPreventVirtualValue(Symbol, out var temp, true, true, true) && temp;

            // Case when symbol is a derived one...
            if (Symbol.BaseType != null && Symbol.BaseType.Name != "Object")
            {
                // There might be already a base method...
                var method = FindMethod(Symbol, top: false, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;

                    var str = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual switch
                    {
                        true => $"{str}override ",
                        false => $"{str}new ",
                    };
                }

                // Or being implemented...
                var at = FindCloneableAttribute(Symbol, top: false, chain: true, ifaces: true);
                if (at != null)
                {
                    return prevent ? "public new " : "public override ";
                }
            }

            // Default for concrete types...
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the explicit method implementations for the inherited methods. Note that
    /// this method only makes sense for abstract and concrete types, but not for interface ones.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    protected void EmitCoreExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var ifaces = GetExplicitInterfaces();

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
    /// Returns a list with the interface types that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) Capture(iface);
        TryICloneable();
        return list;

        // Tries to capture the given interface...
        bool Capture(ITypeSymbol iface)
        {
            var found = false;

            // First, its child interfaces...
            foreach (var child in iface.Interfaces)
            {
                var temp = Capture(child);
                if (temp) found = true;
            }

            // And then, this one...
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

        // ICloneable requested as a special case...
        void TryICloneable()
        {
            var add = GetAddICloneableValue(Symbol, out var value, chain: true, ifaces: true) && value;
            if (add)
            {
                var comp = GetBranchCompilation();
                var item = comp.GetTypeByMetadataName("System.ICloneable");
                if (item != null) list.Add(item);
            }
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
    /// Tries to find the generator's method in the given type, including its base types and
    /// interfaces if requested. By default, the search starts from the given type, but if the
    /// <paramref name="top"/> flag is set to <c>false</c>, then only its base types and interfaces
    /// are considered.
    /// <br/> Returns <c>null</c> if not found.
    /// </summary>
    IMethodSymbol? FindMethod(
        ITypeSymbol type,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var item = !top
            ? null
            : type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0);

        if (item == null && chain) // Inheritance chain requested...
        {
            foreach (var temp in type.AllBaseTypes())
                if ((item = FindMethod(temp)) != null) break;
        }

        if (item == null && ifaces) // Interfaces requested...
        {
            foreach (var temp in type.AllInterfaces)
                if ((item = FindMethod(temp)) != null) break;
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the <see cref="CloneableAttribute"/> in the given type, including its
    /// inheritance and interfaces chains, if requested. By default, the search starts from the
    /// given type, but if the <paramref name="top"/> flag is set to <c>false</c>, then only its
    /// base types and interfaces are considered.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="top"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    AttributeData? FindCloneableAttribute(
        ITypeSymbol type,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var at = top
            ? type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault()
            : null;

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

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.PreventVirtual"/> property
    /// from the given attribute. Returns <c>false</c> if the setting is not found, or otherwise
    /// <c>true</c> and the setting's value in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.PreventVirtual"/> property
    /// in the given type, including  inheritance and interfaces chains, if requested. By default,
    /// the search starts from the given type, but if the <paramref name="top"/> flag is set to
    /// <c>false</c>, then only its base types and interfaces are considered.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="top"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    bool GetPreventVirtualValue(
        ITypeSymbol type,
        out bool value,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, top);

        if (at != null)
        {
            if (GetPreventVirtualValue(at, out value)) return true;
        }

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetPreventVirtualValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetPreventVirtualValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.AddICloneable"/> property
    /// from the given attribute. Returns <c>false</c> if the setting is not found, or otherwise
    /// <c>true</c> and the setting's value in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.AddICloneable"/> property
    /// in the given type, including  inheritance and interfaces chains, if requested. By default,
    /// the search starts from the given type, but if the <paramref name="top"/> flag is set to
    /// <c>false</c>, then only its base types and interfaces are considered.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="top"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    bool GetAddICloneableValue(
        ITypeSymbol type,
        out bool value,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, top);

        if (at != null)
        {
            if (GetAddICloneableValue(at, out value)) return true;
        }

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetAddICloneableValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetAddICloneableValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }
}
