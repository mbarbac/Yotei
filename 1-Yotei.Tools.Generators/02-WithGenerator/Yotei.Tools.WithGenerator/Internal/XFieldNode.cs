namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public class XFieldNode : FieldNode, IXNode<IFieldSymbol>
{
    AttributeData Attribute = default!;
    INamedTypeSymbol Host => Parent!.Symbol;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XFieldNode(IFieldSymbol symbol) : base(symbol) { }

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
        if (!Symbol.IsWrittable) { TreeError.NotWrittable.Report(Symbol, context); r = false; }

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
}