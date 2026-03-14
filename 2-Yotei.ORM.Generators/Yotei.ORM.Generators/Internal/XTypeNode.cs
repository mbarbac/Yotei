namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal partial class XTypeNode : TypeNode
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
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        Attribute = default!;
        IsBag = default;
        Arity = default;
        KType = default!; KTypeName = default!; KTypeNullable = default;
        TType = default!; TTypeName = default!; TTypeNullable = default;
        Bracket = default!;
        Template = default!;
    }

    AttributeData Attribute;
    bool IsBag;
    int Arity;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;
    string Bracket;
    Type Template;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); return false; }

        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); return false; }
        else if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); return false; }
        else Attribute = Attributes[0];

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

                TTypeName = TType.EasyName(EasyTypeSymbol.Full);
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{TTypeName}>";
                Arity = 1;
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
                Arity = 2;
            }

            // Should not happen...
            else { Symbol.ReportError(TreeError.InvalidAttribute, context); return false; }
        }

        // For <T> attributes...
        else if (atc.Arity == 1)
        {
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);

            TTypeName = TType.EasyName(EasyTypeSymbol.Full);
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{TTypeName}>";
            Arity = 1;
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
            Arity = 2;
        }

        // Should not happen...
        else { Symbol.ReportError(TreeError.InvalidAttribute, context); return false; }

        return true;
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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Emit 'Clone' if needed...
        EmitClone(cb);

        // Find template and existing methods...
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToDebugArray();

        // Get the appropriate return type...
        var rtype = Symbol.UnwrapNullable(out var rnull);
        if (HasReturnType(Attribute, out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        // HIGH: use 'EasyName' to obtain return type, need to modify EasyName logic.

        // Iterating through template method...
        foreach (var method in methods)
        {
            // If implemented, we'done with this method...
            if (existing.Any(x => SameMethod(method, x))) continue;

            // Method header...
            // cb.AppendLine();
            // EmitDocumentation(method, cb);

            // Return type...
            var roptions = ReturnOptions(rtype, Symbol);
        }

        // Finishing...
        return true;
    }

    /// <summary>
    /// Used to replace generic arguments with the appropriate ones.
    /// </summary>
    string ReplaceKT(string item)
    {
        if (KType != null && KType.Name != "K")
        {
            if (item.Contains("K key")) item = item.Replace("K key", $"{KTypeName} key");
            if (item.Contains("K? key")) item = item.Replace("K? key", $"{KTypeName} key");
            if (item.Contains("<K")) item = item.Replace("<K", $"<{KTypeName}");
        }
        if (TType != null && TType.Name != "T")
        {
            if (item.Contains("T value")) item = item.Replace("T value", $"{TTypeName} value");
            if (item.Contains("T? value")) item = item.Replace("T? value", $"{TTypeName} value");
            if (item.Contains("T>")) item = item.Replace("T>", $"{TTypeName}>");
        }
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit 'Clone()' if needed.
    /// </summary>
    void EmitClone(CodeBuilder cb)
    {
        return; // HIGH: EmitClone...
    }
}