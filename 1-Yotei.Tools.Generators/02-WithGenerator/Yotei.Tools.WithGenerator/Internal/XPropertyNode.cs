namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(IPropertySymbol symbol) : base(symbol)
    {
        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"v_{Symbol.Name}";
    }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }

    string MethodName = default!;
    string ArgumentName = default!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);
        var host = ParentNode!.Symbol;

        if (host.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if (Symbol.IsIndexer) { Symbol.ReportError(TreeError.IndexerNotSupported, context); r = false; }
        if (!Symbol.HasGetter) { Symbol.ReportError(TreeError.NoGetter, context); r = false; }
        if (!Symbol.HasSetter && !host.IsInterface) { Symbol.ReportError(TreeError.NoSetter, context); r = false; }
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a decorated member in the given type, if not null, and in the given type chains.
    /// </summary>
    public bool FindMember(
        [NotNullWhen(true)] out IPropertySymbol? value,
        [NotNullWhen(true)] out AttributeData? attr,
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
    {
        var found = Finder.Find((type, out value) =>
        {
            var member = type
            .GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x => x.Name == Symbol.Name);

            if (member != null)
            {
                var ats = member.GetAttributes([typeof(WithAttribute), typeof(WithAttribute<>)]).ToArray();
                if (ats.Length == 1)
                {
                    value = new(member, ats[0]);
                    return true;
                }
            }

            value = null;
            return false;
        },
        out FindMemberTemp? temp, type, chains);

        value = found ? null : temp!.Element;
        attr = found ? null : temp!.Attribute;
        return found;
    }
    record FindMemberTemp(IPropertySymbol? Element, AttributeData Attribute);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a compatible in the given type, if not null, and in the given type chains.
    /// </summary>
    public bool FindMethod(
        [NotNullWhen(true)] out IMethodSymbol? value,
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == MethodName &&
                x.Parameters.Length == 1 &&
                Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

            return value != null;
        },
        out value, type, chains);
    }
}