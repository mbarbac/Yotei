namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public class XPropertyNode : PropertyNode, IXNode<IPropertySymbol>
{
    AttributeData Attribute = default!;
    INamedTypeSymbol Host => Parent!.Symbol;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(IPropertySymbol symbol) : base(symbol) { }

    /// <summary>
    /// Determines if this instance was captured for an inherited member, or not.
    /// </summary>
    public bool Inherited { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        // Records not supported...
        if (Host.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }

        // Member constrains...
        if (Symbol.IsIndexer) { TreeError.IndexerNotSupported.Report(Symbol, context); r = false; }
        if (!Symbol.HasGetter) { TreeError.NoGetter.Report(Symbol, context); r = false; }
        if (!Symbol.HasSetter &&
            !Symbol.ContainingType.IsInterface) { TreeError.NoSetter.Report(Symbol, context); r = false; }

        // Finding the unique decorating attribute...
        if (Inherited)
        {
            var ats = Host.GetAttributes([typeof(InheritsWithAttribute)]).ToList();
            if (ats.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
            else if (ats.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
            else Attribute = ats[0];
        }
        else
        {
            if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
            else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
            else Attribute = Attributes[0];
        }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmit(in TreeContext context, CodeBuilder cb)
    {
        // If already exist we're done...
        if (XNode.TryFindMethod(this.MethodName, this.MemberType, Host, [], out _)) return true;

        // Otherwise, dispatching...
        if (Host.IsInterface) return EmitHostInterface(in context, cb);
        else if (Host.IsAbstract) return EmitHostAbstract(in context, cb);
        else return EmitHostRegular(in context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    bool EmitHostInterface(in TreeContext _, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Obtains the appropriate modifiers, with a space separator, or null if any.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract one.
    /// </summary>
    bool EmitHostAbstract(in TreeContext _, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Obtains the appropriate modifiers, with a space separator, or null if any.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is a regular one.
    /// </summary>
    bool EmitHostRegular(in TreeContext _, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Obtains the appropriate modifiers, with a space separator, or null if any.
        /// </summary>
        [SuppressMessage("", "IDE0075")]
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Used to keep track of interfaces that need explicit implementation.
    /// </summary>
    record Explicit(
        INamedTypeSymbol IFace,
        INamedTypeSymbol RType, bool RNullable, ITypeSymbol ArgType);

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    /// <param name="cb"></param>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        var hoptions = new EasyTypeOptions
        {
            UseSpecialNames = true,
            NullableStyle = EasyNullableStyle.UseAnnotations,
            GenericListOptions = EasyTypeOptions.Default.WithRecursive(
                namespaceStyle: EasyNamespaceStyle.Default,
                useHost: true,
                useSpecialNames: true,
                nullableStyle: EasyNullableStyle.UseAnnotations)
        };

        var roptions = EasyTypeOptions.Full.WithRecursive(
            useVariance: false,
            useAccessibility: false,
            useModifiers: false,
            useKind: false);

        var mname = this.MethodName;

        var items = GetExplicitInterfaces();
        foreach (var item in items)
        {
            var iface = item.IFace.EasyName(hoptions);
            var rtype = item.RType.EasyName(roptions);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';
            var atype = item.ArgType.EasyName(roptions);

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.{mname}({atype} value)");
            cb.AppendLine($"=> ({rtype}){mname}(value);");
        }
    }

    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<Explicit> GetExplicitInterfaces()
    {
        var membername = this.MemberName;
        var methodname = this.MethodName;
        var argtype = this.MemberType;

        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];

        foreach (var iface in Host.Interfaces) TryCapture(iface);
        return list;

        /// <summary>
        /// Tries to capture the given interface.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x.IFace, iface));
            if (temp is not null) return;

            // Method already exists...
            if (XNode.TryFindMethod(methodname, argtype, iface, [], out var method))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var atype = method.Parameters[0].Type;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }

            // Decorated member exists...
            if (XNode.TryFindMember<IPropertySymbol>(membername, iface, [], out var member, out var at))
            {
                if (!at.HasReturnType(out var rtype, out var rnull)) rtype = iface;
                var atype = member.Type;
                var item = new Explicit(iface, rtype, rnull, atype);
                list.Add(item);
                return;
            }

            // Method is requested...
            if (iface.HasInheritsWithAttribute(out var atts))
            {
                at = atts.First();
                if (!at.HasReturnType(out var rtype, out var rnull)) rtype = iface;

                if (XNode.TryFindMember(membername, null, [iface.AllInterfaces], out member, out at))
                {
                    var atype = member.Type;
                    var item = new Explicit(iface, rtype, rnull, atype);
                    list.Add(item);
                    return;
                }
            }
        }
    }
}