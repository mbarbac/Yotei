namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Represents working data for either a property or a field node.
/// </summary>
internal interface IXNode : INode
{
    /// <summary>
    /// Determines if this instance was created to represent an inherited member or not.
    /// </summary>
    public bool IsInherited { get; }

    /// <summary>
    /// The return type for the generated method.
    /// </summary>
    public INamedTypeSymbol ReturnType { get; set; }

    /// <summary>
    /// Whether the return type for the generated method is a nullable one, or not.
    /// </summary>
    public bool ReturnNullable { get; set; }

    /// <summary>
    /// The options to use to obtain the easy name of the return type of the generated method.
    /// </summary>
    public EasyNameOptions ReturnOptions { get; set; }

    /// <summary>
    /// Whether the generated method should be a virtual-alike one, or not.
    /// </summary>
    public bool UseVirtual { get; set; }
}

// ========================================================
internal static class XNode
{
    extension(IXNode node)
    {
        /// <summary>
        /// The host of this instance in the source code generation hierarchy.
        /// <br/> In some scenarios, the value of this property MIGHT NOT be the containing type.
        /// </summary>
        public INamedTypeSymbol Host
            => node is XPropertyNode prop ? prop.Host : ((XFieldNode)node).Host;

        // ------------------------------------------------

        /// <summary>
        /// Captures relevant working data at 'Emit' time.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CaptureEmit(SourceProductionContext context)
        {
            node.ReturnType = node.Host;
            node.ReturnNullable = false;
            node.ReturnOptions = EasyNameOptions.Default;
            node.UseVirtual = true;

            // Scenario: should be inherited member...
            if (node.Attributes.Length == 0)
            {
                if (!node.IsInherited)
                {
                    CoreDiagnostics.NoAttributes(node.Symbol, $"Node for '{node.Symbol.Name}' is not an inherited member.").Report(context);
                    CoreDiagnostics.NoAttributes(node.Host, $"Node for '{node.Symbol.Name}' is not an inherited member.").Report(context);
                    return false;
                }

                if (!node.Host.FindInheritWithsAttribute(true, out var at))
                {
                    CoreDiagnostics.NoAttributes(node.Host, $"Cannot find '{nameof(InheritWithsAttribute)}'.").Report(context);
                    return false;
                }

                if (at.GetReturnType(out var type, out var nullable))
                {
                    node.ReturnType = type;
                    node.ReturnNullable = nullable;

                    var same = SymbolEqualityComparer.Default.Equals(node.Host, type);
                    if (!same) node.ReturnOptions = EasyNameOptions.Full with
                    { TypeUseNullable = false };
                }

                if (at.GetUseVirtual(out var temp)) node.UseVirtual = temp;

                return true;
            }

            // Scenario: captured member...
            else if (node.Attributes.Length == 1)
            {
                var at = node.Attributes[0];

                if (at.GetReturnType(out var type, out var nullable))
                {
                    node.ReturnType = type;
                    node.ReturnNullable = nullable;

                    var same = SymbolEqualityComparer.Default.Equals(node.Host, type);
                    if (!same) node.ReturnOptions = EasyNameOptions.Full with
                    { TypeUseNullable = false };
                }

                if (at.GetUseVirtual(out var temp)) node.UseVirtual = temp;

                return true;
            }

            // Invalid member...
            else
            {
                CoreDiagnostics.TooManyAttributes(node.Symbol).Report(context);
                return false;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Gets the appropriate method modifiers when the host instance is an interface one, or
        /// '<c>null</c>' if any can be obtained or are not needed. If not null, a space separator
        /// is added to the returned string.
        /// </summary>
        /// <returns></returns>
        public string? GetInterfaceModifiers() => throw null;

        // ------------------------------------------------

        /// <summary>
        /// Gets the appropriate method modifiers when the host instance is an abstract one, or
        /// '<c>null</c>' if any can be obtained or are not needed. If not null, a space separator
        /// is added to the returned string.
        /// </summary>
        /// <returns></returns>
        public string? GetAbstractModifiers() => throw null;

        // ------------------------------------------------

        /// <summary>
        /// Gets the appropriate method modifiers when the host instance is a regular one, or
        /// '<c>null</c>' if any can be obtained or are not needed. If not null, a space separator
        /// is added to the returned string.
        /// </summary>
        /// <returns></returns>
        public string? GetRegularModifiers() => throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the appropriate documentation for the generated code of the given symbol.
    /// </summary>
    public static void EmitDocumentation(ISymbol item, CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the 'with' keyword for the '{{item.Name}}' member.
        /// </summary>
        {{WithGenerator.AttributeDoc}}
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value of the <see cref="WithAttribute.ReturnType"/> setting from the
    /// given attribute. If it is a generic one, then the value extracted is the one of its unique
    /// type argument.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static bool GetReturnType(
        this AttributeData data,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        // Generic attribute...
        if (data.AttributeClass!.Arity == 1)
        {
            value = (INamedTypeSymbol)data.AttributeClass!.TypeArguments[0];
            value = value.UnwrapNullable(out nullable);
            return true;
        }

        // Not-generic attribute...
        else if (data.AttributeClass!.Arity == 0)
        {
            var name = "ReturnType";

            if (data.GetNamedArgument(name, out var arg))
            {
                if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                {
                    value = temp.UnwrapNullable(out nullable);
                    return true;
                }
            }
        }

        // Default and error conditions...
        value = default;
        nullable = default;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value of the <see cref="WithAttribute.UseVirtual"/> setting from the
    /// given attribute.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetUseVirtual(this AttributeData data, out bool value)
    {
        var name = "UseVirtual";

        if (data.GetNamedArgument(name, out var arg))
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
    /// Tries to find a method with the given 'With[Name]'-alike name and whose unique argument is
    /// assignable to the given one, either in the given type or in the first type in the given
    /// chains.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="name"></param>
    /// <param name="argtype"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindMethod(
        this INamedTypeSymbol type,
        bool usehost,
        string name,
        ITypeSymbol argtype,
        [NotNullWhen(true)] out IMethodSymbol? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();
        argtype.ThrowWhenNull();
        name = name.NotNullNotEmpty(true);

        if (!name.StartsWith("With")) throw new ArgumentException(
            "Method name must start with 'With'.")
            .WithData(name);

        var found = type.Finder<IMethodSymbol>(usehost, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == name &&
                x.Parameters.Length == 1 &&
                argtype.IsAssignableTo(x.Parameters[0].Type));

            return value is not null;
        },
        out value, chains);
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member decorated with the <see cref="WithAttribute"/> in the given type,
    /// or in the first type in the given chains, with the given name. If it is a property,  then
    /// it must not be an indexed one.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindDecoratedMember(
        this INamedTypeSymbol type,
        bool usehost,
        string name,
        [NotNullWhen(true)] out ISymbol? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();
        name = name.NotNullNotEmpty(true);

        var found = type.Finder<ISymbol>(usehost, (type, out value) =>
        {
            var items = type.GetMembers();
            foreach (var item in items)
            {
                if (item is not IPropertySymbol and not IFieldSymbol) continue;
                if (item.Name != name) continue;
                if (item is IPropertySymbol temp && temp.Parameters.Length != 0) continue;
                if (!item.HasAttributes(typeof(WithAttribute)) &&
                    !item.HasAttributes(typeof(WithAttribute<>))) continue;

                value = item;
                return true;
            }

            value = null!;
            return false;
        },
        out value, chains);
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member decorated with the <see cref="WithAttribute"/> in the given type,
    /// or in the first type in the given chains, with the given name. If it is a property,  then
    /// it must not be an indexed one. If found, returns the decorating attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindWithAttribute(
        this INamedTypeSymbol type,
        bool usehost,
        string name,
        [NotNullWhen(true)] out AttributeData? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();

        return type.Finder(usehost, (INamedTypeSymbol type, out AttributeData value) =>
        {
            var found = type.FindDecoratedMember(true, name, out var member);
            if (found)
            {
                value = member!.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
                if (value is not null) return true;

                value = member!.GetAttributes(typeof(WithAttribute<>)).FirstOrDefault();
                if (value is not null) return true;
            }

            value = null!;
            return false;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a member decorated with the <see cref="InheritWithsAttribute"/> in the given
    /// type, or in the first type in the given chains. If found, returns the decorating attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindInheritWithsAttribute(
        this INamedTypeSymbol type,
        bool usehost,
        [NotNullWhen(true)] out AttributeData? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();

        return type.Finder(usehost, (INamedTypeSymbol type, out AttributeData value) =>
        {
            value = type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();
            if (value != null) return true;

            value = type.GetAttributes(typeof(InheritWithsAttribute<>)).FirstOrDefault();
            if (value != null) return true;

            value = null!;
            return false;
        },
        out value, chains);
    }
}