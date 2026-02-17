namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Tries to find the 'UseVirtual' named argument on the given attribute.
    /// Is found, returns its value in the out argument.
    /// </summary>
    public static bool FindUseVirtualAt(AttributeData at, out bool value)
    {
        if (at.FindNamedArgument("UseVirtual", out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }
        value = false;
        return false;
    }

    /// <summary>
    /// Tries to find value of the first 'UseVirtual' property that happens in attributes of the given
    /// type, or types of the given chains, that correspond to a member with the given name.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FinderUseVirtual(
        out bool value,
        string name,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            // Member attribute takes priority...
            var member = type.GetMembers().FirstOrDefault(x => x.Name == name);
            if (member != null)
            {
                var atmember = member.GetAttributes().FirstOrDefault(x =>
                    x.AttributeClass != null &&
                    x.AttributeClass.MatchAny([typeof(WithAttribute), typeof(WithAttribute<>)]));

                if (atmember != null && FindUseVirtualAt(atmember, out value)) return true;
            }

            // Otherwise, inherit attribute...
            var athost = type.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass != null &&
                x.AttributeClass.MatchAny([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]));

            if (athost != null && FindUseVirtualAt(athost, out value)) return true;

            // Try next...
            value = default!;
            return false;
        },
        out value, type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find either the 'ReturnType' named argument on the given attribute, or its unique
    /// generic type, as appropriate. If found, returns its value in the out argument.
    /// </summary>
    public static bool FindReturnTypeAt(
        AttributeData at, [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        // Generic attribute...
        if (at.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)at.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out nullable);
            return true;
        }

        // Not-generic attribute...
        else if (at.AttributeClass.Arity == 0)
        {
            if (at.FindNamedArgument("ReturnType", out var arg))
            {
                if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                {
                    value = temp.UnwrapNullable(out nullable);
                    return true;
                }
            }
        }

        // Defaul...
        value = null;
        nullable = false;
        return false;
    }

    /// <summary>
    /// Tries to find value of the first 'ReturnType' property that happens in attributes of the given
    /// type, or types of the given chains, that correspond to a member with the given name.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FinderReturnType(
        out INamedTypeSymbol? value, out bool nullable,
        string name,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        var found = Finder.Find((type, out item) =>
        {
            // Member attribute takes priority...
            var member = type.GetMembers().FirstOrDefault(x => x.Name == name);
            if (member != null)
            {
                var atmember = member.GetAttributes().FirstOrDefault(x =>
                    x.AttributeClass != null &&
                    x.AttributeClass.MatchAny([typeof(WithAttribute), typeof(WithAttribute<>)]));

                if (atmember != null && FindReturnTypeAt(atmember, out var mtype, out var mnullable))
                { item = new(mtype, mnullable); return true; }
            }

            // Otherwise, inherit attribute...
            var athost = type.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass != null &&
                x.AttributeClass.MatchAny([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]));

            if (athost != null && FindReturnTypeAt(athost, out var htype, out var hnullable))
            { item = new(htype, hnullable); return true; }

            // Try next...
            item = new(null, false);
            return false;
        },
        out ReturnTypeFound item, type, chains);

        value = found ? item.Type : null;
        nullable = found ? item.Nullable : false;
        return found;
    }
    record struct ReturnTypeFound(INamedTypeSymbol? Type, bool Nullable);

    // ----------------------------------------------------

    /// <summary>
    /// Finds the <see cref="InheritsWithAttribute"/> or the <see cref="InheritsWithAttribute{T}"/>
    /// attribute at the given type (if not null) and at the types of the given arrays, in that
    /// order.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FinderInheritsWithAttribute(
        [NotNullWhen(true)] out AttributeData? value,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetAttributes([typeof(InheritsWithAttribute)]).FirstOrDefault();
            if (value is not null) return true;

            value = type.GetAttributes([typeof(InheritsWithAttribute<>)]).FirstOrDefault();
            if (value is not null) return true;

            value = default;
            return false;
        },
        out value, type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds a member with the given name in the given type (if not null) or in the types of the
    /// given arrays, in that order, that is decorated with the <see cref="WithAttribute"/> or the
    /// <see cref="WithAttribute{T}"/> attribute.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FinderWithAttribute(
        [NotNullWhen(true)] out AttributeData? value, [NotNullWhen(true)] out ISymbol? symbol,
        string name,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        name = name.NotNullNotEmpty(true);

        var found = Finder.Find((type, out item) =>
        {
            var props = type.GetMembers(name).OfType<IPropertySymbol>();
            foreach (var prop in props)
            {
                if (FindWithAttributeAt(prop, out var value))
                {
                    item.data = value;
                    item.member = prop;
                    return true;
                }
            }

            var fields = type.GetMembers(name).OfType<IFieldSymbol>();
            foreach (var field in fields)
            {
                if (FindWithAttributeAt(field, out var value))
                {
                    item.data = value;
                    item.member = field;
                    return true;
                }
            }

            item = default;
            return false;
        },
        out (AttributeData? data, ISymbol? member) item, type, chains);

        value = found ? item.data : null;
        symbol = found ? item.member : null;
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> or with
    /// a <see cref="WithAttribute{T}"/> attribute and, if so, returns the found one in the out
    /// argument.
    /// </summary>
    /// <param name="member"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool FindWithAttributeAt(
        IPropertySymbol member, [NotNullWhen(true)] out AttributeData? value)
    {
        value = member.GetAttributes([typeof(WithAttribute)]).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes([typeof(WithAttribute<>)]).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }

    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> or with
    /// a <see cref="WithAttribute{T}"/> attribute and, if so, returns the found one in the out
    /// argument.
    /// </summary>
    public static bool FindWithAttributeAt(
        IFieldSymbol member, [NotNullWhen(true)] out AttributeData? value)
    {
        value = member.GetAttributes([typeof(WithAttribute)]).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes([typeof(WithAttribute<>)]).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The version of the generator for documentation purposes.
    /// </summary>
    static string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    /// <summary>
    /// A string with the 'GeneratedCode' attribute of the generator for documentation purposes.
    /// </summary>
    static string AttributeDoc => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits documentation.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="cb"></param>
    public static void EmitDocumentation(ISymbol symbol, CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the '<see langword="with"/>' keyword for the '{{symbol.Name}}' member.
        /// </summary>
        {{AttributeDoc}}
        """);
}