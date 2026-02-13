namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Tries to find the 'UseVirtual' named argument on the given attribute.
    /// Is found, returns its value in the out argument.
    /// </summary>
    public static bool FindUseVirtual(AttributeData at, out bool value)
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

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find either the 'ReturnType' named argument on the given attribute, or its unique
    /// generic type, as appropriate.
    /// Is found, returns its value in the out argument.
    /// </summary>
    public static bool FindReturnType(
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
    public static bool FindInheritsWithAttribute(
        [NotNullWhen(true)] out AttributeData? value,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetAttributes(typeof(InheritsWithAttribute)).FirstOrDefault();
            if (value is not null) return true;

            value = type.GetAttributes(typeof(InheritsWithAttribute<>)).FirstOrDefault();
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
    public static bool FindWithAttribute(
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
                if (FindWithAttribute(prop, out var value))
                {
                    item.data = value;
                    item.member = prop;
                    return true;
                }
            }

            var fields = type.GetMembers(name).OfType<IFieldSymbol>();
            foreach (var field in fields)
            {
                if (FindWithAttribute(field, out var value))
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
    public static bool FindWithAttribute(
        IPropertySymbol member, [NotNullWhen(true)] out AttributeData? value)
    {
        value = member.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes(typeof(WithAttribute<>)).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }

    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> or with
    /// a <see cref="WithAttribute{T}"/> attribute and, if so, returns the found one in the out
    /// argument.
    /// </summary>
    public static bool FindWithAttribute(
        IFieldSymbol member, [NotNullWhen(true)] out AttributeData? value)
    {
        value = member.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes(typeof(WithAttribute<>)).FirstOrDefault();
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits documentation.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="cb"></param>
    public static void EmitDocumentation(ISymbol symbol, CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the 'with' keyword for the '{{symbol.Name}}' member.
        /// </summary>
        {{AttributeDoc}}
        """);
}