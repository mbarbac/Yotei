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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        // Base validations...
        if (!base.Validate(context)) r = false;

        // Other validations...
        if (Host.IsRecord) { TreeDiagnostics.RecordsNotSupported(Host).Report(context); r = false; }
        if (Symbol.IsIndexer) { TreeDiagnostics.IndexerNotSupported(Symbol).Report(context); r = false; }
        if (!Symbol.HasGetter()) { TreeDiagnostics.NoGetter(Symbol).Report(context); r = false; }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit()) { TreeDiagnostics.NoSetter(Symbol).Report(context); r = false; }
        if (!CanUseReturnInterface()) { WithDiagnostics.InvalidReturnInterface(Host).Report(context); return false; }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindMethod(Host) != null) return;

        // Dispatching...
        if (Host.IsInterface()) EmitAsInterface(context, cb);
        else if (Host.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    void EmitAsInterface(SourceProductionContext source, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract class.
    /// </summary>
    void EmitAsAbstract(SourceProductionContext source, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete class.
    /// </summary>
    void EmitAsConcrete(SourceProductionContext source, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member with the same name as this symbol in the given type, whose type
    /// is either the same as this symbol (strict mode), or just compatible with it by default.
    /// he base types and interfaces of the host type are also searched if explicitly requested.
    /// Returns <c>null</c> if not found.
    /// </summary>
    IPropertySymbol? FindMember(
        INamedTypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false, bool decorated = false)
    {
        var comparer = SymbolEqualityComparer.Default;

        var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.Parameters.Length == 0 &&
            (decorated
            ? x.HasAttributes(typeof(WithAttribute))
            : true) &&
            (strict
            ? comparer.Equals(Symbol.Type, x.Parameters[0].Type)
            : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));

        if (item == null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindMember(child, strict, false, false, decorated);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindMember(child, strict, false, false, decorated);
                if (item != null) break;
            }
        }

        return item;
    }

    /// <summary>
    /// Tries to find a member with the same name as this symbol in the given type, whose type
    /// is either the same as this symbol (strict mode), or just compatible with it by default.
    /// he base types and interfaces of the host type are also searched if explicitly requested.
    /// Returns <c>null</c> if not found.
    /// </summary>
    IPropertySymbol? FindDecoratedMember(
        INamedTypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false) => FindMember(type, strict, chain, ifaces, true);

    /// <summary>
    /// Returns the 'With()' method declared or implemented by the given type, where the method's
    /// unique argument must either be the same as this symbol (strict mode), or just compatible
    /// with it by default. The base types and interfaces of the host type are also searched if
    /// explicitly requested. Returns <c>null</c> if not found.
    /// </summary>
    IMethodSymbol? FindMethod(
        INamedTypeSymbol type,
        bool strict = false,
        bool chain = false, bool ifaces = false)
    {
        var comparer = SymbolEqualityComparer.Default;

        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            (strict
            ? comparer.Equals(Symbol.Type, x.Parameters[0].Type)
            : Symbol.Type.IsAssignableTo(x.Parameters[0].Type)));

        if (item == null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                item = FindMethod(child, strict);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                item = FindMethod(child, strict);
                if (item != null) break;
            }
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the <see cref="WithAttribute"/> attribute applied to a member whose name matches
    /// this one in the given type, or null if any. The base types and interfaces of the type are
    /// also searched, if such is explicitly requested.
    /// </summary>
    AttributeData? FindWithAttribute(
        INamedTypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var member = FindDecoratedMember(type, strict: false);
        var at = member?.GetAttributes(typeof(WithAttribute)).FirstOrDefault();

        if (at == null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                at = FindWithAttribute(child);
                if (at != null) break;
            }
        }

        if (at == null && ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                at = FindWithAttribute(child);
                if (at != null) break;
            }
        }

        return at;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithAttribute.ReturnInterface"/> named argument
    /// from the given attribute. If found among the used arguments, returns true and its actual
    /// value in the out argument. Otherwise, returns false.
    /// </summary>
    static bool ValueReturnInterface(AttributeData at, out bool value)
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
    /// Tries to get the value of the <see cref="WithAttribute.ReturnInterface"/> named argument
    /// from the attribute that decorates the given type, if any. If found,returns true and its
    /// actual value in the out argument. Otherwise, returns false.
    /// </summary>
    bool ValueReturnInterface(
        INamedTypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var at = FindWithAttribute(type, chain, ifaces);
        if (at != null && ValueReturnInterface(at, out value)) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes())
                if (ValueReturnInterface(child, out value)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces)
                if (ValueReturnInterface(child, out value)) return true;

        value = false;
        return false;
    }

    /// <summary>
    /// When the host is not an interface and <see cref="WithAttribute.ReturnInterface"/> is
    /// requested, then the return type of a base method must be an interface as well, because
    /// C# cannot override with an interface a concrete return type.
    /// </summary>
    bool CanUseReturnInterface()
    {
        // If host is an interface, not applicable...
        if (Host.IsInterface()) return true;

        // If no return interface is requested, just return...
        var useiface = ValueReturnInterface(Host, out var temp) && temp;
        if (!useiface) return true;

        // If a base method's return type is not an interface, it is an error...
        var host = Host;
        while ((host = host.BaseType) != null)
        {
            var method = FindMethod(host);
            if (method != null && method.ReturnType.IsInterface()) return false;

            var at = FindWithAttribute(host);
            if (at == null) continue;
            useiface = ValueReturnInterface(at, out temp) && temp;
            if (!useiface) return false;
        }

        // No errors found...
        return true;
    }

    /// <summary>
    /// Returns the return type to use with the generated methods.
    /// <br/> If the host type is an interface, then that interface is always returned.
    /// <br/> Otherwise, if <see cref="WithAttribute.ReturnInterface"/> is requested, the return
    /// type will either be the first directly implemented interface that qualifies, or the class
    /// type itself.
    /// </summary>
    /// <returns></returns>
    INamedTypeSymbol CaptureReturnType()
    {
        // Not an interface and return interface requested...
        if (!Host.IsInterface())
        {
            var useiface = ValueReturnInterface(Host, out var temp) && temp;
            if (useiface)
            {
                foreach (var iface in Host.Interfaces)
                {

                }
            }
        }

        // By default, returns the host type...
        return Host;
    }
    /*
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
     */

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithAttribute.PreventVirtual"/> named argument
    /// from the given attribute. If found among the used arguments, returns true and its actual
    /// value in the out argument. Otherwise, returns false.
    /// </summary>
    static bool ValuePreventVirtual(AttributeData at, out bool value)
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
    /// Tries to get the value of the <see cref="WithAttribute.PreventVirtual"/> named argument
    /// from the attribute that decorates the given type, if any. If found,returns true and its
    /// actual value in the out argument. Otherwise, returns false.
    /// </summary>
    bool ValuePreventVirtual(
        INamedTypeSymbol type, out bool value, bool chain = false, bool ifaces = false)
    {
        var at = FindWithAttribute(type, chain, ifaces);
        if (at != null && ValuePreventVirtual(at, out value)) return true;

        if (chain)
            foreach (var child in type.AllBaseTypes())
                if (ValuePreventVirtual(child, out value)) return true;

        if (ifaces)
            foreach (var child in type.AllInterfaces)
                if (ValuePreventVirtual(child, out value)) return true;

        value = false;
        return false;
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
        /// Emulates the 'with' keyword for the '{{Symbol.Name}}' decorated member.
        /// </summary>
        {{AttributeDoc}}
        """);
}