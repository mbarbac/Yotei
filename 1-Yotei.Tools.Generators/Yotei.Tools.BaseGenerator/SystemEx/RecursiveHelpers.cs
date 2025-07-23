namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class RecursiveHelpers
{
    /// <summary>
    /// Tries to obtain the requested value using the given delegate. The chains of its base types
    /// and direct or all implemented interfaces are also used if such is explicitly requested.
    /// Returns the requested value if so, or <c>null</c> otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="func"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <param name="allifaces"></param>
    /// <returns></returns>
    public static T? Recursive<T>(
        this ITypeSymbol type,
        Func<ITypeSymbol, T?> func,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        type.ThrowWhenNull();
        func.ThrowWhenNull();

        // Delegate on the type itself...
        var found = func(type);

        // Delegate on the base types...
        if (found is null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                found = func(child);
                if (found is not null) break;
            }
        }

        // Delegate on the interfaces...
        if (found is null && (ifaces || allifaces))
        {
            var iter = ifaces ? type.Interfaces : type.AllInterfaces;
            foreach (var child in iter)
            {
                found = func(child);
                if (found is not null) break;
            }
        }

        return found;
    }

    /// <summary>
    /// Tries to determine if the requested value can be obtained using the given delegate. The
    /// chains of its base types and direct or all implemented interfaces are also used if such
    /// is explicitly requested. Returns <c>true</c> and sets the out argument if so, or returns
    /// <c>false</c> otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="func"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <param name="allifaces"></param>
    /// <returns></returns>
    public static bool RecursiveBool<T>(
        this ITypeSymbol type, out T? value,
        BoolDelegate<T> func,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        type.ThrowWhenNull();
        func.ThrowWhenNull();

        // Delegate on the type itself...
        var found = func(type, out value);

        // Delegate on the base types...
        if (!found && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                found = func(child, out value);
                if (found) break;
            }
        }

        // Delegate on the interfaces...
        if (!found && (ifaces || allifaces))
        {
            var iter = ifaces ? type.Interfaces : type.AllInterfaces;
            foreach (var child in iter)
            {
                found = func(child, out value);
                if (found) break;
            }
        }

        return found;
    }

    /// <summary>
    /// The delegate for <see cref="RecursiveBool{T}(ITypeSymbol, out T?, BoolDelegate{T}, bool, bool, bool)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool BoolDelegate<T>(ITypeSymbol type, out T? value);
}