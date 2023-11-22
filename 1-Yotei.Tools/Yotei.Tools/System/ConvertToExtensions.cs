namespace Yotei.Tools;

// ========================================================
public static partial class ConvertToExtensions
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

        target = default!;

        // Null source...
        if (source is null)
            return NullSourceToTarget(sourceType, targetType) != null;

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
            return targetType.IsNullable() || GetCoreType(targetType).IsNullable();

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
            return targetType.IsNullable() || GetCoreType(targetType).IsNullable();

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
            return NullSourceToTarget(sourceType, targetType) != null;

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
            return targetType.IsNullable() || GetCoreType(targetType).IsNullable();

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
            return targetType.IsNullable() || GetCoreType(targetType).IsNullable();

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
}