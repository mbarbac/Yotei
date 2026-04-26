namespace Yotei.ORM.Generators.InvariantGenerator;

// ========================================================
partial class XTypeNode
{
    /// <summary>
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
    /// Emits the appropriate documentation for the given method.
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
    /// Determines if the given symbol is decorated with any invariant collection attribute and,
    /// if so, returns them in the out argument.
    /// </summary>
    static bool HasInvariantAttributes(
        ISymbol symbol,
        out IEnumerable<AttributeData> attributes)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        attributes = symbol.GetAttributes([
            typeof(IInvariantBagAttribute), typeof(IInvariantBagAttribute<>),
            typeof(IInvariantListAttribute), typeof(IInvariantListAttribute<,>), typeof(IInvariantListAttribute<>),
            typeof(InvariantBagAttribute), typeof(InvariantBagAttribute<>),
            typeof(InvariantListAttribute), typeof(InvariantListAttribute<,>), typeof(InvariantListAttribute<>),]);

        return attributes.Any();
    }

    /// <summary>
    /// Determines if the given symbol is decorated with just one invariant attribute and, if so,
    /// returns it in the out argument. If many arguments are found, then this method returns false
    /// but set the <paramref name="hasmany"/> argument to true.
    /// </summary>
    static bool HasInvariantAttribute(
        ISymbol symbol,
        [NotNullWhen(true)] out AttributeData? at, out bool hasmany)
    {
        var r = HasInvariantAttributes(symbol, out var items);
        hasmany = items.Skip(1).Any();
        at = !hasmany && r ? items.First() : null;
        return r;
    }

    // ----------------------------------------------------

    const string RETURNTYPE = "ReturnType";

    /// <summary>
    /// Tries to find the value of the 'ReturnType' setting on the given attribute. If so, returns
    /// it in the out argument along with whether it is a nullable one or not.
    /// </summary>
    static bool HasReturnType(
        AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at);
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        if (at.FindNamedArgument(RETURNTYPE, out var arg) &&
            !arg.Value.IsNull &&
            arg.Value.Value is INamedTypeSymbol temp)
        {
            value = temp.UnwrapNullable(out nullable);
            return true;
        }

        value = null;
        nullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the appropriate options to emit the given type, based upon whether it is the same as
    /// the other given one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    static EasyTypeOptions GetReturnOptions(
        INamedTypeSymbol type, INamedTypeSymbol other)
    {
        return SymbolEqualityComparer.Default.Equals(type, other)
            ? EasyTypeOptions.Default
            : EasyTypeOptions.Full with { NullableStyle = EasyNullableStyle.None };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines, for the sole purposes of this generator, if the two given elements shall be
    /// considered the same, or not.
    /// </summary>
    bool SameMethod(MethodInfo method, IMethodSymbol existing)
    {
        var mname = method.Name;
        var ename = existing.Name;
        if (mname != ename) return false;

        var mpars = method.GetParameters();
        var epars = existing.Parameters;
        if (mpars.Length != epars.Length) return false;

        for (int i = 0; i < mpars.Length; i++)
        {
            var mpar = mpars[i];
            var epar = epars[i];
            if (!SameArgument(mpar, epar)) return false;
        }

        return true;
    }

    /// <summary>
    /// Determines, for the sole purposes of this generator, if the two given elements shall be
    /// considered the same, or not.
    /// </summary>
    bool SameArgument(ParameterInfo mpar, IParameterSymbol epar)
    {
        var mtype = mpar.ParameterType;
        var etype = (INamedTypeSymbol)epar.Type;

        return SameType(mtype, etype);
    }

    /// <summary>
    /// Determines, for the sole purposes of this generator, if the two given elements shall be
    /// considered the same, or not.
    /// </summary>
    bool SameType(Type mtype, INamedTypeSymbol etype)
    {
        var comparer = SymbolEqualityComparer.Default;

        switch (mtype.Name)
        {
            case "K": if (!comparer.Equals(KType, etype)) return false; break;
            case "T": if (!comparer.Equals(TType, etype)) return false; break;
        }

        // Trivial cases...
        if (etype.IsNamespace) return false;
        if (etype.Kind == SymbolKind.TypeParameter) return true;
        if (mtype.IsGenericParameter) return true;

        // Capturing...
        var eargs = etype.TypeArguments;
        var margs = mtype.GenericTypeArguments.Length != 0
            ? mtype.GenericTypeArguments
            : mtype is System.Reflection.TypeInfo info ? info.GenericTypeParameters : [];

        if (eargs.Length != margs.Length) return false; // shortcut...

        // Names...
        var ename = etype.Name;
        var mname = mtype.Name;
        var index = mname.IndexOf('`');
        if (index >= 0) mname = mname[..index];

        if (ename.EndsWith("Attribute")) ename = ename.RemoveLast("Attribute").ToString();
        if (mname.EndsWith("Attribute")) mname = mname.RemoveLast("Attribute").ToString();
        if (ename != mname) return false;

        // Hierarchy...
        var ehost = etype.ContainingType;
        var mhost = mtype.DeclaringType;

        if (ehost is null && mhost is null) // Namespaces...
        {
            var sspace = etype.ContainingNamespace?.ToString() ?? string.Empty;
            var tspace = mtype.Namespace ?? string.Empty;
            if (sspace != tspace) return false;
        }
        else if (ehost is null || mhost is null) return false;

        if (ehost is not null && mhost is not null) // Nested types...
        {
            if (!ehost.Match(mhost)) return false;
        }
        else if (ehost is not null || mhost is not null) return false;

        // Generic arguments...
        var count = eargs.Length;
        for (int i = 0; i < count; i++)
        {
            var earg = eargs[i];
            var marg = margs[i];
            if (!SameType(marg, (INamedTypeSymbol)earg)) return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the generic KT arguments with the appropriate ones.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
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
}