namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Tries to find a 'Clone' method at the given type, if it is not null, or at the types in
    /// the given chains, in that order. If found, returns its value in the out argument.
    /// </summary>
    public static bool FindMethod(
        [NotNullWhen(true)] out IMethodSymbol? value,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return Finder.Find(out value, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

            return value != null;
        },
        type, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the 'ReturnType' setting at the given type, if it is not null,
    /// or at the types in the given chains, in that order. If found, returns its value and the
    /// host where it was found in the out arguments.
    /// </summary>
    public static bool FindReturnType(
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable,
        [NotNullWhen(true)] out INamedTypeSymbol? host,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        var found = Finder.Find((type, out info) =>
        {
            var ats = type.GetAttributes([typeof(CloneableAttribute), typeof(CloneableAttribute<>)]);
            foreach (var at in ats)
            {
                if (at.HasReturnType(out var temp, out var nullable))
                {
                    info = new(temp, nullable, type);
                    return true;
                }
            }

            // Try next...
            info = null!;
            return false;
        },
        out FindReturnTypeInfo info, type, chains);

        value = found ? info.Value : null;
        nullable = found && info.Nullable;
        host = found ? info.Host : null;
        return found;
    }

    record FindReturnTypeInfo(INamedTypeSymbol Value, bool Nullable, INamedTypeSymbol? Host);

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
    /// Tries to find the value of the 'UseVirtual' setting at the given type, if it is not null,
    /// or at the types in the given chains, in that order. If found, returns its value and the
    /// host where it was found in the out arguments.
    /// </summary>
    public static bool FindUseVirtual(
        out bool value,
        [NotNullWhen(true)] out INamedTypeSymbol? host,
        INamedTypeSymbol? type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        var found = Finder.Find((type, out info) =>
        {
            var ats = type.GetAttributes([typeof(CloneableAttribute), typeof(CloneableAttribute<>)]);
            foreach (var at in ats)
            {
                if (at.HasUseVirtual(out var temp))
                {
                    info = new(temp, type);
                    return true;
                }
            }

            // Try next...
            info = null!;
            return false;
        },
        out FindUseVirtualInfo info, type, chains);

        value = found && info.Value;
        host = found ? info.Host : null;
        return found;
    }

    record FindUseVirtualInfo(bool Value, INamedTypeSymbol? Host);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given attribute carries a 'ReturnType' value and, if so, returns it in
    /// the out argument.
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
    /// Determines if the given attribute carries a 'UseVirtual' value and, if so, returns it in
    /// the out argument.
    /// </summary>
    public static bool HasUseVirtual(this AttributeData at, out bool value)
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
    /// Determines if the given member is decorated with a <see cref="CloneableAttribute"/>
    /// attribute or with a <see cref="CloneableAttribute{T}"/> one, and if so returns it in the
    /// out argument.
    /// </summary>
    public static bool HasCloneableAttribute(
        this INamedTypeSymbol type,
        [NotNullWhen(true)] out AttributeData? value)
    {
        value = type.GetAttributes([typeof(CloneableAttribute)]).FirstOrDefault();
        if (value is not null) return true;

        value = type.GetAttributes([typeof(CloneableAttribute<>)]).FirstOrDefault();
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
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits documentation.
    /// </summary>
    public static void EmitDocumentation(ISymbol symbol, CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{AttributeDoc}}
        """);
}