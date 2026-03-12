namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode, IXNode
{
    const string IINVARIANTBAG = "IInvariantBag";
    const string IINVARIANTLIST = "IInvariantList";
    const string INVARIANTBAG = "InvariantBag";
    const string INVARIANTLIST = "InvariantList";
    const string NAMESPACE = "Yotei.ORM.Tools";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    AttributeData Attribute = default!;
    bool IsBag = default;
    int Arity = default;
    INamedTypeSymbol KType = default!; string KTypeName = default!; bool KTypeNullable = false;
    INamedTypeSymbol TType = default!; string TTypeName = default!; bool TTypeNullable = false;
    string Bracket = default!;
    Type Template = default!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }

        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
        else if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
        else Attribute = Attributes[0];
        if (Attribute is null) goto ENDVALIDATION; // A 'goto', what a sin!

        var atc = Attribute.AttributeClass!;

        // Easy way...
        IsBag = atc.Name.StartsWith(INVARIANTBAG) || atc.Name.StartsWith(IINVARIANTBAG);

        // Attribute has no generic arguments...
        if (atc.Arity == 0)
        {
            var args = Attribute.ConstructorArguments
                .Where(static x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(static x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            // For <T> attributes...
            if (args.Length == 1)
            {
                Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
                TType = args[0].UnwrapNullable(out TTypeNullable);

                TTypeName = KType.EasyName(EasyTypeSymbol.Full);
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{TTypeName}>";
            }

            // For <K, T> attributes...
            else if (args.Length == 2)
            {
                Template = typeof(IListTemplate<,>);
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(EasyTypeSymbol.Full);
                TTypeName = KType.EasyName(EasyTypeSymbol.Full);
                if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{KTypeName}, {TTypeName}>";
            }

            // Should not happen...
            else { Symbol.ReportError(TreeError.InvalidAttribute, context); r = false; }
        }

        // For <T> attributes...
        else if (atc.Arity == 1)
        {
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);

            TTypeName = KType.EasyName(EasyTypeSymbol.Full);
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{TTypeName}>";
        }

        // For <K, T> attributes...
        else if (atc.Arity == 2)
        {
            Template = typeof(IListTemplate<,>);
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(EasyTypeSymbol.Full);
            TTypeName = KType.EasyName(EasyTypeSymbol.Full);
            if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{KTypeName}, {TTypeName}>";
        }

        // Should not happen...
        else { Symbol.ReportError(TreeError.InvalidAttribute, context); r = false; }

        ENDVALIDATION:
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override string? GetBaseList()
    {
        var chains = new INamedTypeSymbol[][] {
            Symbol.IsInterface ? [.. Symbol.AllInterfaces] : [.. Symbol.AllBaseTypes] };

        var found = Finder.Find(null, chains, out INamedTypeSymbol? value, (type, out value) =>
        {
            if (type.Match(typeof(IInvariantBagAttribute)) ||
                type.Match(typeof(IInvariantBagAttribute<>)) ||
                type.Match(typeof(IInvariantListAttribute)) ||
                type.Match(typeof(IInvariantListAttribute<>)) ||
                type.Match(typeof(IInvariantListAttribute<,>)))
            {
                value = type;
                return true;
            }
            
            value = null;
            return false;
        });

        if (!found) // We shall include the base type to inherit from...
        {
            var name = Attribute.AttributeClass!.Name;
            name = name.RemoveLast("Attribute").ToString();
            name += Bracket;

            return name;
        }

        return null; // Base type already specified...
    }
}