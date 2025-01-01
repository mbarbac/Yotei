namespace Yotei.Tools;

// ========================================================
public static class ConvertToExtensions
{
    /// <summary>
    /// Tries to convert the given source value to the given target type.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool TryConvertTo<TSource, TTarget>(
        [AllowNull] this TSource source,
        [MaybeNull] out TTarget target)
    {
        Type sourceType = source is null ? typeof(TSource) : source.GetType();
        Type targetType = typeof(TTarget);

        // Setting the out return value in advance...
        target = default!;

        // Null source...
        if (source is null)
            return GetConverterFromNull(sourceType, targetType) != null;

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try
            {
                target = (TTarget)converter.DynamicInvoke(source)!;
                return true;
            }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            var method = GetRegularToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = (TTarget)method.Invoke(source, null)!;
                    return true;
                }
                catch { }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            var method = GetRegularParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = (TTarget)pars[1]!;
                        return true;
                    }
                }
                catch { }
            }
        }

        // Conversion not found...
        return false;
    }

    /// <summary>
    /// Converts the given source value to the given target type, or throws an exception if that
    /// conversion is not possible.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TSource, TTarget>([AllowNull] this TSource source)
    {
        if (TryConvertTo<TSource, TTarget>(source, out var target)) return target;

        ThrowCastException(source, typeof(TSource), typeof(TTarget));
        return default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value to the given target type.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool TryConvertTo<TTarget>(
        this object? source,
        [MaybeNull] out TTarget target)
    {
        Type sourceType = source is null ? null! : source.GetType();
        Type targetType = typeof(TTarget);

        target = default!;

        // Null source...
        if (source is null)
            return targetType.IsNullable() || TryUnwrapNullableType(targetType).IsNullable();

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try
            {
                target = (TTarget)converter.DynamicInvoke(source)!;
                return true;
            }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            var method = GetRegularToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = (TTarget)method.Invoke(source, null)!;
                    return true;
                }
                catch { }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            var method = GetRegularParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = (TTarget)pars[1]!;
                        return true;
                    }
                }
                catch { }
            }
        }

        // Conversion not found...
        return false;
    }

    /// <summary>
    /// Converts the given source value to the given target type, or throws an exception if that
    /// conversion is not possible.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TTarget>(this object? source)
    {
        if (TryConvertTo<TTarget>(source, out var target)) return target;

        if (source is null) ThrowCastException(source, typeof(TTarget));
        else ThrowCastException(source, source.GetType(), typeof(TTarget));
        return default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value to the given target type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool TryConvertTo(
        this object? source,
        Type targetType,
        out object? target)
    {
        Type sourceType = source is null ? null! : source.GetType();

        targetType = targetType.ThrowWhenNull();
        target = default!;

        // Null source...
        if (source is null)
            return targetType.IsNullable() || TryUnwrapNullableType(targetType).IsNullable();

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try
            {
                target = converter.DynamicInvoke(source)!;
                return true;
            }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            var method = GetRegularToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = method.Invoke(source, null)!;
                    return true;
                }
                catch { }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            var method = GetRegularParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = pars[1]!;
                        return true;
                    }
                }
                catch { }
            }
        }

        // Conversion not found...
        return false;
    }

    /// <summary>
    /// Converts the given source value to the given target type, or throws an exception if that
    /// conversion is not possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static object? ConvertTo(
        this object? source,
        Type targetType)
    {
        if (TryConvertTo(source, targetType, out var target)) return target;

        if (source is null) ThrowCastException(source, targetType);
        else ThrowCastException(source, source.GetType(), targetType);
        return default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value to the given target type, using the given locale
    /// if such is available.
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
        IFormatProvider provider)
    {
        Type sourceType = source is null ? typeof(TSource) : source.GetType();
        Type targetType = typeof(TTarget);

        provider = provider.ThrowWhenNull();
        target = default!;

        // Null source...
        if (source is null)
            return GetConverterFromNull(sourceType, targetType) != null;

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try
            {
                target = (TTarget)converter.DynamicInvoke(source)!;
                return true;
            }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            var method = GetFormattedToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = (TTarget)method.Invoke(source, [provider])!;
                    return true;
                }
                catch { }
            }

            method = GetRegularToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = (TTarget)method.Invoke(source, null)!;
                    return true;
                }
                catch { }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            var method = GetFormattedParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, provider, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = (TTarget)pars[2]!;
                        return true;
                    }
                }
                catch { }
            }

            method = GetRegularParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = (TTarget)pars[1]!;
                        return true;
                    }
                }
                catch { }
            }
        }

        // Conversion not found...
        return false;
    }

    /// <summary>
    /// Converts the given source value to the given target type, using the given locale if such
    /// is available, or throws an exception if that conversion is not possible.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TSource, TTarget>(
        [AllowNull] this TSource source,
        IFormatProvider provider)
    {
        if (TryConvertTo<TSource, TTarget>(source, out var target, provider)) return target;

        ThrowCastException(source, typeof(TSource), typeof(TTarget));
        return default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value to the given target type, using the given locale
    /// if such is available.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool TryConvertTo<TTarget>(
        this object? source,
        [MaybeNull] out TTarget target,
        IFormatProvider provider)
    {
        Type sourceType = source is null ? null! : source.GetType();
        Type targetType = typeof(TTarget);

        provider = provider.ThrowWhenNull();
        target = default!;

        // Null source...
        if (source is null)
            return targetType.IsNullable() || TryUnwrapNullableType(targetType).IsNullable();

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try
            {
                target = (TTarget)converter.DynamicInvoke(source)!;
                return true;
            }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            var method = GetFormattedToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = (TTarget)method.Invoke(source, [provider])!;
                    return true;
                }
                catch { }
            }

            method = GetRegularToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = (TTarget)method.Invoke(source, null)!;
                    return true;
                }
                catch { }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            var method = GetFormattedParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, provider, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = (TTarget)pars[2]!;
                        return true;
                    }
                }
                catch { }
            }

            method = GetRegularParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = (TTarget)pars[1]!;
                        return true;
                    }
                }
                catch { }
            }
        }

        // Conversion not found...
        return false;
    }

    /// <summary>
    /// Converts the given source value to the given target type, using the given locale if such
    /// is available, or throws an exception if that conversion is not possible.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TTarget>(
        this object? source,
        IFormatProvider provider)
    {
        if (TryConvertTo<TTarget>(source, out var target, provider)) return target;

        if (source is null) ThrowCastException(source, typeof(TTarget));
        else ThrowCastException(source, source.GetType(), typeof(TTarget));
        return default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value to the given target type, using the given locale
    /// if such is available.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <param name="target"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool TryConvertTo(
        this object? source,
        Type targetType,
        out object? target,
        IFormatProvider provider)
    {
        Type sourceType = source is null ? null! : source.GetType();

        targetType = targetType.ThrowWhenNull();
        provider = provider.ThrowWhenNull();

        target = default!;

        // Null source...
        if (source is null)
            return targetType.IsNullable() || TryUnwrapNullableType(targetType).IsNullable();

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try
            {
                target = converter.DynamicInvoke(source)!;
                return true;
            }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            var method = GetFormattedToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = method.Invoke(source, [provider])!;
                    return true;
                }
                catch { }
            }

            method = GetRegularToString(sourceType);
            if (method != null)
            {
                try
                {
                    target = method.Invoke(source, null)!;
                    return true;
                }
                catch { }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            var method = GetFormattedParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, provider, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = pars[2]!;
                        return true;
                    }
                }
                catch { }
            }

            method = GetRegularParser(targetType);
            if (method != null)
            {
                try
                {
                    var pars = new object?[] { source, null };
                    var r = method.Invoke(null, pars);
                    if (r is bool rb && rb)
                    {
                        target = pars[1]!;
                        return true;
                    }
                }
                catch { }
            }
        }

        // Conversion not found...
        return false;
    }

    /// <summary>
    /// Converts the given source value to the given target type, using the given locale if such
    /// is available, or throws an exception if that conversion is not possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static object? ConvertTo(
        this object? source,
        Type targetType,
        IFormatProvider provider)
    {
        if (TryConvertTo(source, targetType, out var target, provider)) return target;

        if (source is null) ThrowCastException(source, targetType);
        else ThrowCastException(source, source.GetType(), targetType);
        return default!;
    }

    // ----------------------------------------------------

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

    /// <summary>
    /// Cached conversion delegates from a type to a type.
    /// </summary>
    static readonly Dictionary<Type, Dictionary<Type, Delegate?>> Dict = [];

    /// <summary>
    /// Returns the delegate that can convert from null instances of the source type to the
    /// given target one, or null if such conversion is not possible.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static Delegate? GetConverterFromNull(Type sourceType, Type targetType)
    {
        if (!targetType.IsNullable()) return null;

        var sourceCore = TryUnwrapNullableType(sourceType);
        var targetCore = TryUnwrapNullableType(targetType);

        return GetConverter(sourceCore, targetCore);
    }

    /// <summary>
    /// Tries to unwrap the given type, provided that it is a nullable one. Returns the core
    /// underlying type, or the type itself if it is not a nullable one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static Type TryUnwrapNullableType(Type type)
    {
        if (type.IsGenericType)
        {
            var temp = type.GetGenericTypeDefinition();
            if (temp == typeof(Nullable<>))
            {
                var args = type.GetGenericArguments();
                if (args.Length == 1)
                    type = TryUnwrapNullableType(args[0]);
            }
        }
        return type;
    }

    /// <summary>
    /// Returns either a delegate that can convert instances from a given source type to a
    /// given target type, or null if such conversion is not possible.
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
                Dict.Add(sourceType, childs = []);

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