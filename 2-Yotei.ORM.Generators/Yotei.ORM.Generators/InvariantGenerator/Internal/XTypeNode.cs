namespace Yotei.ORM.InvariantGenerator;

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

    AttributeData Attribute;
    bool IsBag;
    int Arity;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;
    string Bracket;
    Type Template;

    /// <summary>
    /// Initializes a new instance.
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
        if (Symbol.IsRecord)
        { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }

        // Finding the unique decorating attribute...
        if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
        else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
        else Attribute = Attributes[0];

        // Validating if the attribute is for a bag or a list...
        var atc = Attribute.AttributeClass!;
        IsBag = atc.Name.StartsWith(INVARIANTBAG) || atc.Name.StartsWith(IINVARIANTBAG);

        // Case: attribute is NOT a generic one...
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

                TTypeName = TType.EasyName(EasyTypeOptions.Full);
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

                KTypeName = KType.EasyName(EasyTypeOptions.Full);
                TTypeName = KType.EasyName(EasyTypeOptions.Full);
                if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{KTypeName}, {TTypeName}>";
                Arity = 2;
            }

            // Should not happen...
            else
            {
                TreeError.InvalidAttribute.Report(Symbol, context);
                r = false;
            }
        }

        // Case: attribute IS a <T> one...
        else if (atc.Arity == 1)
        {
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);

            TTypeName = TType.EasyName(EasyTypeOptions.Full);
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{TTypeName}>";
            Arity = 1;
        }

        // Case: attribute IS a <K, T> one...
        else if (atc.Arity == 2)
        {
            Template = typeof(IListTemplate<,>);
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(EasyTypeOptions.Full);
            TTypeName = KType.EasyName(EasyTypeOptions.Full);
            if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{KTypeName}, {TTypeName}>";
            Arity = 2;
        }

        // Should not happen...
        else
        {
            TreeError.InvalidAttribute.Report(Symbol, context);
            r = false;
        }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override string? GetBaseList()
    {
        // Where to find if an invariant-alike element is already present...
        var chains = new INamedTypeSymbol[][]{
            Symbol.IsInterface
            ? [.. Symbol.AllInterfaces]
            : [.. Symbol.AllBaseTypes, .. Symbol.AllInterfaces] };

        // Finding, giving priority to concrete classes over interfaces...
        var found = Finder.Find(chains, out INamedTypeSymbol? value, (type, out value) =>
        {
            if (type.Match(typeof(InvariantBagAttribute)) ||
                type.Match(typeof(InvariantBagAttribute<>)) ||
                type.Match(typeof(InvariantListAttribute)) ||
                type.Match(typeof(InvariantListAttribute<>)) ||
                type.Match(typeof(InvariantListAttribute<,>)) ||
            
                type.Match(typeof(IInvariantBagAttribute)) ||
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

        // Not found, lets add it...
        if (!found)
        {
            var name = Attribute.AttributeClass!.Name;
            name = name.RemoveLast("Attribute").ToString();
            name += Bracket;
            return name;
        }

        // It was already present...
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmitCore(in TreeContext context, CodeBuilder cb)
    {
        // Gets the appropriate return type...
        if (!HasReturnType(Attribute, out var rtype, out var rnull)) rtype = Symbol;
        var roptions = GetReturnOptions(rtype, Symbol);
        var stype = rtype.EasyName(roptions);
        var snull = rnull ? "?" : string.Empty;

        // Try to emit Clone()...
        TryEmitClone(cb, stype, snull);

        // Options...

        //var argoptions = EasyTypeOptions.Empty.WithRecursive(
        //    useVariance: true,
        //    namespaceStyle: EasyNamespaceStyle.Default,
        //    useHost: true,
        //    useSpecialNames: true,
        //    nullableStyle: EasyNullableStyle.UseAnnotations);

        //var moptions = EasyMethodOptions.Empty with
        //{
        //    GenericListOptions = EasyTypeOptions.Full.WithRecursive(
        //        useVariance: false,
        //        useAccessibility: false,
        //        useModifiers: false,
        //        useKind: false),

        //    ParameterOptions = EasyParameterOptions.Empty with
        //    {
        //        UseModifiers = true,
        //        TypeOptions = argoptions,
        //        UseName = true
        //    }
        //};

        // Finding in both template and existing methods...
        var methodinfos = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var methodsymbols = Symbol.GetMembers().OfType<IMethodSymbol>();

        foreach (var method in methodinfos)
        {
            // If already implemented, process next method...
            if (methodsymbols.Any(x => SameMethod(method, x))) continue;

            // Header...
            cb.AppendLine();
            EmitDocumentation(method, cb);

            // Interface...
            if (Symbol.IsInterface)
            {
                //var name = method.EasyName(moptions);
                //name = ReplaceKT(name);

                //cb.AppendLine($"new {stype}{snull} {name};");
                //continue;
            }

            // Other hosts...
            else
            {
                //var mods = Symbol.IsAbstract ? "abstract override" : "override";
                //var name = method.EasyName(moptions);
                //name = ReplaceKT(name);

                //var args = method.EasyName(argoptions);
                //args = ReplaceKT(args);

                //cb.AppendLine($"public {mods} {stype}{snull} {name}");
                //cb.AppendLine($"=> ({stype}{snull})base.{args};");

                //EmitExplicitInterfaces(cb);
            }
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<string> GetExplicitInterfaces(MethodInfo method)
    {
        var comparer = SymbolEqualityComparer.Default;
        List<INamedTypeSymbol> list = [];
        var options = EasyTypeOptions.Full.WithRecursive(
            useVariance: false,
            useAccessibility: false,
            useModifiers: false,
            useKind: false);

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        var items = list.Select(x => x.EasyName(options)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface for the given method.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x, iface));
            if (temp is not null) return true;

            // Finding the interface among the valid ones...
            var temps = iface.GetMembers().OfType<IMethodSymbol>().ToDebugArray();
            var found = temps.Any(x => SameMethod(method, x));
            if (!found) return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Used to replace the K and T generic argument types with the appropriate concrete ones.
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
            if (item.Contains("T?>")) item = item.Replace("T?>", $"{TTypeName}?>");
        }
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to emit a Clone() method...
    /// </summary>
    void TryEmitClone(CodeBuilder cb, string stype, string snull)
    {
        // First thing first, it must be explicitly requested...
        var requested = HasEmitClone(Attribute, out var value) && value;
        if (!requested) return;

        // If existing, or externally requested, we're done...

        // Documentation...

        // Interface...

        // Regular host, we know we are inheriting from a base CLASS...
    }
}