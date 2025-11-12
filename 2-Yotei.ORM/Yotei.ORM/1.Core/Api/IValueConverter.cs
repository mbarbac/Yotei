namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability to convert instances of the given source type to instances of the
/// given target one. Converters are typically used when there is no conversion from application
/// level types to database ones.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// The source type to convert from.
    /// </summary>
    Type SourceType { get; }

    /// <summary>
    /// The target type to convert to.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Converts the given value from its source type (assumed to be compatible to the one of
    /// this instance) to the target one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    object? Convert(object? value, ILocale? locale = null);
}

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverter"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IValueConverter<TSource, TTarget> : IValueConverter
{
    /// <summary>
    /// Converts the given value from its source type to the target one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    [return: MaybeNull] TTarget Convert([AllowNull] TSource value, ILocale? locale = null);
}