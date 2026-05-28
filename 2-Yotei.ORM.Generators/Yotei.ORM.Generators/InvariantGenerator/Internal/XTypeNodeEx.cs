namespace Yotei.ORM.InvariantGenerator;

// ========================================================
partial class XTypeNode
{
    readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    const string USEVIRTUAL = "UseVirtual";
    const string RETURNTYPE = "ReturnType";
    const string EMITCLONE = "EmitClone";

    // ----------------------------------------------------

    // <summary>
    /// Gets the version of this generator for documentation purposes.
    /// </summary>
    static string DocVersion => Assembly.GetExecutingAssembly().GetName().Version.To3String();

    /// <summary>
    /// Gets the string that emits the attribute decoration, for documentation purposes.
    /// </summary>
    static string DocAttribute => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantGenerator)}}", "{{DocVersion}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated methods.
    /// </summary>
    void EmitDocumentation(MethodInfo method, CodeBuilder cb)
    {
        var headdoc = Symbol.IsInterface
            ? (IsBag ? IINVARIANTBAG : IINVARIANTLIST)
            : (IsBag ? INVARIANTBAG : INVARIANTLIST);

        var name = method.EasyName();
        name = $"{headdoc}{Bracket}.{name}";
        name = name.Replace('<', '{').Replace('>', '}');
        name = $"/// <inheritdoc cref=\"{name}\"/>";

        cb.AppendLine(name);
        cb.AppendLine($"{DocAttribute}");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is decorated with a "Invariant"-alike attribute. If so,
    /// returns the found ones in the out argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    static bool HasInvariantAttribute(
        ISymbol symbol,
        out IEnumerable<AttributeData> attributes)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        attributes = symbol.GetAttributes([
            typeof(IInvariantBagAttribute), typeof(IInvariantBagAttribute<>),
            typeof(IInvariantListAttribute),
            typeof(IInvariantListAttribute<>), typeof(IInvariantListAttribute<,>),

            typeof(InvariantBagAttribute), typeof(InvariantBagAttribute<>),
            typeof(InvariantListAttribute),
            typeof(InvariantListAttribute<>), typeof(InvariantListAttribute<,>)
            ]);

        return attributes.Any();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the "EmitClone" setting on the given attribute. If so, returns
    /// it in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    static bool HasEmitClone(
        AttributeData at,
        [NotNullWhen(true)] out bool value)
    {
        ArgumentNullException.ThrowIfNull(at);
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        if (at.FindNamedArgument(EMITCLONE, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = default;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the "ReturnType" setting on the given attribute. If so, returns
    /// it in the out argument, and whether it is a nullable one or not.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    static bool HasReturnType(
        AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at);
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        if (at.FindNamedArgument(RETURNTYPE, out var arg))
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

    /// <summary>
    /// Gets the appropriate options to emit the given type, based upon whether it is the same as
    /// the other given one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    static EasyTypeOptions GetReturnOptions(
        INamedTypeSymbol type,
        INamedTypeSymbol other)
    {
        var options = EasyTypeOptions.Default with
        {
            NullableStyle = EasyNullableStyle.None,
            GenericListOptions = EasyTypeOptions.Default.WithRecursive(
                namespaceStyle: EasyNamespaceStyle.Default,
                useHost: true,
                useSpecialNames: true,
                nullableStyle: EasyNullableStyle.None)
        };

        var same = SymbolEqualityComparer.Default.Equals(type, other);
        if (!same)
        {
            options.NamespaceStyle = EasyNamespaceStyle.Default;
            options.UseHost = true;
        }

        return options;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method info and symbol are logically the same one, for the sole
    /// purposes of this generator.
    /// </summary>
    bool SameMethod(MethodInfo methodinfo, IMethodSymbol methodsymbol)
    {
        // Same name...
        var mname = methodinfo.Name;
        var sname = methodsymbol.Name;
        if (mname != sname) return false;

        // Same parameters...
        var mpars = methodinfo.GetParameters();
        var spars = methodsymbol.Parameters;
        if (mpars.Length != spars.Length) return false;

        for (int i = 0; i < mpars.Length; i++)
        {
            var mpar = mpars[i];
            var spar = spars[i];
            if (!SameArgument(mpar, spar)) return false;
        }

        // Finishing...
        return true;
    }

    /// <summary>
    /// Determines if the given parameter info and symbol are logically the same one, for the
    /// sole purposes of this generator. Comparison only takes into consideration their types
    /// and modifiers, but not their names.
    /// </summary>
    bool SameArgument(ParameterInfo parinfo, IParameterSymbol parsymbol)
    {
        // Validate modifiers...
        // TODO: InvariantGenerator comparison of parameters' modifiers

        // Validate type...
        var mtype = parinfo.ParameterType;
        var stype = (INamedTypeSymbol)parsymbol.Type;
        if (!SameType(mtype, stype)) return false;

        // Finishing...
        return true;
    }

    /// <summary>
    /// Determines if the given type info and symbol are logically the same one, for the sole
    /// purposes of this generator.
    /// </summary>
    bool SameType(Type typeinfo, INamedTypeSymbol typesymbol)
    {
        // Comparing types. We intercept K and T ones, using that that we know that these actually
        // are their names. If this ever changes, change the following logic accordingly...
        switch (typeinfo.Name)
        {
            case "K": if (!Comparer.Equals(KType, typesymbol)) return false; break;
            case "T": if (!Comparer.Equals(TType, typesymbol)) return false; break;
            default: if (!typesymbol.Match(typeinfo)) return false; break;
        }

        // Validating their generic arguments, if any...
        var margs = typeinfo.GenericTypeArguments;
        var sargs = typesymbol.TypeArguments;
        if (margs.Length != sargs.Length) return false;

        for (int i = 0; i < margs.Length; i++)
        {
            var marg = margs[i];
            var sarg = sargs[i];
            var same = SameType(marg, (INamedTypeSymbol)sarg);
            if (!same) return false;
        }

        // Finishing...
        return true;
    }
}