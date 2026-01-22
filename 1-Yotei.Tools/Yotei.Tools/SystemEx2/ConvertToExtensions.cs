namespace Yotei.Tools;

// ========================================================
public static class ConvertToExtensions
{
    /// <summary>
    /// Tries to convert the source value to an instance of the given target type.
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

        // Converter from null source...
        if (source is null)
        {
            target = default;
            return targetType.IsNullable || UnwrapNullableType(targetType).IsNullable;
        }

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try { target = (TTarget)converter.DynamicInvoke(source)!; return true; }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            if (provider is not null)
            {
                var method = GetFormattedToString(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, [provider])!; return true; }
                    catch { }
                }
            }
            else
            {
                var method = GetStandardToString(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, null)!; return true; }
                    catch { }
                }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            if (provider is not null)
            {
                var method = GetFormattedTryParse(targetType);
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
                var method = GetStandardTryParse(targetType);
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
    /// Converts the source value to an instance of the given target type, or throws an exception
    /// if that conversion is not possible.
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
    /// Tries to convert the source value to an instance of the given target type.
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

        // Converter from null source...
        if (source is null)
        {
            target = default;
            return targetType.IsNullable || UnwrapNullableType(targetType).IsNullable;
        }

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try { target = (TTarget)converter.DynamicInvoke(source)!; return true; }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            if (provider is not null)
            {
                var method = GetFormattedToString(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, [provider])!; return true; }
                    catch { }
                }
            }
            else
            {
                var method = GetStandardToString(sourceType);
                if (method is not null)
                {
                    try { target = (TTarget)method.Invoke(source, null)!; return true; }
                    catch { }
                }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            if (provider is not null)
            {
                var method = GetFormattedTryParse(targetType);
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
                var method = GetStandardTryParse(targetType);
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
    /// Converts the source value to an instance of the given target type, or throws an exception
    /// if that conversion is not possible.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TTarget>(
        this object? source,
        IFormatProvider? provider = null)
    {
        if (TryConvertTo<TTarget>(source, out var target, provider)) return target;

        if (source is null) ThrowCastException(source, typeof(TTarget));
        else ThrowCastException(source, source.GetType(), typeof(TTarget));
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the source value to an instance of the given target type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="targetType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool TryConvertTo(
        this object? source,
        out object? target,
        Type targetType,
        IFormatProvider? provider = null)
    {
        Type sourceType = source is null ? null! : source.GetType();
        targetType.ThrowWhenNull();

        // Converter from null source...
        if (source is null)
        {
            target = default;
            return targetType.IsNullable || UnwrapNullableType(targetType).IsNullable;
        }

        // Standard conversion...
        var converter = GetConverter(sourceType, targetType);
        if (converter != null)
        {
            try { target = converter.DynamicInvoke(source)!; return true; }
            catch { }
        }

        // ToString methods...
        if (targetType == typeof(string))
        {
            if (provider is not null)
            {
                var method = GetFormattedToString(sourceType);
                if (method is not null)
                {
                    try { target = method.Invoke(source, [provider])!; return true; }
                    catch { }
                }
            }
            else
            {
                var method = GetStandardToString(sourceType);
                if (method is not null)
                {
                    try { target = method.Invoke(source, null)!; return true; }
                    catch { }
                }
            }
        }

        // TryParse methods...
        if (sourceType == typeof(string))
        {
            if (provider is not null)
            {
                var method = GetFormattedTryParse(targetType);
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
                var method = GetStandardTryParse(targetType);
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
    /// Converts the source value to an instance of the given target type, or throws an exception
    /// if that conversion is not possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static object? ConvertTo(
        this object? source,
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
    /// Returns the delegate that can convert instances of the source type to instances of the
    /// target one, or null if such conversion is not possible.
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

    /// <summary>
    /// Returns either the underlying not-nullable type, provided the given one is nullable, or
    /// that given type otherwise.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static Type UnwrapNullableType(Type type)
    {
        if (type.IsGenericType)
        {
            var temp = type.GetGenericTypeDefinition();
            if (temp == typeof(Nullable<>))
            {
                var args = type.GetGenericArguments();
                if (args.Length == 1) type = UnwrapNullableType(args[0]);
            }
        }
        return type;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    static void ThrowCastException(object? source, Type targetType)
    {
        if (source is null) throw new InvalidCastException(
            "Cannot convert the given source to the given target type.")
            .WithData(source)
            .WithData(targetType);

        ThrowCastException(source, source.GetType(), targetType);
    }

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    static void ThrowCastException(object? source, Type sourceType, Type targetType)
    {
        throw new InvalidCastException(
            "Cannot convert the given source to the given target type.")
            .WithData(source)
            .WithData(sourceType)
            .WithData(targetType);
    }

    // ----------------------------------------------------

    static readonly Dictionary<Type, MethodInfo?> StandardToString = [];

    /// <summary>
    /// Gets the standard 'ToString()' method declared at the given type, or null if any.
    /// </summary>
    static MethodInfo? GetStandardToString(Type type)
    {
        if (StandardToString.TryGetValue(type, out var temp)) return temp;

        var methods = type.GetMethods().Where(static x => x.Name == "ToString");
        foreach (var method in methods)
        {
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 0) continue;

            StandardToString.Add(type, method);
            return method;
        }
        return null;
    }

    // ----------------------------------------------------

    static readonly Dictionary<Type, MethodInfo?> FormattedToString = [];

    /// <summary>
    /// Gets the 'ToString(IFormatProvider)' method declared at the given type, or null if any.
    /// </summary>
    static MethodInfo? GetFormattedToString(Type type)
    {
        if (FormattedToString.TryGetValue(type, out var temp)) return temp;

        var methods = type.GetMethods().Where(static x => x.Name == "ToString");
        foreach (var method in methods)
        {
            if (method.DeclaringType != type) continue;

            var pars = method.GetParameters();
            if (pars.Length != 1) continue;
            if (!pars[0].ParameterType.IsAssignableTo(typeof(IFormatProvider))) continue;

            FormattedToString.Add(type, method);
            return method;
        }
        return null;
    }

    // ----------------------------------------------------

    static readonly Dictionary<Type, MethodInfo?> StandardTryParser = [];

    /// <summary>
    /// Gets the static 'TryParse(string, out Type value)' method or null if any.
    /// </summary>
    static MethodInfo? GetStandardTryParse(Type type)
    {
        if (StandardTryParser.TryGetValue(type, out var temp)) return temp;

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

            StandardTryParser.Add(type, method);
            return method;
        }
        return null;
    }

    // ----------------------------------------------------

    static readonly Dictionary<Type, MethodInfo?> FormattedTryParser = [];

    /// <summary>
    /// Gets the static 'TryParse(string, IFormatProvider, out Type value)' method or null if any.
    /// </summary>
    static MethodInfo? GetFormattedTryParse(Type type)
    {
        if (FormattedTryParser.TryGetValue(type, out var temp)) return temp;

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

            FormattedTryParser.Add(type, method);
            return method;
        }
        return null;
    }
}