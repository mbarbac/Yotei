namespace Yotei.Tools.WithGenerator;

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited properties, if any.
    /// </summary>
    void CaptureProperties()
    {
        var comparer = SymbolComparer.Empty;

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures members at the type's level...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => comparer.Equals(x.Symbol, member));
                if (temp == null)
                {
                    var node = new XPropertyNode(this, member);
                    ChildProperties.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited fields, if any.
    /// </summary>
    void CaptureFields()
    {
        var comparer = SymbolComparer.Empty;

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures members at the type's level...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => comparer.Equals(x.Symbol, member));
                if (temp == null)
                {
                    var node = new XFieldNode(this, member);
                    ChildFields.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the <see cref="InheritWithsAttribute"/> attribute in the given type,
    /// including also its base types and interfaces if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public static AttributeData? FindInheritWithsAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false)
    {
        var at = type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();

        if (at == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                at = FindInheritWithsAttribute(temp);
                if (at != null) break;
            }
        }

        if (at == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                at = FindInheritWithsAttribute(temp);
                if (at != null) break;
            }
        }

        return at;
    }

    // ----------------------------------------------------

    /*
    /// <summary>
    /// Tries to get the value of the '<see cref="WithAttribute.InheritMembers"/>'
    /// named argument from the given attribute data. Returns <c>true</c> if the value is found,
    /// and the value itself in the <paramref name="value"/> parameter, or false otherwise.
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
    }*/

    /*
    /// <summary>
    /// Tries to get the value of the '<see cref="WithAttribute.InheritMembers"/>'
    /// named argument from that attribute applied to the given type, if any. Returns <c>true</c>
    /// if the value is found, and the value itself in the <paramref name="value"/> parameter, or
    /// false otherwise.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public static bool GetTypeInheritMembersValue(
        ITypeSymbol type,
        out bool value,
        bool chain = false, bool ifaces = false)
    {
        var at = FindTypeWithAttribute(type, chain, ifaces);
        if (at != null)
            if (GetInheritMembersValue(at, out value)) return true;

        if (chain)
        {
            foreach (var temp in type.AllBaseTypes())
                if (GetTypeInheritMembersValue(temp, out value)) return true;
        }

        if (ifaces)
        {
            foreach (var temp in type.AllInterfaces)
                if (GetTypeInheritMembersValue(temp, out value)) return true;
        }

        value = default;
        return false;
    }*/
}