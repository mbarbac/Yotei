namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal interface IXNode
{
    public INamedTypeSymbol Symbol { get; }
}

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Determines if the given type is decorated with a <see cref="CloneableAttribute"/> or with
    /// a <see cref="CloneableAttribute{T}"/> attribute and, if so, returns the first one found.
    /// </summary>
    public static bool HasCloneableAttribute(
        this INamedTypeSymbol type, [NotNullWhen(true)] out AttributeData? at)
    {
        ArgumentNullException.ThrowIfNull(type);

        var ats = type.GetAttributes([typeof(CloneableAttribute), typeof(CloneableAttribute<>)]).ToArray();
        at = ats.Length > 0 ? ats[0] : null;
        return ats.Length > 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given attribute carries a 'UseVirtual' value and, if so, returns it in
    /// the out argument.
    /// </summary>
    public static bool HasUseVirtual(this AttributeData at, out bool value)
    {
        ArgumentNullException.ThrowIfNull(at);

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
    /// Determines if the given attribute carries a 'ReturnType' value and, if so, returns it and
    /// whether it is a nullable one or not the out arguments.
    /// </summary>
    public static bool HasReturnType(
        this AttributeData at, [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
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

        // Not allowed arity or not found...
        value = null;
        nullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the appropriate options to print this return type based upon if it is the same as
    /// the given host one, or not.
    /// </summary>
    public static EasyTypeSymbol ReturnOptions(this INamedTypeSymbol rtype, INamedTypeSymbol host)
    {
        return SymbolEqualityComparer.Default.Equals(host, rtype)
            ? EasyTypeSymbol.Default
            : EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None };
    }

    // ====================================================

    extension(IXNode node)
    {
        /// <summary>
        /// The version of the generator for documentation purposes.
        /// </summary>
        public string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// A string with the 'GeneratedCode' attribute of the generator for documentation purposes.
        /// </summary>
        public string AttributeDoc => $$"""
            [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{node.VersionDoc}}")]
            """;

        /// <summary>
        /// Emits documentation.
        /// </summary>
        /// <param name="cb"></param>
        public void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
            /// <summary>
            /// <inheritdoc cref="ICloneable.Clone"/>
            /// </summary>
            {{node.AttributeDoc}}
            """);

        // ------------------------------------------------

        /// <summary>
        /// Tries to find a suitable method in either the given type, if it is not null, or in any
        /// type in the given chains, in that order. If found, returns it in the out argument.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="chains"></param>
        /// <returns></returns>
        public bool FindMethod(
            [NotNullWhen(true)] out IMethodSymbol value,
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
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

        // ------------------------------------------------

        /// <summary>
        /// Tries to find the value of the 'UseVirtual' setting in either the given type, if it is
        /// not null, or in any type in the given chains, in that order. If found, returns it and
        /// in what host type it was found in the out arguments.
        /// </summary>
        public bool FindUseVirtual(
            out bool value, [NotNullWhen(true)] out INamedTypeSymbol? host,
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
        {
            var found = Finder.Find(
                out (bool Value, INamedTypeSymbol? Host) info, (type, out info) =>
                {
                    bool value;

                    // Existing method (not considering interfaces, are virtual-alike by default)...
                    if (!type.IsInterface &&
                        node.FindMethod(out var method, type, []))
                    {
                        value = method.IsVirtual || method.IsOverride || method.IsAbstract;
                        info = new(value, type);
                        return true;
                    }

                    // Requested...
                    if (type.HasCloneableAttribute(out var at) &&
                        at.HasUseVirtual(out value))
                    {
                        info = new(value, type);
                        return true;
                    }

                    // Try next...
                    info = new();
                    return false;
                },
                type, chains);

            value = found && info.Value;
            host = found ? info.Host : null;
            return found;
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to find the value of the 'ReturnType' setting in either the given type, if it is
        /// not null, or in any type in the given chains, in that order. If found, returns it and
        /// in what host type it was found in the out arguments.
        /// </summary>
        public bool FindReturnType(
            [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable,
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
        {
            var found = Finder.Find(
                out (INamedTypeSymbol? Value, bool Nullable) info, (type, out info) =>
                {
                    INamedTypeSymbol? value;
                    bool nullable;

                    // Intercepting special case...
                    if (type.IsInterface && type.Name == "ICloneable")
                    {
                        info = new();
                        return false;
                    }

                    // Existing method...
                    if (node.FindMethod(out var method, type, []))
                    {
                        value = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out nullable);
                        info = new(value, nullable);
                        return true;
                    }

                    // Requested...
                    if (type.HasCloneableAttribute(out var at) &&
                        at.HasReturnType(out value, out nullable))
                    {
                        info = new(value, nullable);
                        return true;
                    }

                    // Try next...
                    info = new();
                    return false;
                },
                type, chains);

            value = found ? info.Value : null;
            nullable = found && info.Nullable;
            return found;
        }
    }
}