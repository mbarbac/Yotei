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

    /// <summary>
    /// Finds a compatible member starting by default at the the given type, or otherwise, using
    /// the type's base ones and interfaces, if requested. Returns null if no member can be found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="top"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public IPropertySymbol? FindMember(
        ITypeSymbol type,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var member = top
            ? type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name &&
                x.Parameters.Length == 0 &&
                Symbol.Type.IsAssignableTo(x.Type))
            : null;

        if (member == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                member = FindMember(temp);
                if (member != null) break;
            }
        }

        if (member == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                member = FindMember(temp);
                if (member != null) break;
            }
        }

        return member;
    }

    /// <summary>
    /// Finds a decorated compatible member starting by default at the the given type, or
    /// otherwise, using the type's base ones and interfaces, if requested. Returns null if
    /// no member can be found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="top"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public IPropertySymbol? FindDecoratedMember(
        ITypeSymbol type,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var member = top
            ? type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
                x.Name == Symbol.Name &&
                x.Parameters.Length == 0 &&
                Symbol.Type.IsAssignableTo(x.Type) &&
                x.HasAttributes(typeof(WithAttribute)))
            : null;

        if (member == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                member = FindMember(temp);
                if (member != null) break;
            }
        }

        if (member == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                member = FindMember(temp);
                if (member != null) break;
            }
        }

        return member;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the <see cref="WithAttribute"/> in the given type, including its inheritance and
    /// interfaces chains, if requested, or null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public AttributeData? FindWithAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false)
    {
        var member = FindDecoratedMember(type, true, chain, ifaces);
        var item = member?.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithAttribute.PreventVirtual"/> property
    /// from the given attribute. Returns <c>false</c> if the setting is not found, or otherwise
    /// <c>true</c> and the setting's value in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool GetPreventVirtualValue(AttributeData at, out bool value)
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

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.PreventVirtual"/> property
    /// in this member, or in its base type's ones and interfaces , if requested. By default,
    /// the search starts from the given type, but if the <paramref name="top"/> flag is set to
    /// <c>false</c>, then only its base types and interfaces are considered.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="top"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public bool GetPreventVirtualValue(
        ITypeSymbol type,
        out bool value,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        AttributeData? at = null;

        if (top) at = FindWithAttribute(type, chain, false);
        else
        {
            var host = type.BaseType;
            if (host != null) at = FindWithAttribute(host, chain, ifaces);
        }

        value = at != null && (GetPreventVirtualValue(at, out var temp) && temp);
        return at != null;
    }
}
/*
        var at = FindWithAttribute(type, top, chain, ifaces);

        if (at != null)
        {
            if (GetInheritMembersValue(at, out value)) return true;
        }

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetInheritMembersValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetInheritMembersValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }
 */