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

    public static AttributeData? FindWithAttribute(
        IPropertySymbol symbol)
    {
        throw null;
    }
}
/*


    

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
    public static AttributeData? FindWithAttribute(
        ITypeSymbol type,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
        var at = top
            ? type.GetAttributes(typeof(WithAttribute)).FirstOrDefault()
            : null;

        if (at == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if ((at = FindWithAttribute(temp)) != null) break;
        }

        if (at == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if ((at = FindWithAttribute(temp)) != null) break;
        }

        return at;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithAttribute.InheritMembers"/> property
    /// from the given attribute. Returns <c>false</c> if the setting is not found, or otherwise
    /// <c>true</c> and the setting's value in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetInheritMembersValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(WithAttribute.InheritMembers), out var arg))
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
    /// Tries to find the value of the <see cref="WithAttribute.InheritMembers"/> property
    /// in the given type, including  inheritance and interfaces chains, if requested. By default,
    /// the search starts from the given type, but if the <paramref name="top"/> flag is set to
    /// <c>false</c>, then only its base types and interfaces are considered.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="top"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public static bool GetInheritMembersValue(
        ITypeSymbol type,
        out bool value,
        bool top = true,
        bool chain = false, bool ifaces = false)
    {
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