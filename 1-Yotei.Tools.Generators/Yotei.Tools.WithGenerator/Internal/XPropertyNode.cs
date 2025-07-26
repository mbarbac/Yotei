namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol ReturnType = default!;
    internal bool IsInherited = false;

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Other validations...
        if (Host.IsRecord)
        {
            TreeDiagnostics.RecordsNotSupported(Host).Report(context);
            return false;
        }
        if (!ValidateSpecific(context)) return false;

        // Finishing...
        return true;
    }

    /// <summary>
    /// Invoked to validate specific conditions of the member.
    /// </summary>
    bool ValidateSpecific(SourceProductionContext context)
    {
        if (Symbol.IsIndexer)
        {
            TreeDiagnostics.IndexerNotSupported(Symbol).Report(context);
            return false;
        }
        if (!Symbol.HasGetter())
        {
            TreeDiagnostics.NoGetter(Symbol).Report(context);
            return false;
        }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit())
        {
            TreeDiagnostics.NoSetter(Symbol).Report(context);
            return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        if (Host.Name == "AType00") { } // DEBUG

        if (!CaptureReturnType(out ReturnType))
        {
            TreeDiagnostics.InvalidReturnType(Symbol).Report(context);
            return;
        }

        // Declared or implemented explicitly...
        if (FindWithMethod(Host) != null) return;

        // Dispatching...
        if (Host.IsInterface()) EmitForInterface(context, cb);
        else if (Host.IsAbstract) EmitForAbstract(context, cb);
        else EmitForConcrete(context, cb);
    }

    /// <summary>
    /// Validates and captures the return type to use with the generated methods.
    /// </summary>
    bool CaptureReturnType(out INamedTypeSymbol type)
    {
        // Interfaces are valid per-se...
        if (Host.IsInterface()) { type = Host; return true; }

        // If no return interface requested, use host type...
        var attr = FindWithAttribute(Host, chain: true, allifaces: true);
        if (attr == null) { type = Host; return true; }

        var found = GetReturnInterfaceValue(attr, out var value);
        if (!found || !value) { type = Host; return true; }

        // Validating no base type with a concrete return type...
        var host = Host;
        while ((host = host.BaseType) != null)
        {
            var method = FindWithMethod(host);
            if (method != null && !method.ReturnType.IsInterface()) { type = null!; return false; }

            attr = FindWithAttribute(host);
            if (attr == null) continue;

            found = GetReturnInterfaceValue(attr, out value);
            if (!found || !value) { type = null!; return false; } // We need returning iface...
        }

        // Finding a suitable first-level interface...
        foreach (var iface in Host.Interfaces)
        {
            found = iface.Recursive(iface =>
            {
                var method = FindWithMethod(iface); if (method != null) return true;
                var attr = FindWithAttribute(iface); if (attr != null) return true;
                return false;
            },
            allifaces: true);

            if (found) { type = iface; return true; }
        }

        // Default is using the host type...
        type = Host;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    void EmitForInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typename = Host.EasyName();
        var membername = Symbol.Type.EasyName(RoslynNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typename}");
        cb.AppendLine($"{MethodName}({membername} {ArgumentName});");

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any. Logic is:
        /// if any parent interface defines a method, return 'new', otherwise null.
        /// </summary>
        string? GetModifiers()
        {
            var found = false;            
            if (IsInherited) found = true;// If inherited this is easy...
            found |= Host.AllInterfaces.Any(x => FindWithMethod(x) != null);
            found |= Host.AllInterfaces.Any(x => FindDecoratedMember(x) != null);

            return found ? "new " : null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract class.
    /// </summary>
    void EmitForAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;

        var modifiers = GetModifiers();
        var retname = ReturnType.EasyName(options);
        var membername = Symbol.Type.EasyName(RoslynNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname}");
        cb.AppendLine($"{MethodName}({membername} {ArgumentName});");

        EmitExplicitInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            // When the host is a derived one...
            var host = Host.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindWithMethod(host, chain: true);
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
                var at = FindWithAttribute(host, chain: true);
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
        var ctor = Host.GetCopyConstructor(strict: false);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Host).Report(context);
            return;
        }

        // Emitting...
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;

        var modifiers = GetModifiers();
        var typename = Host.EasyName();
        var retname = ReturnType.EasyName(options);
        var membername = Symbol.Type.EasyName(RoslynNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{retname}");
        cb.AppendLine($"{MethodName}({membername} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var xtemp = "x_temp";
            cb.AppendLine($"var {xtemp} = new {typename}(this)");
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

        EmitExplicitInterfaces(context, cb);

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var issealed = Symbol.IsSealed;
            var prevent = GetPreventVirtualValue(Host, out var temp, true, false, true) && temp;

            // When the host is a derived one...
            var host = Host.BaseType;
            if (host != null && host.Name != "Object")
            {
                // If there is a base method...
                var method = FindWithMethod(host, chain: true);
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
                var at = FindWithAttribute(host, chain: true);
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
    void EmitExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        var comparer = SymbolComparer.Default;
        var ifaces = GetExplicitInterfaces();

        foreach (var iface in ifaces)
        {
            if (comparer.Equals(iface, ReturnType)) continue;

            var member = FindDecoratedMember(iface, allifaces: true);
            if (member == null)
            {
                TreeDiagnostics.SymbolNotFound(iface).Report(context);
                return;
            }

            var typename = iface.EasyName(RoslynNameOptions.Full with { UseTypeNullable = false });
            var membername = member.Type.EasyName(RoslynNameOptions.Full);

            cb.AppendLine();
            cb.AppendLine($"{typename}");
            cb.Append($"{typename}.{MethodName}({membername} value)");
            cb.AppendLine($" => {MethodName}(value);");
        }
    }

    /// <summary>
    /// Gets a list with the interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Host.Interfaces) TryCapture(iface);
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
                FindMember(iface) != null ||
                FindDecoratedMember(iface) != null;

            // Adding to the list if needed, and finishing...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }
            return found;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member with the name of this instance in the given type, or in its chain
    /// of base types and direct or all implemented interfaces if such is explicitly requested.
    /// By default, the member's type just need to be a compatible one. In strict mode, it must
    /// be the be the same or a derived one. Returns null if not found, or the found member
    /// otherwise.
    /// </summary>
    IPropertySymbol? FindMember(
        ITypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name &&
                x.Parameters.Length == 0 &&
                (strict
                    ? x.Type.IsAssignableTo(Symbol.Type)
                    : Symbol.Type.IsAssignableTo(x.Type)));
        },
        chain, ifaces, allifaces);
    }

    /// <summary>
    /// Tries to find a decorated member with the name of this instance in the given type, or in
    /// its chain of base types and direct or all implemented interfaces if such is explicitly
    /// requested. By default, the member's type just need to be a compatible one. In strict
    /// mode, it must be the be the same or a derived one. Returns null if not found, or the
    /// found member otherwise.
    /// </summary>
    IPropertySymbol? FindDecoratedMember(
        ITypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name &&
                x.Parameters.Length == 0 &&
                x.HasAttributes(typeof(WithAttribute)) &&
                (strict
                    ? x.Type.IsAssignableTo(Symbol.Type)
                    : Symbol.Type.IsAssignableTo(x.Type)));
        },
        chain, ifaces, allifaces);
    }

    /// <summary>
    /// Tries to find a 'With' method with the name of this instance, in the given type, or in
    /// its chain of base types and direct or all implemented interfaces if such is explicitly
    /// requested. By default, the unique method parameter's type just need to be a compatible
    /// one. In strict mode, it must be the be the same or a derived one. Returns null if not
    /// found, or the found method otherwise.
    /// </summary>
    IMethodSymbol? FindWithMethod(
        ITypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == MethodName &&
                x.Parameters.Length == 1 &&
                (strict
                    ? x.Parameters[0].Type.IsAssignableTo(Symbol.Type)
                    : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the <see cref="WithAttribute"/> attribute on an appropriate member of the
    /// given type. The chain of base types and direct or all implemented interfaces are also
    /// used if such is explicitly requested.
    /// </summary>
    AttributeData? FindWithAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            var member = FindDecoratedMember(type);
            if (member == null) return null;

            return member.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.ReturnInterface"/> property on
    /// the given attribute. If found, returns <c>true</c> and sets the value of the out argument
    /// to the value found. If not, returns <c>false</c>.
    /// </summary>
    bool GetReturnInterfaceValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(WithAttribute.ReturnInterface), out var arg))
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
    /// Tries to find the value of the <see cref="WithAttribute.ReturnInterface"/> property on
    /// the given type, or in its chain of base types and direct or all implemented interfaces if
    /// such is explicitly requested. If found, returns <c>true</c> and sets the value of the out
    /// argument to the value found. If not, returns <c>false</c>.
    /// </summary>
    bool GetReturnInterfaceValue(
        ITypeSymbol type, out bool value,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.RecursiveBool(out value, (ITypeSymbol type, out bool value) =>
        {
            var at = FindWithAttribute(type);
            if (at != null && GetReturnInterfaceValue(at, out value)) return true;

            value = false;
            return false;
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.PreventVirtual"/> property on the
    /// given attribute. If found, returns <c>true</c> and sets the value of the out argument to
    /// the value found. If not, returns <c>false</c>.
    /// </summary>
    bool GetPreventVirtualValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(WithAttribute.PreventVirtual), out var arg))
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
    /// Tries to find the value of the <see cref="WithAttribute.PreventVirtual"/> property on the
    /// given type, or in its chain of base types and direct or all implemented interfaces if such
    /// is explicitly requested. If found, returns <c>true</c> and sets the value of the out
    /// argument to the value found. If not, returns <c>false</c>.
    /// </summary>
    bool GetPreventVirtualValue(
        ITypeSymbol type, out bool value,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.RecursiveBool(out value, (ITypeSymbol type, out bool value) =>
        {
            var at = FindWithAttribute(type);
            if (at != null && GetPreventVirtualValue(at, out value)) return true;

            value = false;
            return false;
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the 'with' keyword for the '{{Symbol.Name}}' member.
        /// </summary>
        {{AttributeDoc}}
        """);
}