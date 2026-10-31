#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class ConvertToExtensions
{
    /// <summary>
    /// Tries to convert the given source value into an instance of the given target type.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool TryConvertTo<TSource, TTarget>(
        [AllowNull] this TSource source,
        [MaybeNull] out TTarget target,
        IFormatProvider? provider = null)
    {
        Type sourceType = source is null ? typeof(TSource) : source.GetType();
        Type targetType = typeof(TTarget);

        if (source is null) // From null souce...
        {
            target = default;
            return targetType.IsNullable || UnWrapIfNullable(targetType).IsNullable;
        }

        var converter = GetConverter(sourceType, targetType); // Trying a standard converter...
        if (converter is not null)
        {
            try { target = (TTarget)converter.DynamicInvoke(source)!; return true; }
            catch { }
        }

        if (targetType == typeof(string)) // Try ToString methods...
        {
            if (provider is not null)
            {
                var method = GetToStringWithFormatProvider(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, [provider])!; return true; }
                    catch { }
                }
            }
            else
            {
                var method = GetToStringStandard(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, null)!; return true; }
                    catch { }
                }
            }
        }

        if (sourceType == typeof(string)) // Try Parse methods...
        {
            if (provider is not null)
            {
                var method = GetTryParseWithFormatProvider(targetType);
                if (method is not null)
                {
                    try
                    {
                        var pars = new object?[] { source, provider, null };
                        var done = method.Invoke(null, pars);

                        if (done is bool value && value)
                        {
                            target = (TTarget)pars[2]!;
                            return true;
                        }
                    }
                    catch { }
                }
            }
            else
            {
                var method = GetTryParseStandard(targetType);
                if (method is not null)
                {
                    try
                    {
                        var pars = new object?[] { source, null };
                        var done = method.Invoke(null, pars);

                        if (done is bool value && value)
                        {
                            target = (TTarget)pars[1]!;
                            return true;
                        }
                    }
                    catch { }
                }
            }
        }

        // No conversion found...
        target = default;
        return false;
    }

    /// <summary>
    /// Converts the given source value into an instance of the given target type, or throws an
    /// exception if such conversion is not possible.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TSource, TTarget>(
        [AllowNull] this TSource source,
        IFormatProvider? provider = null)
    {
        if (TryConvertTo<TSource, TTarget>(source, out var target, provider)) return target;
        ThrowCastException(source, typeof(TSource), typeof(TTarget));
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value into an instance of the given target type.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool TryConvertTo<TTarget>(
        this object? source,
        [MaybeNull] out TTarget target,
        IFormatProvider? provider = null)
    {
        Type sourceType = source is null ? null! : source.GetType();
        Type targetType = typeof(TTarget);

        if (source is null) // From null souce...
        {
            target = default;
            return targetType.IsNullable || UnWrapIfNullable(targetType).IsNullable;
        }

        var converter = GetConverter(sourceType, targetType); // Trying a standard converter...
        if (converter is not null)
        {
            try { target = (TTarget)converter.DynamicInvoke(source)!; return true; }
            catch { }
        }

        if (targetType == typeof(string)) // Try ToString methods...
        {
            if (provider is not null)
            {
                var method = GetToStringWithFormatProvider(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, [provider])!; return true; }
                    catch { }
                }
            }
            else
            {
                var method = GetToStringStandard(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, null)!; return true; }
                    catch { }
                }
            }
        }

        if (sourceType == typeof(string)) // Try Parse methods...
        {
            if (provider is not null)
            {
                var method = GetTryParseWithFormatProvider(targetType);
                if (method is not null)
                {
                    try
                    {
                        var pars = new object?[] { source, provider, null };
                        var done = method.Invoke(null, pars);

                        if (done is bool value && value)
                        {
                            target = (TTarget)pars[2]!;
                            return true;
                        }
                    }
                    catch { }
                }
            }
            else
            {
                var method = GetTryParseStandard(targetType);
                if (method is not null)
                {
                    try
                    {
                        var pars = new object?[] { source, null };
                        var done = method.Invoke(null, pars);

                        if (done is bool value && value)
                        {
                            target = (TTarget)pars[1]!;
                            return true;
                        }
                    }
                    catch { }
                }
            }
        }

        // No conversion found...
        target = default;
        return false;
    }

    /// <summary>
    /// Converts the given source value into an instance of the given target type, or throws an
    /// exception if such conversion is not possible.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TTarget>(
        [AllowNull] this object? source,
        IFormatProvider? provider = null)
    {
        if (TryConvertTo<TTarget>(source, out var target, provider)) return target;

        if (source is null) ThrowCastException(source, typeof(TTarget));
        else ThrowCastException(source, source.GetType(), typeof(TTarget));
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value into an instance of the given target type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="targetType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool TryConvertTo(
        this object? source,
        [MaybeNull] out object? target,
        Type targetType,
        IFormatProvider? provider = null)
    {
        Type sourceType = source is null ? null! : source.GetType();
        ArgumentNullException.ThrowIfNull(targetType);

        if (source is null) // From null souce...
        {
            target = default;
            return targetType.IsNullable || UnWrapIfNullable(targetType).IsNullable;
        }

        var converter = GetConverter(sourceType, targetType); // Trying a standard converter...
        if (converter is not null)
        {
            try { target = converter.DynamicInvoke(source)!; return true; }
            catch { }
        }

        if (targetType == typeof(string)) // Try ToString methods...
        {
            if (provider is not null)
            {
                var method = GetToStringWithFormatProvider(sourceType);
                if (method is not null)
                {
                    try { target = method.Invoke(source, [provider])!; return true; }
                    catch { }
                }
            }
            else
            {
                var method = GetToStringStandard(sourceType);
                if (method is not null)
                {
                    try { target = method.Invoke(source, null)!; return true; }
                    catch { }
                }
            }
        }

        if (sourceType == typeof(string)) // Try Parse methods...
        {
            if (provider is not null)
            {
                var method = GetTryParseWithFormatProvider(targetType);
                if (method is not null)
                {
                    try
                    {
                        var pars = new object?[] { source, provider, null };
                        var done = method.Invoke(null, pars);

                        if (done is bool value && value)
                        {
                            target = pars[2]!;
                            return true;
                        }
                    }
                    catch { }
                }
            }
            else
            {
                var method = GetTryParseStandard(targetType);
                if (method is not null)
                {
                    try
                    {
                        var pars = new object?[] { source, null };
                        var done = method.Invoke(null, pars);

                        if (done is bool value && value)
                        {
                            target = pars[1]!;
                            return true;
                        }
                    }
                    catch { }
                }
            }
        }

        // No conversion found...
        target = default;
        return false;
    }

    /// <summary>
    /// Converts the given source value into an instance of the given target type, or throws an
    /// exception if such conversion is not possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static object? ConvertTo(
        [AllowNull] this object? source,
        Type targetType,
        IFormatProvider? provider = null)
    {
        if (TryConvertTo(source, out var target, targetType, provider)) return target;

        if (source is null) ThrowCastException(source, targetType);
        else ThrowCastException(source, source.GetType(), targetType);
        return default;
    }

    // ----------------------------------------------------
    static readonly Dictionary<Type, Dictionary<Type, Delegate?>> Converters = [];

    /// <summary>
    /// Returns the delegate to invoke to convert instances of the source type to instances of
    /// the target one, or null if any.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static Delegate? GetConverter(Type sourceType, Type targetType)
    {
        lock (Converters)
        {
            // We always need a first-level entry for the source type...
            if (!Converters.TryGetValue(sourceType, out var childs))
                Converters.Add(sourceType, childs = []);

            // We may already have found a converter (or null)...
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

            // General case...
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

            // Conversion not possible...
            childs.Add(targetType, null);
            return null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either the underlying not-nullable type, if the given one is nullable, or the
    /// original type otherwise.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static Type UnWrapIfNullable(Type type)
    {
        if (type.IsGenericType)
        {
            var temp = type.GetGenericTypeDefinition();
            if (temp == typeof(Nullable<>))
            {
                var args = type.GetGenericArguments();
                if (args.Length == 1) type = UnWrapIfNullable(args[0]);
            }
        }
        return type;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    [DoesNotReturn]
    static void ThrowCastException(object? value, Type targetType)
    {
        throw new InvalidCastException(
            "Cannot convert the given source value to the given target type.")
            .WithData(value)
            .WithData(targetType);
    }

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    [DoesNotReturn]
    static void ThrowCastException(object? value, Type sourceType, Type targetType)
    {
        throw new InvalidCastException(
            "Cannot convert from the given source type to the given target one.")
            .WithData(value)
            .WithData(sourceType)
            .WithData(targetType);
    }

    // ----------------------------------------------------
    static readonly Dictionary<Type, MethodInfo?> ToStringStandard = [];

    /// <summary>
    /// Gets the standard '<c>ToString()</c>' method declared at the given type, or null if any.
    /// </summary>
    static MethodInfo? GetToStringStandard(Type type)
    {
        if (ToStringStandard.TryGetValue(type, out var temp)) return temp;

        var methods = type.GetMethods().Where(static x => x.Name == "ToString");
        foreach (var method in methods)
        {
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 0) continue;

            ToStringStandard.Add(type, method);
            return method;
        }
        return null;
    }

    // ----------------------------------------------------
    static readonly Dictionary<Type, MethodInfo?> ToStringWithFormatProviders = [];

    /// <summary>
    /// Gets the standard '<c>ToString(IFormatProvider)</c>' method declared at the given type, or
    /// null if any.
    /// </summary>
    static MethodInfo? GetToStringWithFormatProvider(Type type)
    {
        if (ToStringWithFormatProviders.TryGetValue(type, out var temp)) return temp;

        var methods = type.GetMethods().Where(static x => x.Name == "ToString");
        foreach (var method in methods)
        {
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 1) continue;
            if (!pars[0].ParameterType.IsAssignableTo(typeof(IFormatProvider))) continue;

            ToStringWithFormatProviders.Add(type, method);
            return method;
        }
        return null;
    }

    // ----------------------------------------------------
    static readonly Dictionary<Type, MethodInfo?> TryParserStandard = [];

    /// <summary>
    /// Gets the standard '<c>TryParse(string, out Type)</c>' method declared at the given type, or
    /// null if any.
    /// </summary>
    static MethodInfo? GetTryParseStandard(Type type)
    {
        if (TryParserStandard.TryGetValue(type, out var temp)) return temp;

        var methods = type.GetMethods().Where(static x => x.Name == "TryParse");
        foreach (var method in methods)
        {
            if (!method.IsStatic) continue;
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 2) continue;
            if (pars[0].ParameterType != typeof(string)) continue;
            if (!pars[1].IsOut) continue;

            var name = pars[1].ParameterType.FullName;
            if (name != $"{type.FullName}&") continue;

            TryParserStandard.Add(type, method);
            return method;
        }
        return null;
    }

    // ----------------------------------------------------
    static readonly Dictionary<Type, MethodInfo?> TryParseWithFormatProvider = [];

    /// <summary>
    /// Gets the standard '<c>TryParse(string, IFormatProvider, out Type)</c>' method declared at the given type, or
    /// null if any.
    /// </summary>
    static MethodInfo? GetTryParseWithFormatProvider(Type type)
    {
        if (TryParseWithFormatProvider.TryGetValue(type, out var temp)) return temp;

        var methods = type.GetMethods().Where(static x => x.Name == "TryParse");
        foreach (var method in methods)
        {
            if (!method.IsStatic) continue;
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 3) continue;
            if (pars[0].ParameterType != typeof(string)) continue;
            if (!pars[1].ParameterType.IsAssignableTo(typeof(IFormatProvider))) continue;
            if (!pars[2].IsOut) continue;

            var name = pars[2].ParameterType.FullName;
            if (name != $"{type.FullName}&") continue;

            TryParseWithFormatProvider.Add(type, method);
            return method;
        }
        return null;
    }
}