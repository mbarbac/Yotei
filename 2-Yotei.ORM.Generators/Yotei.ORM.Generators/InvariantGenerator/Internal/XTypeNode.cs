namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    const string IListName = "IInvariantList";
    const string ListName = "InvariantList";
    const string IBagName = "IInvariantBag";
    const string BagName = "InvariantBag";
    const string Namespace = "Yotei.ORM.Tools";

    const string RETURNTYPE = "ReturnType";

    INamedTypeSymbol ReturnType;
    bool ReturnNullable;
    EasyNameOptions ReturnOptions;

    string Bracket;
    Type Template;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        ReturnType = Symbol;
        ReturnNullable = false;
        ReturnOptions = EasyNameOptions.Default;

        Bracket = null!;
        Template = null!;
        KType = null!; KTypeName = null!; KTypeNullable = false;
        TType = null!; TTypeName = null!; TTypeNullable = false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// NOTE: At the end of this method we expect to have the above fields with their values set.
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        // Records not allowed, only one attribute allowed...
        if (Symbol.IsRecord) { CoreDiagnostics.RecordsNotSupported(Symbol).Report(context); return false; }
        if (Attributes.Length == 0) { CoreDiagnostics.NoAttributes(Symbol).Report(context); return false; }
        if (Attributes.Length > 1) { CoreDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }

        var at = Attributes[0];
        var atc = at.AttributeClass;
        if (atc is null) { CoreDiagnostics.InvalidAttribute(Symbol).Report(context); return false; }

        // Getting the custom return type, if any...
        if (GetReturnType(at, out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(Symbol, type);
            if (!same)
                ReturnOptions = EasyNameOptions.Full with { TypeUseNullable = false };
        }

        // Identifies if attribute is an invariant bag, or otherwise an invariant list...
        var isbag = atc.Name.Contains("InvariantBag");

        // Attribute that uses no generic arguments...
        if (atc.Arity == 0)
        {
            var args = at.ConstructorArguments
                .Where(static x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(static x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            // One type argument: <T>...
            if (args.Length == 1)
            {
                Template = isbag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
                TType = args[0].UnwrapNullable(out TTypeNullable);
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{TTypeName}>";
            }

            // Two type arguments: <K, T>...
            if (args.Length == 2 && !isbag)
            {
                Template = typeof(IListTemplate<,>);
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(EasyNameOptions.Full) + (KTypeNullable ? "?" : "");
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{KTypeName}, {TTypeName}>";
            }
        }

        // Attribute that uses one generic argument: <T>...
        if (atc.Arity == 1)
        {
            Template = isbag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{TTypeName}>";
        }

        // Attribute that uses two generic arguments: <K, T>...
        if (atc.Arity == 2 && !isbag)
        {
            Template = typeof(IListTemplate<,>);
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(EasyNameOptions.Full) + (KTypeNullable ? "?" : "");
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{KTypeName}, {TTypeName}>";
        }

        // Finishing...
        if (Template is null) CoreDiagnostics.NoAttributes(Symbol).Report(context);
        return Template is not null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override string GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var need = !HasHeader(out var name);
        if (need) head += $" : {Namespace}.{name}";
        return head;
    }

    /// <summary>
    /// Determines if this type already has a base element of the appropriate 'Invariant' alike
    /// type, either by itself or through inheritance.
    /// </summary>
    bool HasHeader(out string? name)
    {
        var at = Attributes[0].AttributeClass!.Name;
        at = at.RemoveTail("Attribute").ToString();
        at += Bracket;
        name = at;

        var args = EasyNameOptions.Full;
        var opts = EasyNameOptions.Default with { TypeGenericArgumentOptions = args };

        return Symbol.Finder<bool>(false, (type, out value) =>
        {
            var temp = type.EasyName(opts);
            value = string.Compare(at, temp) == 0;
            return value;
        },
        out _, Symbol.AllBaseTypes, Symbol.AllInterfaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the unique 'Invariant' attribute that decorates the given type, or in the
    /// first one in the given chains. If several are found, an error is reported and this method
    /// returns false.
    /// </summary>
    static bool FindInvariantAttribute(
        INamedTypeSymbol type,
        bool usehost,
        [NotNullWhen(true)] out AttributeData? value,
        SourceProductionContext context,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Finder(usehost, (type, out value) =>
        {
            var items = type.GetAttributes().Where(static x =>
                x.AttributeClass is not null && (
                x.AttributeClass.Name.StartsWith("IInvariantList") ||
                x.AttributeClass.Name.StartsWith("InvariantList") ||
                x.AttributeClass.Name.StartsWith("IInvariantBag") ||
                x.AttributeClass.Name.StartsWith("InvariantBag")))
                .ToList();

            if (items.Count == 1) { value = items[0]; return true; }

            if (items.Count > 1) CoreDiagnostics.TooManyAttributes(type).Report(context);
            value = null;
            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the value of the 'ReturnType' property from the given attribute.
    /// </summary>
    static bool GetReturnType(
        AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        if (at.GetNamedArgument(RETURNTYPE, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
            {
                value = temp.UnwrapNullable(out nullable);
                return true;
            }
        }

        value = null;
        nullable = false;
        return false;
    }
}