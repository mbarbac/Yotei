#pragma warning disable IDE0075

namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Tries to find a parameterless 'Clone()' method, either in the given type or in the first
    /// one in the given chains.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="useHost"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindMethod(
        this INamedTypeSymbol type,
        bool useHost,
        [NotNullWhen(true)] out IMethodSymbol? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();

        var name = nameof(ICloneable.Clone);

        return type.Finder(useHost, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == name &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

            return value is not null;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value of the <see cref="CloneableAttribute.ReturnType"/> setting from
    /// the given attribute. If it is a generic one, then the value extracted is the one of its 
    /// unique type argument.
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
            var name = nameof(CloneableAttribute.ReturnType);

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

    /// <summary>
    /// Tries to extract the value of the <see cref="CloneableAttribute.UseVirtual"/> setting from
    /// the given attribute.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetUseVirtual(this AttributeData data, out bool value)
    {
        var name = nameof(CloneableAttribute.UseVirtual);

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
    /// Tries to find a <see cref="CloneableAttribute"/> or <see cref="CloneableAttribute{T}"/>
    /// attribute in the given type, or in the first one in the given chains.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="useHost"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindCloneableAttribute(
        this INamedTypeSymbol type,
        bool useHost,
        [NotNullWhen(true)] out AttributeData? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();
        value = null;

        return type.Finder(useHost, static (type, out value) =>
        {
            value = type.GetAttributes(typeof(CloneableAttribute)).FirstOrDefault();
            if (value != null) return true;

            value = type.GetAttributes(typeof(CloneableAttribute<>)).FirstOrDefault();
            if (value != null) return true;

            value = null;
            return false;
        },
        out value, chains);
    }

    extension(XTypeNode node)
    {
        // ------------------------------------------------

        /// <summary>
        /// Gets the appropriate method modifiers when the host instance is an interface one.
        /// Returns '<c>null</c>' if any can be obtained or are not needed. If not null, a space
        /// separator is added to the returned string.
        /// </summary>
        /// <returns></returns>
        public string? GetInterfaceModifiers()
        {
            var found = node.Symbol.Finder<string?>(false, static (parent, out value) =>
            {
                // If existing or requested...
                if (parent.FindMethod(true, out _) ||
                    parent.FindCloneableAttribute(true, out _))
                {
                    value = "new ";
                    return true;
                }

                // Not found, try next...
                value = null;
                return false;
            },
            out var value, node.Symbol.AllInterfaces);
            if (found) return value;

            // Default...
            return null;
        }

        // ------------------------------------------------

        /// <summary>
        /// Gets the appropriate method modifiers when the host instance is an abstract one.
        /// Returns '<c>null</c>' if any can be obtained or are not needed. If not null, a space
        /// separator is added to the returned string.
        /// </summary>
        /// <returns></returns>
        /// Base Method     Derived Class   Modifier
        /// --------------- --------------- ---------------
        /// abstract        abstract        abstract override / override
        /// abstract        abstract        abstract override / override
        /// --------------- --------------- ----------------
        /// concrete-virt   abstract        abstract override
        /// concrete-nonv   abstract        abstract new
        /// --------------- --------------- ----------------
        public string? GetAbstractModifiers()
        {
            var found = node.Symbol.Finder<string?>(false, static (parent, out value) =>
            {
                value = null;

                // Existing parent method...
                if (parent.FindMethod(true, out var method))
                {
                    var dec = method.DeclaredAccessibility;
                    if (dec == Accessibility.Private) return false;
                    var str = dec.EasyName(addspace: true) ?? "public ";

                    if (parent.IsInterface) { value = $"{str}abstract "; return true; }

                    var basevirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                    value = !basevirt ? $"{str}abstract new " : $"{str}abstract override ";
                    return true;
                }

                // Element requested...
                if (parent.FindCloneableAttribute(true, out var at))
                {
                    if (parent.IsInterface) { value = $"public abstract "; return true; }

                    var basevirt = at.GetUseVirtual(out var temp) ? temp : true;
                    value = !basevirt ? "public abstract new " : "public abstract override ";
                    return true;
                }

                // Not found, try next...
                return false;
            },
            out var value, node.Symbol.AllBaseTypes, node.Symbol.AllInterfaces);
            if (found) return value;

            // Default...
            return "public abstract ";
        }

        // ------------------------------------------------

        /// <summary>
        /// Gets the appropriate method modifiers when the host instance is a regular one.
        /// Returns '<c>null</c>' if any can be obtained or are not needed. If not null, a space
        /// separator is added to the returned string.
        /// </summary>
        /// <returns></returns>
        /// Base Method  Derived Class   Sealed  Modifier
        /// ------------ --------------- ------- -----------
        /// Not-virt     Not-virt        No      new
        /// Not-virt     virt            No      new virtual
        /// Not-virt     Not-virt        Yes     new
        /// Not-virt     virt            Yes     new
        /// ------------ --------------- ------- -----------
        /// Virt         Not-virt        No      new
        /// Virt         virt            No      override
        /// Virt         Not-virt        Yes     override
        /// Virt         virt            Yes     override
        /// ------------ --------------- ------- -----------
        public string? GetRegularModifiers(SourceProductionContext _)
        {
            var nodevirt = node.UseVirtual;
            var nodesealed = node.Symbol.IsSealed;

            var found = node.Symbol.Finder<string?>(false, (parent, out value) =>
            {
                value = null;

                // Existing parent method...
                if (parent.FindMethod(true, out var method))
                {
                    var dec = method.DeclaredAccessibility;
                    if (dec == Accessibility.Private) return false;
                    var str = dec.EasyName(addspace: true) ?? "public ";

                    if (parent.IsInterface)
                    {
                        value = !nodevirt || nodesealed ? str : $"{str}virtual ";
                        return true;
                    }

                    var basevirt = method.IsVirtual || method.IsAbstract || method.IsOverride;
                    if (!basevirt)
                    {
                        value = nodevirt && !nodesealed ? $"{str}new virtual " : $"{str}new ";
                        return true;
                    }
                    else
                    {
                        value = !nodevirt && !nodesealed ? $"{str}new " : $"{str}override ";
                        return true;
                    }
                }

                // Element requested...
                if (parent.FindCloneableAttribute(true, out var at))
                {
                    if (parent.IsInterface)
                    {
                        value = !nodevirt || nodesealed ? "public " : "public virtual ";
                        return true;
                    }

                    var basevirt = at.GetUseVirtual(out var temp) ? temp : true;
                    if (!basevirt)
                    {
                        value = nodevirt && !nodesealed ? "public new virtual " : "public new ";
                        return true;
                    }
                    else
                    {
                        value = !nodevirt && !nodesealed ? "public new " : "public override ";
                        return true;
                    }
                }

                // Not found, try next...
                return false;
            },
            out var value, node.Symbol.AllBaseTypes, node.Symbol.AllInterfaces);
            if (found) return value;

            // Default...
            return !nodevirt || nodesealed ? "public " : "public virtual ";
        }
    }
}