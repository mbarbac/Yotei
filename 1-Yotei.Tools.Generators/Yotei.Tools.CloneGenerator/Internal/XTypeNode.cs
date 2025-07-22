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
        if (!CaptureReturnType())
        {
            TreeDiagnostics.InvalidReturnType(Symbol).Report(context);
            return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var add = GetAddICloneableValue(Symbol, out var temp) && temp;

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
        if (Symbol.IsInterface()) EmitForInterface(context, cb);
        else if (Symbol.IsAbstract) EmitForAbstract(context, cb);
        else EmitForConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    void EmitForInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Symbol.EasyName();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename} Clone();");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any. The logic is:
        /// if any base interface defines a 'Clone' method, then we return 'new'. Otherwise, we
        /// don't need any modifier.
        /// </summary>
        string? GetModifiers()
        {
            var found = false;
            if (!found) found = Symbol.AllInterfaces.Any(x => x.Name == "ICloneable");
            if (!found) found = GetAddICloneableValue(Symbol, out var value, true, false, true) && value;

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
    /// Invoked when the type is an abstract class.
    /// </summary>
    void EmitForAbstract(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;
        var retname = ReturnType.EasyName(options);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname} Clone();");

        EmitExplicitInterfaces(cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            // When the host is a derived one...
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindCloneMethod(host, chain: true);
                if (method != null)
                {
                    // Reusing base method accessibility...
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    // May need to re-asbtract...
                    var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                    return isvirtual ? $"{str}abstract override " : $"{str}abstract ";
                }

                // Else, method might have been requested, and we always re-abstract...
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
    /// Invoked when the type is a concrete class.
    /// </summary>
    void EmitForConcrete(SourceProductionContext context, CodeBuilder cb)
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
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;
        var retname = ReturnType.EasyName(options);

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

        EmitExplicitInterfaces(cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var prevent = GetPreventVirtualValue(Symbol, out var temp, true, false, true) && temp;

            // When the host is a derived one...
            var host = Symbol.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindCloneMethod(host, chain: true);
                if (method != null)
                {
                    // Reusing base method accessibility...
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null;
                    var str = access.ToCSharpString(addspace: true);

                    if (prevent) return $"{str}override sealed ";
                    else
                    {
                        var isvirtual = method.IsVirtual || method.IsOverride | method.IsAbstract;
                        return isvirtual ? $"{str}override " : $"{str}new ";
                    }
                }

                // Else, method might have been requested...
                var at = FindCloneableAttribute(host, chain: true);
                if (at != null)
                {
                    return host.IsAbstract || !prevent ? "public override " : "public new ";
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
    void EmitExplicitInterfaces(CodeBuilder cb)
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
        TryAddICloneable();
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            // First, the childs...
            var found = false;
            foreach (var child in iface.Interfaces)
            {
                var temp = TryCapture(child);
                if (temp) found = true;
            }

            // Then, the interface itself...
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

        /// <summary>
        /// Tries to add the <see cref="ICloneable"/> interface, if needed.
        /// </summary>
        void TryAddICloneable()
        {
            var add = GetAddICloneableValue(Symbol, out var value) && value;
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
    /// Captures the type to use as the return type of the generated method. Returns <c>false</c>
    /// if the return type cannot be captured because returning an interface is requested and a
    /// similar method in a base type returns a concrete type. Otherwise returns <c>true</c> and
    /// sets the out argument to that return type.
    /// </summary>
    bool CaptureReturnType()
    {
        // If host is an interface, use the host type itself...
        if (Symbol.IsInterface())
        {
            ReturnType = Symbol;
            return true;
        }

        // If no ReturnInterface is requested, also use the host type itself...
        var value = GetReturnInterfaceValue(Symbol, out var temp) && temp;
        if (!value)
        {
            ReturnType = Symbol;
            return true;
        }

        // Cannot override with an interface a base method with a concrete return type...
        var host = Symbol;
        while ((host = host.BaseType) != null)
        {
            // Either because it has such method...
            var method = FindCloneMethod(host);
            if (method != null && method.ReturnType.IsInterface()) return false;

            // Or because such method is requested...
            var attr = FindCloneableAttribute(host);
            if (attr == null) continue;

            var found = GetReturnInterfaceValue(attr, out value);
            if (!found) continue;
            if (!value) return false;
        }

        // Finding a suitable direct interface, if any...
        foreach (var iface in Symbol.Interfaces)
        {
            if (iface.Name == "ICloneable") continue; // ICloneable does not qualify...
            if (IsCloneable(iface, false, false, true))
            {
                ReturnType = iface;
                return true;
            }
        }

        // Validated...
        ReturnType = Symbol;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a valid 'Clone()' method in the given type, or in its chain of base types
    /// and direct or all implemented interfaces if such is explicitly requested. Returns null
    /// if not found, or the found method otherwise.
    /// </summary>
    static IMethodSymbol? FindCloneMethod(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);
        },
        chain, ifaces, allifaces);
    }

    /// <summary>
    /// Determines if the given type is a 'Cloneable'-alike one, either because it implements a
    /// 'Clone' method, or because it is decorated with the <see cref="CloneableAttribute"/>
    /// attribute. The chains of its base types and direct or all implemented interfaces are also
    /// used if such is explicitly requested.
    /// </summary>
    static bool IsCloneable(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            var method = FindCloneMethod(type); if (method != null) return true;
            var attr = FindCloneableAttribute(type); if (attr != null) return true;
            return false;
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the <see cref="CloneableAttribute"/> attribute on the given type. The chain
    /// of base types and direct or all implemented interfaces are also used if such is explicitly
    /// requested.
    /// </summary>
    static AttributeData? FindCloneableAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.ReturnInterface"/> property on
    /// the given attribute. If found, returns <c>true</c> and sets the value of the out argument
    /// to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetReturnInterfaceValue(AttributeData at, out bool value)
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
    /// Tries to find the value of the <see cref="CloneableAttribute.ReturnInterface"/> property on
    /// the given type, or in its chain of base types and direct or all implemented interfaces if
    /// such is explicitly requested. If found, returns <c>true</c> and sets the value of the out
    /// argument to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetReturnInterfaceValue(
        ITypeSymbol type, out bool value,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.RecursiveBool(out value, (ITypeSymbol type, out bool value) =>
        {
            var at = FindCloneableAttribute(type);
            if (at != null && GetReturnInterfaceValue(at, out value)) return true;

            value = false;
            return false;
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.AddICloneable"/> property on
    /// the given attribute. If found, returns <c>true</c> and sets the value of the out argument
    /// to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetAddICloneableValue(AttributeData at, out bool value)
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
    /// Tries to find the value of the <see cref="CloneableAttribute.AddICloneable"/> property on
    /// the given type, or in its chain of base types and direct or all implemented interfaces if
    /// such is explicitly requested. If found, returns <c>true</c> and sets the value of the out
    /// argument to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetAddICloneableValue(
        ITypeSymbol type, out bool value,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.RecursiveBool(out value, (ITypeSymbol type, out bool value) =>
        {
            var at = FindCloneableAttribute(type);
            if (at != null && GetAddICloneableValue(at, out value)) return true;

            value = false;
            return false;
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.PreventVirtual"/> property on
    /// the given attribute. If found, returns <c>true</c> and sets the value of the out argument
    /// to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetPreventVirtualValue(AttributeData at, out bool value)
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
    /// Tries to find the value of the <see cref="CloneableAttribute.PreventVirtual"/> property on
    /// the given type, or in its chain of base types and direct or all implemented interfaces if
    /// such is explicitly requested. If found, returns <c>true</c> and sets the value of the out
    /// argument to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetPreventVirtualValue(
        ITypeSymbol type, out bool value,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.RecursiveBool(out value, (ITypeSymbol type, out bool value) =>
        {
            var at = FindCloneableAttribute(type);
            if (at != null && GetPreventVirtualValue(at, out value)) return true;

            value = false;
            return false;
        },
        chain, ifaces, allifaces);
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