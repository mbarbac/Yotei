namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal static class XNode
{
    /*
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
    /// <returns></returns>*/

    public static bool FindMethod(
        this INamedTypeSymbol type,
        bool useHost,
        [NotNullWhen(true)] out IMethodSymbol? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();

        var name = nameof(ICloneable.Clone);

        return type.Finder<IMethodSymbol>(useHost, (type, out value) =>
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

        return type.Finder<AttributeData?>(useHost, (type, out value) =>
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
}