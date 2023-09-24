namespace Yotei.Tools;

// ========================================================
public static partial class ConvertToExtensions
{
    /// <summary>
    /// Gets the instance 'ToString(IFormatProvider)' method, or null if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static MethodInfo? GetFormattedToString(Type type)
    {
        var methods = type.GetMethods().Where(x => x.Name == "ToString");
        foreach (var method in methods)
        {
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 1) continue;
            if (!pars[0].ParameterType.IsAssignableTo(typeof(IFormatProvider))) continue;

            return method;
        }
        return null;
    }

    /// <summary>
    /// Gets the instance 'ToString()' method, or null if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static MethodInfo? GetRegularToString(Type type)
    {
        var methods = type.GetMethods().Where(x => x.Name == "ToString");
        foreach (var method in methods)
        {
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length == 0) return method;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the static 'TryParse(string, IFormatProvider, out Type value)' method, or null.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static MethodInfo? GetFormattedParser(Type type)
    {
        var methods = type.GetMethods().Where(x => x.Name == "TryParse");
        foreach (var method in methods)
        {
            if (!method.IsStatic) continue;
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 3) continue;
            if (pars[0].ParameterType != typeof(string)) continue;
            if (!pars[1].ParameterType.IsAssignableTo(typeof(IFormatProvider))) continue;
            if (!pars[2].IsOut) continue;

            var temp = pars[2].ParameterType.FullName;
            if (temp != $"{type.FullName}&") continue;

            return method;
        }
        return null;
    }

    /// <summary>
    /// Gets the static 'TryParse(string, out Type value)' method, or null.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static MethodInfo? GetRegularParser(Type type)
    {
        var methods = type.GetMethods().Where(x => x.Name == "TryParse");
        foreach (var method in methods)
        {
            if (!method.IsStatic) continue;
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 2) continue;
            if (pars[0].ParameterType != typeof(string)) continue;
            if (!pars[1].IsOut) continue;

            var temp = pars[1].ParameterType.FullName;
            if (temp != $"{type.FullName}&") continue;

            return method;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds a converter from a nullable source to a target type.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static Delegate? NullSourceToTarget(Type sourceType, Type targetType)
    {
        if (!targetType.IsNullable()) return null;

        var sourceCore = GetCoreType(sourceType);
        var targetCore = GetCoreType(targetType);

        return GetConverter(sourceCore, targetCore);
    }

    /// <summary>
    /// Gets the core type of the given one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static Type GetCoreType(Type type)
    {
        if (type.IsGenericType)
        {
            var temp = type.GetGenericTypeDefinition();
            if (temp == typeof(Nullable<>))
            {
                var args = type.GetGenericArguments();
                if (args.Length == 1)
                    type = GetCoreType(args[0]);
            }
        }
        return type;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    static void ThrowCastException(object? source, Type targetType)
    {
        if (source is null) throw new InvalidCastException(
            "Cannot convert the source value to the given target type.")
             .WithData(source)
            .WithData(targetType);

        ThrowCastException(source, source.GetType(), targetType);
    }

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    static void ThrowCastException(object? source, Type sourceType, Type targetType)
    {
        throw new InvalidCastException(
            "Cannot convert the source value to the given target type.")
            .WithData(source)
            .WithData(sourceType)
            .WithData(targetType);
    }

    // ----------------------------------------------------

    static readonly Dictionary<Type, Dictionary<Type, Delegate?>> Dict = new();

    /// <summary>
    /// Returns a delegate that can convert values of a given source type to a given target one.
    /// Returns null if such conversion is not possible.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static Delegate? GetConverter(Type sourceType, Type targetType)
    {
        lock (Dict)
        {
            // We need a source type entry, even an empty one...
            if (!Dict.TryGetValue(sourceType, out var childs))
                Dict.Add(sourceType, childs = new());

            // Returning an existing converter...
            if (childs.TryGetValue(targetType, out var converter)) return converter;

            // Converter for the trivial case...
            if (sourceType == targetType)
            {
                var input = Expression.Parameter(sourceType);
                var expr = Expression.Lambda(input, input);

                converter = expr.Compile();
                childs[targetType] = converter;
                return converter;
            }

            // Conversion converter...
            try
            {
                var input = Expression.Parameter(sourceType);
                var body = Expression.Convert(input, targetType);
                var expr = Expression.Lambda(body, input);

                converter = expr.Compile();
                childs[targetType] = converter;
                return converter;
            }
            catch { }

            // Finishing...
            childs.Add(targetType, null);
            return null;
        }
    }
}