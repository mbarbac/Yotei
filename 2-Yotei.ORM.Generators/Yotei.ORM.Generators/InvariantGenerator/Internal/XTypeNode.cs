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

    bool IsBag;
    string Bracket;
    Type Template;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        ReturnType = Symbol;
        ReturnNullable = false;
        ReturnOptions = EasyNameOptions.Default;

        IsBag = false;
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
        IsBag = atc.Name.Contains(BagName);

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
                Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
                TType = args[0].UnwrapNullable(out TTypeNullable);
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{TTypeName}>";
            }

            // Two type arguments: <K, T>...
            if (args.Length == 2 && !IsBag)
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
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{TTypeName}>";
        }

        // Attribute that uses two generic arguments: <K, T>...
        if (atc.Arity == 2 && !IsBag)
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Emits 'Clone()' if needed...
        EmitClone(cb);

        // Iterating through template's methods...
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();

        foreach (var method in methods)
        {
            // If already exists, we're done...
            if (existing.Any(x => SameMethod(method, x))) continue;

            // Emit documentation...
            var headdoc = Symbol.IsInterface
                ? (IsBag ? IBagName : IListName)
                : (IsBag ? BagName : ListName);

            var name = method.EasyName();
            name = $"{headdoc}{Bracket}.{name}";
            name = name.Replace('<', '{').Replace('>', '}');
            name = $"/// <inheritdoc cref=\"{name}\"/>";

            cb.AppendLine();
            cb.AppendLine(name);
            cb.AppendLine($"{InvariantGenerator.AttributeDoc}");

            // Prepare to emit method...
            var nameoptions = EasyNameOptions.Default with
            { MemberArgumentTypeOptions = EasyNameOptions.Full, MemberUseArgumentNames = true };

            if (method.Name == "Replace") { } // DEBUGING Replace method with out argument

            name = method.EasyName(nameoptions);
            name = name.Replace("K ", $"{KTypeName} "); // K key...
            name = name.Replace("<K", $"<{KTypeName}"); // IComparer<K> comparer...
            name = name.Replace("T ", $"{TTypeName} "); // T item...
            name = name.Replace("T>", $"{TTypeName}>"); // IEnumerable<T> range...

            var rtype = ReturnType.EasyName(ReturnOptions);
            var rnull = ReturnNullable ? "?" : string.Empty;

            // Emit when host is an interface...
            if (Symbol.IsInterface) cb.AppendLine($"new {rtype}{rnull} {name};");

            // Otherwise, emit inheriting from a base class...
            else
            {
                var argoptions = EasyNameOptions.Default with
                { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };

                var args = method.EasyName(argoptions);
                var mods = Symbol.IsAbstract ? "abstract override" : "override";

                cb.AppendLine($"public {mods} {rtype}{rnull} {name}");
                cb.AppendLine($"=> ({rtype}{rnull})base.{args};");

                foreach (var iface in GetMethodInterfaces(method))
                {
                    cb.AppendLine();
                    cb.AppendLine(iface);
                    cb.AppendLine($"{iface}.{name} => {args};");
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method is the same as the given existing one.
    /// </summary>
    bool SameMethod(MethodInfo method, IMethodSymbol existing)
    {
        var mname = method.Name;
        var ename = existing.Name;
        if (mname != ename) return false; // Names differ...

        var mpars = method.GetParameters();
        var epars = existing.Parameters;
        if (mpars.Length != epars.Length) return false; // Number of pars differ...

        for (int i = 0; i < mpars.Length; i++)
        {
            var mpar = mpars[i];
            var epar = epars[i];
            if (!SameArgument(mpar, epar)) return false; // Parameter differ...
        }

        return true; // Same method...
    }

    /// <summary>
    /// Determines if the given arguments are the same or not.
    /// </summary>
    /// **********
    /// HIGH: In SameArgumen() determine also by 'out' modifier!!!
    /// **********
    bool SameArgument(ParameterInfo mpar, IParameterSymbol epar)
    {
        var mtype = mpar.ParameterType;
        var etype = (INamedTypeSymbol)epar.Type;

        return SameArgument(mtype, etype);
    }

    /// <summary>
    /// Determines if the types of the given arguments are the same or not.
    /// </summary>
    bool SameArgument(Type mtype, INamedTypeSymbol etype)
    {
        var comparer = SymbolEqualityComparer.Default;
        switch (mtype.Name)
        {
            case "K": if (!comparer.Equals(KType, etype)) return false; break;
            case "T": if (!comparer.Equals(TType, etype)) return false; break;
            default: if (!etype.Match(mtype)) return false; break;
        }

        var margs = mtype.GenericTypeArguments;
        var eargs = etype.TypeArguments;
        if (margs.Length != eargs.Length) return false;

        for (int i = 0; i < margs.Length; i++)
        {
            var marg = margs[i];
            var earg = eargs[i];
            var same = SameArgument(marg, (INamedTypeSymbol)earg);
            if (!same) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the interfaces for which the given method needs explicit implementation.
    /// </summary>
    List<string> GetMethodInterfaces(MethodInfo method)
    {
        // HIGH: GetMethodInterfaces()...
        return [];
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to emit a 'Clone()' method if needed.
    /// </summary>
    void EmitClone(CodeBuilder cb)
    {
        // HIGH: EmitClone();
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