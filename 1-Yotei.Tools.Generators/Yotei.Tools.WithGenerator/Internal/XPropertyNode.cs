#pragma warning disable IDE0075

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    bool HasNewModifier = false;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (Host.IsRecord)
        {
            TreeDiagnostics.KindNotSupported(Host).Report(context);
            r = false;
        }
        if (Symbol.IsIndexer)
        {
            TreeDiagnostics.IndexerNotSupported(Symbol).Report(context);
            r = false;
        }
        if (!Symbol.HasGetter())
        {
            TreeDiagnostics.NoGetter(Symbol).Report(context);
            r = false;
        }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit())
        {
            TreeDiagnostics.NoSetter(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        if (ParentNode.Symbol.Name == "ITypeB") { } // DEBUG-ONLY

        // Declared or implemented explicitly, no need for strict typing...
        if (FindMethod(Host) != null) return;

        // Dispatching...
        if (Host.IsInterface()) EmitAsInterface(context, cb);
        else if (Host.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsInterface(SourceProductionContext _, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            // A 'new' property means a brand new 'With'method...
            var hasnew = FindNewModifier();
            if (!hasnew) goto BYDEFAULT;

            // If implemented with strict typing, then 'new' is needed...
            foreach (var iface in Host.AllInterfaces)
            {
                var member = FindDecoratedMember(iface, strict: true);
                if (member != null) return "new ";

                var method = FindMethod(iface, strict: true);
                if (method != null) return "new ";
            }

            BYDEFAULT:
            return null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            // A 'new' property means a brand new 'With'method...
            var hasnew = FindNewModifier();
            if (!hasnew) goto BYDEFAULT;

            // If appears in a base type, then 'override' is needed...
            if (Host.BaseType != null)
            {
                // If there is a base method implementation...
                var method = FindMethod(Host.BaseType, strict: true, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                    return $"{access}abstract override ";
                }

                // Or if it is being implemented...
                var at = FindWithsAttribute(Host.BaseType, strict: true, chain: true);
                var inherit = FindInheritWithsAttribute(Host.BaseType, chain: true);

                if (at != null && inherit != null)
                {
                    return "public abstract override ";
                }
            }

            BYDEFAULT:
            return "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Gets the method modifiers followed by a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var prevent = FindPreventVirtual(Host, out var temp, strict: true) && temp;
            var issealed = Host.IsSealed;

            // A 'new' property means a brand new 'With'method...
            var hasnew = FindNewModifier();
            if (!hasnew) goto BYDEFAULT;

            // If appears in a base type, then 'override' or 'new' is needed...
            if (Host.BaseType != null)
            {
                // If there is a base method implementation...
                var method = FindMethod(Host.BaseType, strict: true, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null; // Cannot override...

                    var accstr = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                    return isvirtual switch
                    {
                        true => $"{accstr}override ",
                        false => $"{accstr}new ",
                    };
                }

                // Or if it is being implemented...
                var at = FindWithsAttribute(Host.BaseType, strict: true, chain: true);
                var inherit = FindInheritWithsAttribute(Host.BaseType, chain: true);

                if (at != null && inherit != null)
                {
                    return prevent ? "public new " : "public override ";
                }
            }

            BYDEFAULT:
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the explicit interface implementations associated to this type, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitExplicitInterfaces(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    /// <summary>
    /// Returns a list with the interface types that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetExplicitInterfaces()
    {
        throw null;
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
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{AttributeDoc}}
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member with the same name, at the given type, and then searching also
    /// in its base ones and interfaces if requested. The member type must either be strictly
    /// the same, or a compatible one. Returns null if none can be found.
    /// </summary>
    public IPropertySymbol? FindMember(
        ITypeSymbol type,
        bool strict = false, bool chain = false, bool ifaces = false, bool decorated = false)
    {
        var comparer = SymbolEqualityComparer.Default;

        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.Parameters.Length == 0 &&
            (strict
            ? comparer.Equals(Symbol.Type, x.Type)
            : Symbol.Type.IsAssignableTo(x.Type)) &&
            (decorated
            ? x.HasAttributes(typeof(WithAttribute))
            : true));

        if (item is null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindMember(temp, strict);
                if (item is not null) break;
            }
        }

        if (item is null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindMember(temp, strict);
                if (item is not null) break;
            }
        }

        return item;
    }

    /// <summary>
    /// Tries to find a decorated member with the same name, at the given type, and then searching
    /// also in its base ones and interfaces if requested. The member type must either be strictly
    /// the same, or a compatible one. Returns null if none can be found.
    /// </summary>
    public IPropertySymbol? FindDecoratedMember(
        ITypeSymbol type, bool strict = false, bool chain = false, bool ifaces = false)
    {
        return FindMember(type, strict, chain, ifaces, decorated: true);
    }

    /// <summary>
    /// Tries to find a '<c>With[name](value)</c>' method at the given type, and then searching
    /// also in its base ones and interfaces if requested. The argument type must either be strictly
    /// the same as this symbol, or a compatible one. Returns null if none can be found.
    /// </summary>
    public IMethodSymbol? FindMethod(
        ITypeSymbol type,
        bool strict = false, bool chain = false, bool ifaces = false)
    {
        var comparer = SymbolEqualityComparer.Default;

        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            (strict
            ? comparer.Equals(Symbol.Type, x.Parameters[0].Type)
            : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));

        if (item is null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindMethod(temp, strict);
                if (item is not null) break;
            }
        }

        if (item is null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindMethod(temp, strict);
                if (item is not null) break;
            }
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this symbol has a 'new' modifier.
    /// </summary>
    bool FindNewModifier()
    {
        var comparer = SymbolEqualityComparer.Default;

        if (!ParentNode.Symbol.IsInterface())
        {
            foreach (var type in ParentNode.Symbol.AllBaseTypes())
            {
                var item = FindMember(type);
                if (item is not null && !comparer.Equals(item, Symbol)) return true;
            }
        }

        else
        {
            foreach (var type in ParentNode.Symbol.AllInterfaces)
            {
                var item = FindMember(type);
                if (item is not null && !comparer.Equals(item, Symbol)) return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the value of the <see cref="WithAttribute.PreventVirtual"/> setting on a decorated
    /// member of the given type, or the <see cref="InheritWithsAttribute.PreventVirtual"/> one
    /// of the given type, including also its base types and interfaces if needed.
    /// </summary>
    bool FindPreventVirtual(ITypeSymbol type, out bool value, bool strict = false)
    {
        var at = FindWithsAttribute(type, strict: strict);
        if (at != null && Extract(at, out value)) return true;

        at = FindInheritWithsAttribute(type);
        if (at != null && Extract(at, out value)) return true;

        foreach (var temp in type.AllBaseTypes())
        {
            if (FindPreventVirtual(temp, out value, strict: strict)) return true;
        }

        foreach (var temp in type.AllInterfaces)
        {
            if (FindPreventVirtual(temp, out value, strict: strict)) return true;
        }

        value = default!;
        return false;

        /// <summary>
        /// Extracts the value of the 'PreventVirtual' setting from the given attribute.
        /// </summary>
        static bool Extract(AttributeData at, out bool value)
        {
            if (at.GetNamedArgument(nameof(WithAttribute.PreventVirtual), out var arg))
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
    }

    /// <summary>
    /// Returns the <see cref="WithAttribute"/> applied to a decorated member of the given type,
    /// including also its base types and interfaces, if requested, or null if any is found.
    /// </summary>
    AttributeData? FindWithsAttribute(
        ITypeSymbol type, bool strict = false, bool chain = false, bool ifaces = false)
    {
        var member = FindDecoratedMember(type, strict: strict);
        var at = member?.GetAttributes(typeof(WithAttribute)).FirstOrDefault();

        if (at != null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                at = FindWithsAttribute(temp, strict: strict);
                if (at != null) break;
            }
        }

        if (at != null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                at = FindWithsAttribute(temp, strict: strict);
                if (at != null) break;
            }
        }

        return at;
    }

    /// <summary>
    /// Returns the <see cref="InheritWithsAttribute"/> applied to the given type, including
    /// also its base types and interfaces, if requested, or null if any is found.
    /// </summary>
    AttributeData? FindInheritWithsAttribute(
        ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var at = type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();

        if (at != null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                at = FindInheritWithsAttribute(temp);
                if (at != null) break;
            }
        }

        if (at != null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                at = FindInheritWithsAttribute(temp);
                if (at != null) break;
            }
        }

        return at;
    }
}