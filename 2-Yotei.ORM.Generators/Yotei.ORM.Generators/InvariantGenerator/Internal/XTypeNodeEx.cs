
namespace Yotei.ORM.Generators;

// ========================================================
partial class XTypeNode
{
    readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    // ----------------------------------------------------

    /// <summary>
    /// Gets the version of this generator for documentation purposes.
    /// </summary>
    static string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.To3String();

    /// <summary>
    /// Obtains a string with the 'GeneratedCode' attribute for documentation purposes.
    /// </summary>
    static string AttributeDoc => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Invoked to emit a method documentation.
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
        cb.AppendLine($"{AttributeDoc}");
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is decorated with any 'Invariant'-alike attributes and, if
    /// so, returns them all in the out argument.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    static bool HasInvariantAttributes(
        ITypeSymbol type,
        out IEnumerable<AttributeData> attributes)
    {
        attributes = type.GetAttributes([
            typeof(IInvariantBagAttribute),
            typeof(IInvariantBagAttribute<>),
            typeof(IInvariantListAttribute),
            typeof(IInvariantListAttribute<>),
            typeof(IInvariantListAttribute<,>),
            typeof(InvariantBagAttribute),
            typeof(InvariantBagAttribute<>),
            typeof(InvariantListAttribute),
            typeof(InvariantListAttribute<>),
            typeof(InvariantListAttribute<,>),
        ]);

        return attributes.Any();
    }

    /// <summary>
    /// Determines if the given type is decorated with an 'Invariant'-alike attributes and, if
    /// so, returns the first one in the out argument.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="at"></param>
    /// <returns></returns>
    static bool HasInvariantAttribute(
        ITypeSymbol type,
        [NotNullWhen(true)] out AttributeData? at)
    {
        var found = HasInvariantAttributes(type, out var ats);
        at = found ? ats.First() : null;
        return found;
    }

    // ----------------------------------------------------

    const string RETURNTYPE = "ReturnType";

    /// <summary>
    /// Determines if the given attribute carries a 'ReturnType' setting. If so, returns it and
    /// whether it is a nullable one in the out argument.
    /// </summary>
    public static bool HasReturnType(
        AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
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

    // ----------------------------------------------------

    /// <summary>
    /// Gets the appropriate options to print the given return type, based upon whether it is the
    /// same as the given host one, or not.
    /// </summary>
    static EasyTypeOptions ReturnOptions(INamedTypeSymbol rtype, INamedTypeSymbol host)
    {
        return SymbolEqualityComparer.Default.Equals(host, rtype)
            ? EasyTypeOptions.Default
            : EasyTypeOptions.Full with { NullableStyle = EasyNullableStyle.None };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given methods are the same, or not, for the sole purposes of this
    /// generator.
    /// </summary>
    bool SameMethod(MethodInfo method, IMethodSymbol existing)
    {
        var mname = method.Name;
        var ename = existing.Name;
        if (mname != ename) return false; // Named differ...

        var mpars = method.GetParameters();
        var epars = existing.Parameters;
        if (mpars.Length != epars.Length) return false; // Not the same number of parameters...

        for (int i = 0; i < mpars.Length; i++)
        {
            var mpar = mpars[i];
            var epar = epars[i];
            if (!SameArgument(mpar, epar)) return false; // Not the same argument...
        }

        return true; // Same method...
    }

    /// <summary>
    /// Determines if the two given parameters are the same, for the sole purposes of this generator.
    /// We only consider their respective types and modifiers, not their names.
    /// </summary>
    bool SameArgument(ParameterInfo mpar, IParameterSymbol epar)
    {
        // HIGH: In 'SameArgument()' also validate the 'out' modifier (and others?)

        var mtype = mpar.ParameterType;
        var etype = (INamedTypeSymbol)epar.Type;

        return SameType(mtype, etype);
    }

    /// <summary>
    /// Determines if the two given parameter types are the same, for the sole purposes of this
    /// generator.
    /// </summary>
    bool SameType(Type mtype, INamedTypeSymbol etype)
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
            var same = SameType(marg, (INamedTypeSymbol)earg);
            if (!same) return false;
        }

        return true;
    }
}