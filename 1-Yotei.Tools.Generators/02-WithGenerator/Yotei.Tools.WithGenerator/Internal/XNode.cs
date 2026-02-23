namespace Yotei.Tools.WithGenerator;

// ========================================================
internal interface IXNode<T> where T : ISymbol
{
    public T Symbol { get; }
}

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> or with a
    /// <see cref="WithAttribute{T}"/> attribute and, if so, returns the first one found.
    /// </summary>
    public static bool HasWithAttribute(
        this ISymbol member, [NotNullWhen(true)] out AttributeData? at)
    {
        ArgumentNullException.ThrowIfNull(member);

        var ats = member.GetAttributes([typeof(WithAttribute), typeof(WithAttribute<>)]).ToArray();
        at = ats.Length > 0 ? ats[0] : null;
        return ats.Length > 0;
    }

    /// <summary>
    /// Determines if the given type is decorated with a <see cref="InheritsWithAttribute"/> or
    /// with a <see cref="InheritsWithAttribute{T}"/> attribute and, if so, returns the first one
    /// found.
    /// </summary>
    public static bool HasInheritsWithAttribute(
        this INamedTypeSymbol type, [NotNullWhen(true)] out AttributeData? at)
    {
        ArgumentNullException.ThrowIfNull(type);

        var ats = type.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]).ToArray();
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

    extension<T>(IXNode<T> node) where T : ISymbol
    {
        public string SymbolName => node.Symbol.Name;

        public INamedTypeSymbol SymbolType => node.Symbol is IPropertySymbol prop
            ? (INamedTypeSymbol)prop.Type
            : (INamedTypeSymbol)((IFieldSymbol)node.Symbol).Type;

        public string MethodName => $"With{node.SymbolName}";

        // ------------------------------------------------

        /// <summary>
        /// The version of the generator for documentation purposes.
        /// </summary>
        public string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// A string with the 'GeneratedCode' attribute of the generator for documentation purposes.
        /// </summary>
        public string AttributeDoc => $$"""
            [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{node.VersionDoc}}")]
            """;

        /// <summary>
        /// Emits documentation.
        /// </summary>
        /// <param name="cb"></param>
        public void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
            /// <summary>
            /// Emulates the '<see langword="with"/>' keyword for the '{{node.SymbolName}}' member.
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
            var name = node.MethodName;
            var argtype = node.SymbolType;

            return Finder.Find(out value, (type, out value) =>
            {
                value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                    x.Name == name &&
                    x.Parameters.Length == 1 &&
                    argtype.IsAssignableTo(x.Parameters[0].Type));

                return value != null;
            },
            type, chains);
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to find a decorate member in either the given type, if it is not null, or in any
        /// type in the given chains, in that order. If found, returns it along with the first
        /// found attribute in the out arguments.
        /// </summary>
        public bool FindMember(
            [NotNullWhen(true)] out T? value, [NotNullWhen(true)] out AttributeData? at,
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
        {
            var name = node.SymbolName;

            var found = Finder.Find(out (T? Member, AttributeData? Attribute) info, (type, out info) =>
            {
                var member = type.GetMembers().OfType<T>().FirstOrDefault(x => x.Name == name);
                if (member != null &&
                    member.HasWithAttribute(out var at))
                {
                    info = new(member, at);
                    return true;
                }

                // Try next...
                info = new();
                return false;
            },
            type, chains);

            value = found ? info.Member : default;
            at = found ? info.Attribute : default;
            return found;
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to find the value of the 'UseVirtual' setting in either the given type, if it is
        /// not null, or in any type in the given chains, in that order. If found, returns it in
        /// the out argument, along with in which member it was found, if any, and/or in which
        /// host type it was found.
        /// </summary>
        public bool FindUseVirtual(
            out bool value, out T? member, out INamedTypeSymbol? host,
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
        {
            var found = Finder.Find(
                out (bool Value, T? Member, INamedTypeSymbol? Host) info, (type, out info) =>
            {
                // Member found...
                if (node.FindMember(out var member, out var at, type, []) &&
                    at.HasUseVirtual(out var value))
                {
                    info = new(value, member, type);
                    return true;
                }

                // Method found (not considering interfaces: virtual-alike by default)...
                if (!type.IsInterface &&
                    node.FindMethod(out var method, type, []))
                {
                    value = method.IsVirtual || method.IsOverride || method.IsAbstract;
                    info = new(value, default, type);
                    return true;
                }

                // At type's level...
                if (type.HasInheritsWithAttribute(out at) &&
                    at.HasUseVirtual(out value))
                {
                    info = new(value, default, type);
                    return true;
                }

                // Try next...
                info = new();
                return false;
            },
            type, chains);

            value = found && info.Value;
            member = found ? info.Member : default;
            host = found ? info.Host : default;
            return found;
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to find the value of the 'ReturnType' setting in either the given type, if it is
        /// not null, or in any type in the given chains, in that order. If found, returns it and
        /// whether it is a nullable one in the out arguments.
        /// </summary>
        public bool FindReturnType(
            [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable,
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains)
        {
            var found = Finder.Find(
                out (INamedTypeSymbol? Value, bool Nullable) info, (type, out info) =>
                {
                    // Member found...
                    if (node.FindMember(out var _, out var at, type, []) &&
                        at.HasReturnType(out var value, out var nullable))
                    {
                        info = new(value, nullable);
                        return true;
                    }

                    // Method found...
                    if (node.FindMethod(out var method, type, []))
                    {
                        value = ((INamedTypeSymbol)method.ReturnType).UnwrapNullable(out nullable);
                        info = new(value, nullable);
                        return true;
                    }

                    // At type's level...
                    if (type.HasInheritsWithAttribute(out at) &&
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