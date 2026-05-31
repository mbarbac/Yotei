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

        var options = EasyTypeOptions.Default.WithRecursive(
            namespaceStyle: EasyNamespaceStyle.Default,
            useHost: true);

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

                TTypeName = TType.EasyName(options);
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

                KTypeName = KType.EasyName(options);
                TTypeName = TType.EasyName(options);
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

            TTypeName = TType.EasyName(options);
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

            KTypeName = KType.EasyName(options);
            TTypeName = TType.EasyName(options);
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

        var toptions = EasyTypeOptions.Default.WithRecursive(
            namespaceStyle: EasyNamespaceStyle.Default,
            useHost: true);

        var moptions = EasyMethodOptions.Empty with
        {
            UseAccessibility = true,
            UseModifiers = true,
            GenericListOptions = toptions,
            UseBrackets = true,
            ParameterOptions = EasyParameterOptions.Empty with
            {
                UseModifiers = true,
                TypeOptions = toptions,
                UseName = true
            }
        };

        var argoptions = moptions with
        { ParameterOptions = EasyParameterOptions.Empty with { UseName = true } };

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
                var name = method.EasyName(moptions);
                name = ReplaceKT(name);

                cb.AppendLine($"new {stype}{snull} {name};");
                continue;
            }

            // Other hosts...
            else
            {
                var mods = Symbol.IsAbstract ? "abstract override" : "override";
                var name = method.EasyName(moptions);
                name = ReplaceKT(name);

                var args = method.EasyName(argoptions);
                args = ReplaceKT(args);

                cb.AppendLine($"public {mods} {stype}{snull} {name}");
                cb.AppendLine($"=> ({stype}{snull})base.{args};");

                var ifaces = GetExplicitInterfaces(method);
                foreach (var iface in ifaces)
                {
                    cb.AppendLine();
                    cb.AppendLine(iface);
                    cb.AppendLine($"{iface}.{name} => {args};");
                }
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

        var options = EasyTypeOptions.Default.WithRecursive(
            namespaceStyle: EasyNamespaceStyle.Default,
            useHost: true);

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        var items = list.Select(x => x.EasyName(options)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface for the given method.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x, iface));
            if (temp != null) return;

            // If no attribute and no method, we're done..
            if (!HasInvariantAttribute(iface, out _) &&
                !iface.GetMembers().OfType<IMethodSymbol>().Any(x => SameMethod(method, x)))
                return;

            // Adding to the list...
            list.Add(iface);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Used to replace the K and T generic argument types with the appropriate concrete ones.
    /// NOTE: we use the facts that (1) we know the generic K and T argument names, and (2) we
    /// know in what combinations they appear in the methods we are interested in.
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
        // If existing, or externally requested, we're done...
        if (HasClone(Symbol, out _, out _)) return;

        // Documentation...
        cb.AppendLine($"/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine($"{DocAttribute}");

        // Interface...
        if (Symbol.IsInterface)
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
            return;
        }

        // Other hosts...
        else
        {
            if (Symbol.IsAbstract) // Re-abstracting...
            {
                cb.AppendLine($"public abstract override {stype}{snull} Clone();");
            }
            else // Regular overriding...
            {
                var host = Symbol.EasyName();

                cb.AppendLine($"public override {stype}{snull} Clone()");
                cb.AppendLine("{");
                cb.IndentLevel++;
                {
                    cb.AppendLine($"var v_host = new {host}(this);");
                    cb.AppendLine($"return v_host;");
                }
                cb.IndentLevel--;
                cb.AppendLine("}");
            }

            foreach (var iface in GetCloneInterfaces()) // Explicit interfaces...
            {
                cb.AppendLine();
                cb.AppendLine(iface);
                cb.AppendLine($"{iface}.Clone() => Clone();");
            }
        }
    }

    /// <summary>
    /// Determines if the given type carries a 'Clone()' method, or if that method has been
    /// requested by virtue of an appropriate attribute. If so returns either the found method
    /// or the found attribute in the out arguments.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="method"></param>
    /// <param name="at"></param>
    /// <returns></returns>
    static bool HasClone(INamedTypeSymbol type, out IMethodSymbol? method, out AttributeData? at)
    {
        method = null;
        at = null;

        method = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(static x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        if (method != null) return true;

        at = type.GetAttributes().FirstOrDefault(static x =>
            x.AttributeClass != null &&
            x.AttributeClass.Name == "Cloneable");

        if (at != null) return true;

        return false;
    }


    /// <summary>
    /// Returns a list with the clone-alike interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    List<string> GetCloneInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<INamedTypeSymbol> list = [];

        var options = EasyTypeOptions.Default.WithRecursive(
            namespaceStyle: EasyNamespaceStyle.Default,
            useHost: true);

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        var items = list.Select(x => x.EasyName(options)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface for the given method.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x, iface));
            if (temp != null) return;

            // If no clone method, we're done...
            if (!HasClone(iface, out _, out _) &&
                !HasInvariantAttribute(iface, out _)) return;

            // Adding to the list...
            list.Add(iface);
        }
    }
}