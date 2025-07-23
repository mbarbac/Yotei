#pragma warning disable IDE0075

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }


    INamedTypeSymbol ReturnType = default!;
    bool IsInherited => Candidate is null;
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
        if (!ValidateReturnType())
        {
            TreeDiagnostics.InvalidReturnType(Symbol).Report(context);
            return false;
        }
        if (!ValidateSpecific(context)) return false;

        // Finishing...
        ReturnType = CaptureReturnType();
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

    /// <summary>
    /// Validates that, if using an alternate return type is requested, such is valid because
    /// there is no base type returning a concrete one. If either the host type is itself an
    /// interface, or using an interface return type was not requested, validation is passed.
    /// </summary>
    bool ValidateReturnType()
    {
        if (Host.IsInterface()) return true; // // Interfaces are valid per-se...

        var attr = FindWithAttribute(Host); // No member with attribute, weird...
        if (attr == null) return true;

        var found = GetReturnInterfaceValue(attr, out var value); // No alternate requested...
        if (found && !value) return true;

        var host = Host;
        while ((host = host.BaseType) != null)
        {
            var method = FindWithMethod(host);
            if (method != null && method.ReturnType.IsInterface()) return true; // Good base method...

            attr = FindWithAttribute(host) ?? FindInheritWithsAttribute(host);
            if (attr == null) continue;

            found = GetReturnInterfaceValue(attr, out value);
            if (!found) continue;
            if (!value) return false; // Bad requested base method...
        }

        return true;
    }

    /// <summary>
    /// Captures the return type to use with the generated methods.
    /// </summary>
    INamedTypeSymbol CaptureReturnType()
    {
        if (Host.IsInterface()) return Host; // Return type for interfaces is itself...

        var attr = FindWithAttribute(Host); // Weird, return the host type itself...
        if (attr == null) return Host;

        if (IsInherited) attr = FindInheritWithsAttribute(Host);

        var value = false;
        var found = false;
        if (attr != null) found = GetReturnInterfaceValue(attr, out value); // No requested, return itself...
        if (found && !value) return Host;

        foreach (var iface in Host.Interfaces)
        {
            found = iface.Recursive(iface =>
            {
                var method = FindWithMethod(iface); if (method != null) return true;
                var member = FindMember(iface); if (member != null) return true;

                attr = FindInheritWithsAttribute(iface); if (attr == null) return false;
                found = GetReturnInterfaceValue(attr, out value);
                if (found && value) return true;

                return false;
            },
            allifaces: true);

            if (found) return iface; // Found a first-level interface...
        }

        return Host; // Return host type by default...
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /*protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindCloneMethod(Symbol) != null) return;

        // Dispatching...
        if (Symbol.IsInterface()) EmitForInterface(context, cb);
        else if (Symbol.IsAbstract) EmitForAbstract(context, cb);
        else EmitForConcrete(context, cb);
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    /*void EmitForInterface(SourceProductionContext _, CodeBuilder cb)
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
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract class.
    /// </summary>
    /*void EmitForAbstract(SourceProductionContext _, CodeBuilder cb)
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
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete class.
    /// </summary>
    /*void EmitForConcrete(SourceProductionContext context, CodeBuilder cb)
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
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the explicit interfaces' implementations associated to this type.
    /// </summary>
    /*void EmitExplicitInterfaces(CodeBuilder cb)
    {
        if (Symbol.Name == "Type1B") { } // DEBUG

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
    }*/

    /// <summary>
    /// Gets a list with the interfaces that need explicit implementation.
    /// </summary>
    /*List<ITypeSymbol> GetExplicitInterfaces()
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
    }*/

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
    /// Tries to find the <see cref="InheritWithsAttribute"/> attribute on the given type. The
    /// chain of base types and direct or all implemented interfaces are also used if such is
    /// explicitly requested.
    /// </summary>
    static AttributeData? FindInheritWithsAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();
        },
        chain, ifaces, allifaces);
    }

    /// <summary>
    /// Tries to find the <see cref="WithAttribute"/> attribute on the given type. The chain of
    /// base types and direct or all implemented interfaces are also used if such is explicitly
    /// requested.
    /// </summary>
    static AttributeData? FindWithAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            return type.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        },
        chain, ifaces, allifaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.ReturnInterface"/> property on
    /// the given attribute. If found, returns <c>true</c> and sets the value of the out argument
    /// to the value found. If not, returns <c>false</c>.
    /// </summary>
    static bool GetReturnInterfaceValue(AttributeData at, out bool value)
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
    static bool GetReturnInterfaceValue(
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
    static bool GetPreventVirtualValue(AttributeData at, out bool value)
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
    static bool GetPreventVirtualValue(
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
        /// Emulates the 'with' keyword for the {{Symbol.Name}} member.
        /// </summary>
        {{AttributeDoc}}
        """);
}