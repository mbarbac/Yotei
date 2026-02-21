namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Tries to find the value of the 'UseVirtual' setting at the given type, if it is not null,
    /// or at the types in the given chains, in that order. Uses either the attribute found at a
    /// decorated member, or the one decorating a suitable host. If found, returns its value in
    /// the out argument along with in which member (or null if any) and host type that attribute
    /// was found.
    /// </summary>
    public static bool FindUseVirtual<T>(
        string membername,
        out bool value,
        out T? member,
        out INamedTypeSymbol? host,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains) where T : ISymbol
    {
        var found = Finder.Find((type, out info) =>
        {
            // Members take priority...
            if (FindDecoratedMember<T>(membername, out var member, out var attr, type) &&
                attr.HasUseVirtual(out var value))
            {
                info = new(value, member, type);
                return true;
            }

            // Otherwise at type's level...
            if (type.HasInheritsWithAttribute(out var at) &&
                at.HasUseVirtual(out value))
            {
                info = new(value, default, type);
                return true;
            }

            // Try next...
            info = null!;
            return false;
        },
        out FindUseVirtualInfo<T> info, type, chains);

        value = found && info.Value;
        member = found ? info.Member : default;
        host = found ? info.Host : default;
        return found;
    }
    
    record FindUseVirtualInfo<T>(bool Value, T? Member, INamedTypeSymbol? Host) where T : ISymbol;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the 'ReturnType' setting at the given type, if it is not null,
    /// or at the types in the given chains, in that order. Uses either the attribute found at a
    /// decorated member, or the one decorating a suitable host. If found, returns its value in
    /// the out argument along with in which member (or null if any) and host type that attribute
    /// was found.
    /// </summary>
    public static bool FindReturnType<T>(
        string membername,
        [NotNullWhen(true)] out INamedTypeSymbol? value,
        out bool nullable,
        out INamedTypeSymbol? host,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains) where T : ISymbol
    {
        var found = Finder.Find((type, out info) =>
        {
            // Members take priority...
            if (FindDecoratedMember<T>(membername, out var member, out var attr, type) &&
                attr.HasReturnType(out var value, out var nullable))
            {
                info = new(value, nullable, type);
                return true;
            }

            // Otherwise at type's level...
            if (type.HasInheritsWithAttribute(out var at) &&
                at.HasReturnType(out value, out nullable))
            {
                info = new(value, nullable, type);
                return true;
            }

            // Try next...
            info = null!;
            return false;
        },
        out FindReturnTypeInfo<T> info, type, chains);

        value = found ? info.Value : default;
        nullable = found && info.Nullable;
        host = found ? info.Host : default;
        return found;
    }
    
    record FindReturnTypeInfo<T>(
        INamedTypeSymbol Value, bool Nullable, INamedTypeSymbol? Host)
        where T : ISymbol;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate options to emit the return type, based upon whether it is the
    /// same as the given host one, or not.
    /// </summary>
    public static EasyTypeSymbol ReturnOptions(INamedTypeSymbol hosttype, INamedTypeSymbol rtype)
    {
        return SymbolEqualityComparer.Default.Equals(hosttype, rtype)
            ? EasyTypeSymbol.Default
            : EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a property or field member with the given name in the given type, if it is
    /// not null, or in the types in the given chains, in that order.
    /// </summary>
    public static bool FindMember<T>(
        string membername,
        [NotNullWhen(true)] out T? value,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains) where T : ISymbol
    {
        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<T>().FirstOrDefault(x => x.Name == membername);
            return value != null;
        },
        out value, type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a decorated property or field member with the given name in the given type,
    /// if it is not null, or in the types in the given chains, in that order.
    /// </summary>
    public static bool FindDecoratedMember<T>(
        string membername,
        [NotNullWhen(true)] out T? value,
        [NotNullWhen(true)] out AttributeData? attr,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains) where T : ISymbol
    {
        var found = Finder.Find((type, out info) =>
        {
            var member = type.GetMembers().OfType<T>().FirstOrDefault(x => x.Name == membername);
            if (member != null)
            {
                var ats = member.GetAttributes([typeof(WithAttribute), typeof(WithAttribute<>)]).ToArray();
                if (ats.Length == 1)
                {
                    info = new(member, ats[0]);
                    return true;
                }
            }

            info = null!;
            return false;
        },
        out FindMemberInfo<T> info, type, chains);

        value = found ? info.Member : default;
        attr = found ? info.Attribute : default;
        return found;
    }
    record FindMemberInfo<T>(T Member, AttributeData Attribute) where T : ISymbol;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a method with the given name and parameter type in the given type, if it is
    /// not null, or in the types in the given chains, in that order.
    /// </summary>
    public static bool FindMethod(
        string methodname,
        ITypeSymbol paramtype,
        [NotNullWhen(true)] out IMethodSymbol? value,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        methodname = methodname.NotNullNotEmpty(trim: true);
        ArgumentNullException.ThrowIfNull(paramtype);

        return Finder.Find((type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == methodname &&
                x.Parameters.Length == 1 &&
                paramtype.IsAssignableTo(x.Parameters[0].Type));

            return value != null;
        },
        out value, type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given attribute carries a 'UseVirtual' value and, if so, returns it in
    /// the out arguments.
    /// </summary>
    public static bool HasUseVirtual(
        this AttributeData at,
        out bool value)
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
    /// Determines if the given attribute carries a 'ReturnType' value and, if so, returns it and
    /// whether it is a nullable one in the out arguments.
    /// </summary>
    public static bool HasReturnType(
        this AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at);
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

        // Default...
        value = null;
        nullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> attribute or
    /// with a <see cref="WithAttribute{T}"/> one, and if so returns it in the out argument.
    /// </summary>
    public static bool HasWithAttribute<T>(
        this T member,
        [NotNullWhen(true)] out AttributeData? value) where T : ISymbol
    {
        value = member.GetAttributes([typeof(WithAttribute)]).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes([typeof(WithAttribute<>)]).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is decorated with a <see cref="InheritsWithAttribute"/> or
    /// with a <see cref="InheritsWithAttribute{T}"/> attribute and, if so, returns it in the out
    /// argument.
    /// </summary>
    public static bool HasInheritsWithAttribute(
        this INamedTypeSymbol type,
        [NotNullWhen(true)] out AttributeData? value)
    {
        value = type.GetAttributes([typeof(InheritsWithAttribute)]).FirstOrDefault();
        if (value is not null) return true;

        value = type.GetAttributes([typeof(InheritsWithAttribute<>)]).FirstOrDefault();
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
    public static void EmitDocumentation(ISymbol symbol, CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the '<see langword="with"/>' keyword for the '{{symbol.Name}}' member.
        /// </summary>
        {{AttributeDoc}}
        """);
}