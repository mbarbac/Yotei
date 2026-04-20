namespace Yotei.Tools.CloneGenerator;

// ========================================================
public static class Helpers
{
    public const string USEVIRTUAL = "UseVirtual";
    public const string RETURNTYPE = "ReturnType";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is decorated with any <see cref="CloneableAttribute"/>
    /// attribute and, if so, returns them in the out argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static bool HasCloneableAttributes(
        this ISymbol symbol,
        out IEnumerable<AttributeData> attributes)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        attributes = symbol.GetAttributes([typeof(CloneableAttribute), typeof(CloneableAttribute<>)]);
        return attributes.Any();
    }

    /// <summary>
    /// Determines if the given symbol is decorated with just one <see cref="CloneableAttribute"/>
    /// attribute and, if so, returns it in the out argument. If many arguments are found, then
    /// this method returns false but set the <paramref name="hasmany"/> argument to true.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="at"></param>
    /// <param name="hasmany"></param>
    /// <returns></returns>
    public static bool HasCloneableAttribute(
        this ISymbol symbol,
        [NotNullWhen(true)] out AttributeData? at, out bool hasmany)
    {
        var r = symbol.HasCloneableAttributes(out var items);
        hasmany = items.Skip(1).Any();
        at = !hasmany && r ? items.First() : null;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.UseVirtual"/> setting on the
    /// given attribute. If so, returns it in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool HasUseVirtual(this AttributeData at, out bool value)
    {
        ArgumentNullException.ThrowIfNull(at);

        if (at.FindNamedArgument(USEVIRTUAL, out var arg) &&
                !arg.Value.IsNull &&
                arg.Value.Value is bool temp)
        {
            value = temp;
            return true;
        }

        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.ReturnType"/> setting on the
    /// given attribute. If so, returns it in the out argument along with whether it is a nullable
    /// one or not.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static bool HasReturnType(
        this AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at);
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        // Non-generic attribute...
        if (at.AttributeClass.Arity == 0)
        {
            if (at.FindNamedArgument(RETURNTYPE, out var arg) &&
                !arg.Value.IsNull &&
                arg.Value.Value is INamedTypeSymbol temp)
            {
                value = temp.UnwrapNullable(out nullable);
                return true;
            }
        }

        // Non-generic attribute...
        else if (at.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)at.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out nullable);
            return true;
        }

        // Invalid arity...
        else throw new ArgumentException("Invalid arity.").WithData(at);

        // Not found...
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
    public static EasyTypeOptions GetReturnOptions(
        this INamedTypeSymbol type, INamedTypeSymbol other)
    {
        return SymbolEqualityComparer.Default.Equals(type, other)
            ? EasyTypeOptions.Default
            : EasyTypeOptions.Full with { NullableStyle = EasyNullableStyle.None };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a valid method in either the given host type, or in any of the types in the
    /// given chains, in order. If found, returns it in the out argument.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryFindMethod(
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
        [NotNullWhen(true)] out IMethodSymbol? value)
    {
        return Finder.Find(type, chains, out value, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

            return value != null;
        });
    }
}