namespace Yotei.Tools;

// ========================================================
public static class ConvertToExtensions
{
    /// <summary>
    /// Tries to convert the source instance to the target type.
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
        if (source is null) // Null source...
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            target = default;

            return ValidNullableConversion(sourceType, targetType);
        }
        else // We have a not-null source...
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            target = default;

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

            return false;
        }
    }

    /// <summary>
    /// Converts the source instance to the target type, or throws an exception is such is not
    /// possible.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TSource, TTarget>([AllowNull] this TSource source)
    {
        if (TryConvertTo<TSource, TTarget>(source, out var target)) return target;

        ThrowException(source, typeof(TTarget));
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the source instance to the target type.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool TryConvertTo<TTarget>(
        this object? source,
        [MaybeNull] out TTarget target)
    {
        if (source is null) // Null source...
        {
            var targetType = typeof(TTarget);
            target = default;

            return ValidNullableConversion(targetType, targetType);
        }
        else // We have a not-null source...
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            target = default;

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

            return false;
        }
    }

    /// <summary>
    /// Converts the source instance to the target type, or throws an exception is such is not
    /// possible.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public static TTarget ConvertTo<TTarget>(this object? source)
    {
        if (TryConvertTo<TTarget>(source, out var target)) return target;

        ThrowException(source, typeof(TTarget));
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the source instance to the target type.
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
        targetType = targetType.ThrowIfNull();

        if (source is null) // Null source...
        {
            target = default;
            return ValidNullableConversion(targetType, targetType);
        }
        else // We have a not-null source...
        {
            var sourceType = source.GetType();
            target = default;

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

            return false;
        }
    }

    /// <summary>
    /// Converts the source instance to the target type, or throws an exception is such is not
    /// possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static object? ConvertTo(this object? source, Type targetType)
    {
        targetType = targetType.ThrowIfNull();

        if (TryConvertTo(source, targetType, out var target)) return target;

        ThrowException(source, targetType);
        return default;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an invalid cast exception.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetType"></param>
    static void ThrowException(object? source, Type targetType) =>
        throw new InvalidCastException(
            "Cannot convert the source object to an instance of the given target type.")
            .WithData(source)
            .WithData(targetType);

    /// <summary>
    /// A cache of conversion delegates from a source type to a target one.
    /// </summary>
    static readonly Dictionary<Type, Dictionary<Type, Delegate?>> Dict = new();

    /// <summary>
    /// Returns a delegate that can convert instances of the source type into instances of the
    /// target one, or null if such conversion is not possible.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static Delegate? GetConverter(Type sourceType, Type targetType)
    {
        lock (Dict)
        {
            // Making sure we have a source type entry...
            if (!Dict.TryGetValue(sourceType, out var childs))
            {
                childs = new();
                Dict.Add(sourceType, childs);
            }

            // Either we already have a converter, or creating a null one...
            if (childs.TryGetValue(targetType, out var converter)) return converter;
            childs.Add(targetType, null);

            // Creating a converter for the trivial case...
            if (sourceType == targetType)
            {
                var input = Expression.Parameter(sourceType);
                var expr = Expression.Lambda(input, input);

                converter = expr.Compile();
                childs[targetType] = converter;
                return converter;
            }

            // Custom converter...
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

            // It's ok to return null as we have already added a null converter...
            return null;
        }
    }

    /// <summary>
    /// Determines if there is a valid conversion to the source type to the target one, when
    /// the source value happens to be null.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static bool ValidNullableConversion(Type sourceType, Type targetType)
    {
        if (targetType.IsNullable())
        {
            if (sourceType == targetType) return true;

            var sourceCore = GetCoreType(sourceType);
            var targetCore = GetCoreType(targetType);

            var converter = GetConverter(sourceCore, targetCore);
            return converter is not null;
        }

        return false;

        // Gets the core type of the given one...
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
    }
}