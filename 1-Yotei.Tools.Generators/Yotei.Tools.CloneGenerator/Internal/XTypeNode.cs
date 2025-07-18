namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol ReturnType = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Other validations...
        if (Symbol.IsRecord)
        {
            TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
            return false;
        }

        // Finishing...
        CaptureReturnType();
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var add = GetAddICloneable(Symbol, out var temp, chain: true, ifaces: true) && temp;

        if (add) head += " : ICloneable";
        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindCloneMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitAsInterface(context, cb);
        else if (Symbol.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    void EmitAsInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        // Gets the method modifiers, with a space separator, or null if any.
        string? GetModifiers()
        {
            if (Symbol.Name == "IB") { } // DEBUG

            var found = false;
            if (!found) found = Symbol.AllInterfaces.Any(x => x.Name == "ICloneable");
            if (!found) found = GetAddICloneable(Symbol, out var value, true, true) && value;

            if (!found)
            {
                foreach (var iface in Symbol.Interfaces)
                {
                    if (!found) found = FindCloneMethod(iface, true, true) != null;
                    if (!found) found = FindCloneableAttribute(iface, true, true) != null;
                    if (found) break;
                }
            }

            return found ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract one.
    /// </summary>
    void EmitAsAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var retname = ReturnType.EasyName();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname} Clone();");

        EmitExplicitInterfaces(context, cb);

        // Gets the method modifiers, with a space separator, or null if any.
        string? GetModifiers()
        {
            // When symbol is a derived one...
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindCloneMethod(host, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;

                    var str = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual ? $"{str}abstract override " : $"{str}abstract ";
                }

                // If the method is being implemented...
                var at = FindCloneableAttribute(host, chain: true);
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
    void EmitAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        // We need a copy constructor...
        var ctor = Symbol.GetCopyConstructor(strict: false);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return;
        }

        // Emitting...
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName();
        var retname = ReturnType.EasyName();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_temp = new {typename}(this);");
            cb.AppendLine("return v_temp;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        EmitExplicitInterfaces(context, cb);

        // Gets the method modifiers, with a space separator, or null if any.
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var prevent = GetPreventVirtual(Symbol, out var temp, true, true) && temp;

            // When symbol is a derived one...
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindCloneMethod(host, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;

                    var str = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;

                    return isvirtual ? $"{str}override " : $"{str}new ";
                }

                // If the method is being implemented...
                var at = FindCloneableAttribute(host, chain: true);
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
    /// Invoked to emit the explicit interfaces' implementations associated to this type.
    /// </summary>
    void EmitExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var comparer = SymbolComparer.Default;
        var ifaces = GetExplicitInterfaces();

        foreach (var iface in ifaces)
        {
            if (comparer.Equals(iface, ReturnType)) continue;

            var typename = iface.EasyName(RoslynNameOptions.Full with { UseTypeNullable = false });
            var valuename = iface.Name == "ICloneable" ? "object" : typename;

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone() => ({typename})Clone();");
        }
    }

    /// <summary>
    /// Gets a list with the interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        TryICloneable();
        return list;

        // Tries to capture the given interface...
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First, its childs...
            foreach (var child in iface.Interfaces)
            {
                var temp = TryCapture(child);
                if (temp) found = true;
            }

            // Then the interface itself...
            found = found ||
                iface.Name == "ICloneable" ||
                FindCloneMethod(iface) != null ||
                FindCloneableAttribute(iface) != null;

            // Adding to the list if needed, and finishing...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            return found;
        }

        // Tries to add the 'ICloneable' interface, if needed...
        void TryICloneable()
        {
            var add = GetAddICloneable(Symbol, out var value, true, true) && value;
            if (add)
            {
                var comp = GetBranchCompilation();
                var item = comp.GetTypeByMetadataName("System.ICloneable");

                if (item != null)
                {
                    var temp = list.Find(x => comparer.Equals(x, item));
                    if (temp == null) list.Add(item);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the type to use as the return type for the generated methods.
    /// <br/> If the host type is an interface, the return type is always that interface.
    /// <br/> If the attribute does not use the <see cref="CloneableAttribute.ReturnInterface"/>,
    /// the return type is the host one, either concrete or abstract.
    /// <br/> If that property is <c>true</c>, then the return type is the first interface in the
    /// collection of directly implemented ones that is cloneable-alike. If none is found, then
    /// the return type is the host type itself.
    /// </summary>
    void CaptureReturnType()
    {
        // If host is interface, use that host type...
        if (!Symbol.IsInterface())
        {
            // If use interface is requested...
            var value = GetReturnInterface(Symbol, out var temp) && temp;
            if (value)
            {
                foreach (var iface in Symbol.Interfaces)
                {
                    // ICloneable does not qualify...
                    if (IsCloneAlike(iface) && iface.Name != "ICloneable")
                    { ReturnType = iface; return; }
                }
            }
        }

        // By default, the host type itself is returned...
        ReturnType = Symbol;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the 'Clone()' method declared or implemented by the given type. The base types
    /// and interfaces are also searched if explicitly requested. Returns <c>null</c> if not
    /// found.
    /// </summary>
    static IMethodSymbol? FindCloneMethod(
        INamedTypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        if (item != null) return item;

        if (chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindCloneMethod(child);
                if (item != null) return item;
            }
        }

        if (ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindCloneMethod(child);
                if (item != null) return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Determines if the given type is a 'Clone'-alike one, either because it implements or
    /// declared a 'Clone' method, or because it is decorated with the <see cref="ICloneable"/>
    /// attribute. The base types and interfaces are also searched if explicitly requested.
    /// </summary>
    static bool IsCloneAlike(INamedTypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var method = FindCloneMethod(type);
        if (method != null) return true;

        var at = FindCloneableAttribute(type);
        if (at != null) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes()) if (IsCloneAlike(child)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces) if (IsCloneAlike(child)) return true;

        return false;
    }

    /// <summary>
    /// Tries to find and return the <see cref="CloneableAttribute"/> attribute that decorates
    /// the given type, or null if any. The base types and interfaces are also searched if such
    /// is explicitly requested.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    static AttributeData? FindCloneableAttribute(
        INamedTypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();
        if (item != null) return item;

        if (chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindCloneableAttribute(child);
                if (item != null) return item;
            }
        }

        if (ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindCloneableAttribute(child);
                if (item != null) return item;
            }
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.ReturnInterface"/> named
    /// argument from the given attribute. If found among the used arguments, returns true and
    /// its actual value in the out argument. Otherwise, returns false.
    /// </summary>
    static bool GetReturnInterface(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(CloneableAttribute.ReturnInterface), out var arg))
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
    /// Tries to get the value of the <see cref="CloneableAttribute.ReturnInterface"/> named
    /// argument from the attribute that decorates the given type, if any. If found among the
    /// used arguments, returns true and its actual value in the out argument. Otherwise,
    /// returns false.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    static bool GetReturnInterface(
        INamedTypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, chain, ifaces);
        if (at != null && GetReturnInterface(at, out value)) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes())
                if (GetReturnInterface(child, out value)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces)
                if (GetReturnInterface(child, out value)) return true;

        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.PreventVirtual"/> named
    /// argument from the given attribute. If found among the used arguments, returns true and
    /// its actual value in the out argument. Otherwise, returns false.
    /// </summary>
    static bool GetPreventVirtual(AttributeData at, out bool value)
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
    /// Tries to get the value of the <see cref="CloneableAttribute.PreventVirtual"/> named
    /// argument from the attribute that decorates the given type, if any. If found among the
    /// used arguments, returns true and its actual value in the out argument. Otherwise,
    /// returns false.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    static bool GetPreventVirtual(
        INamedTypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, chain, ifaces);
        if (at != null && GetPreventVirtual(at, out value)) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes())
                if (GetPreventVirtual(child, out value)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces)
                if (GetPreventVirtual(child, out value)) return true;

        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="CloneableAttribute.AddICloneable"/> named
    /// argument from the given attribute. If found among the used arguments, returns true and
    /// its actual value in the out argument. Otherwise, returns false.
    /// </summary>
    static bool GetAddICloneable(AttributeData at, out bool value)
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
    /// Tries to get the value of the <see cref="CloneableAttribute.AddICloneable"/> named
    /// argument from the attribute that decorates the given type, if any. If found among the
    /// used arguments, returns true and its actual value in the out argument. Otherwise,
    /// returns false.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    static bool GetAddICloneable(
        INamedTypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var at = FindCloneableAttribute(type, chain, ifaces);
        if (at != null && GetAddICloneable(at, out value)) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes())
                if (GetAddICloneable(child, out value)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces)
                if (GetAddICloneable(child, out value)) return true;

        value = false;
        return false;
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
}