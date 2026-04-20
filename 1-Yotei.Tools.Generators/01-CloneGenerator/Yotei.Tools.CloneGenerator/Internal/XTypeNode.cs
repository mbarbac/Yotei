namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public class XTypeNode : TypeNode
{
    AttributeData Attribute = default!;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Symbol.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }

        if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
        else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
        else Attribute = Attributes[0];

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmitCore(ref TreeContext context, CodeBuilder cb)
    {
        // If already exist we're done...
        if (Helpers.TryFindMethod(Symbol, [], out _)) return true;

        // Otherwise, dispatching...
        if (Symbol.IsInterface) return EmitHostInterface(ref context, cb);
        else if (Symbol.IsAbstract) return EmitHostAbstract(ref context, cb);
        else return EmitHostRegular(ref context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an interface.
    /// </summary>
    bool EmitHostInterface(ref TreeContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is an abstract type.
    /// </summary>
    bool EmitHostAbstract(ref TreeContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host type is a regular type.
    /// </summary>
    bool EmitHostRegular(ref TreeContext context, CodeBuilder cb)
    {
        throw null;

        /// <summary>
        /// Obtains the appropriate modifiers (followed by an space separator), or null if any.
        /// </summary>
        string? GetModifiers()
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    /// <param name="cb"></param>
    void EmitExplicitInterfaces(CodeBuilder cb)
    {
        var options = EasyTypeOptions.Full with { NullableStyle = EasyNullableStyle.None };

        var items = GetExplicitInterfaces();
        foreach (var item in items)
        {
            var iface = item.IFace.EasyName(options);            
            var core = item.IFace.Name == nameof(ICloneable); // Special 'ICloneable' case...
            var rtype = core ? "object" : item.RType.EasyName(EasyTypeOptions.Full);
            if (item.RNullable && !rtype.EndsWith('?')) rtype += '?';

            cb.AppendLine();
            cb.AppendLine($"{rtype}");
            cb.AppendLine($"{iface}.Clone()");
            cb.AppendLine($"=> ({rtype})Clone();");
        }
    }

    /// <summary>
    /// Keeps track of an interface that needs explicit implementation.
    /// </summary>
    /// <param name="IFace">The interface itself.</param>
    /// <param name="RType">The method return type.</param>
    /// <param name="RNullable">Wheter the return type is nullable or not.</param>
    record Explicit(INamedTypeSymbol IFace, INamedTypeSymbol RType, bool RNullable);

    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<Explicit> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<Explicit> list = [];

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
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

            // Special case: ICloneable...
            if (iface.Name == nameof(ICloneable))
            {
                // We can use 'null' as rtype because in this special case it won't be used...
                var item = new Explicit(iface, null!, false);
                list.Add(item);
                return;
            }

            // Method already exists...
            if (Helpers.TryFindMethod(iface, [], out var method))
            {
                var rtype = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out var rnull);
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }

            // Method is requested...
            if (iface.HasCloneableAttribute(out var at, out _))
            {
                var rtype = at.HasReturnType(out var xtype, out var rnull) ? xtype : iface;
                var item = new Explicit(iface, rtype, rnull);
                list.Add(item);
                return;
            }
        }
    }
}