using System.Net.Mail;

namespace Yotei.Tools.WithGenerator;

// ========================================================
public interface IXNode<T> where T : ISymbol
{
    public T Symbol { get; }
}

// ========================================================
public static class XNode
{
    const string USEVIRTUAL = "UseVirtual";
    const string RETURNTYPE = "ReturnType";

    // ----------------------------------------------------

    extension<T>(IXNode<T> node) where T : ISymbol
    {
        public string MemberName => node.Symbol.Name;

        public ITypeSymbol MemberType => node.Symbol switch
        {
            IPropertySymbol item => item.Type,
            IFieldSymbol item => item.Type,
            _ => throw new UnExpectedException("Invalid node kind.").WithData(node)
        };

        public string MethodName => $"With{node.MemberName}";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is decorated with a <see cref="WithAttribute"/> attribute
    /// and, if so, returns it in the out argument
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="at"></param>
    /// <returns></returns>
    public static bool HasWithAttribute(
        this ISymbol symbol,
        [NotNullWhen(true)] out AttributeData? at)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        var ats = symbol.GetAttributes([typeof(WithAttribute)]);
        at = ats.FirstOrDefault();
        return at != null;
    }

    /// <summary>
    /// Determines if the given symbol is decorated with a <see cref="InheritsWithAttribute"/>
    /// attribute and, if so, returns it in the out argument
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="at"></param>
    /// <returns></returns>
    public static bool HasInheritsWithAttribute(
        this ISymbol symbol,
        [NotNullWhen(true)] out AttributeData? at)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        var ats = symbol.GetAttributes([typeof(InheritsWithAttribute)]);
        at = ats.FirstOrDefault();
        return at != null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.UseVirtual"/> setting on the given
    /// attribute. If so, returns it in the out argument.
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

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.ReturnType"/> setting on the given
    /// attribute. If so, returns it in, and whether it is nullable or nor, in the out arguments.
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

        if (at.FindNamedArgument(RETURNTYPE, out var arg) &&
                !arg.Value.IsNull &&
                arg.Value.Value is INamedTypeSymbol temp)
        {
            value = temp.UnwrapNullable(out nullable);
            return true;
        }

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
    /// Tries to find a valid method in either the given host type, if not null, or in any of the
    /// types in the given chains, in order. The method is found by a matching the given name and
    /// validating that the requested agument is compatible with the method's one. If found, returns
    /// it in the out argument.
    /// </summary>
    public static bool TryFindMethod(
        string methodname, ITypeSymbol argtype,
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
        [NotNullWhen(true)] out IMethodSymbol? value)
    {
        methodname = methodname.NotNullNotEmpty(true);
        ArgumentNullException.ThrowIfNull(argtype);

        return Finder.Find(type, chains, out value, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == methodname &&
                x.Parameters.Length == 1 &&
                argtype.IsAssignableTo(x.Parameters[0].Type));

            return value != null;
        });
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the member whose name is given in the given host type, if not null, or in
    /// any of the types in the given chains, in order. If found, returns it, and the associated
    /// attribute, in the out arguments.
    /// </summary>
    public static bool TryFindMember<T>(
        string membername,
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
        [NotNullWhen(true)] out T? value,
        [NotNullWhen(true)] out AttributeData? at) where T : ISymbol
    {
        membername = membername.NotNullNotEmpty(true);

        var found = Finder.Find(
            type, chains, out (T? Member, AttributeData Attr) info, (type, out info) =>
            {
                var member = type.GetMembers().OfType<T>().FirstOrDefault(x => x.Name == membername);
                if (member != null &&
                    member.HasWithAttribute(out var at))
                {
                    info = new(member, at);
                    return true;
                }

                info = new();
                return false;
            });

        value = found ? info.Member : default;
        at = found ? info.Attr : default;
        return found;
    }
}