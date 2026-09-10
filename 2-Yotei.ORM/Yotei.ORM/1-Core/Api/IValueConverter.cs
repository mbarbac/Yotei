namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of converting instances from a source type to a target one.
/// <br/> Converters are typically used when there is no standard conversion from application
/// level types to database ones, and viceversa.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// The source type to convert values from.
    /// </summary>
    Type SourceType { get; }

    /// <summary>
    /// The target type to convert values to.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Tries to convert the given value from the source type to the target one, using the given
    /// locale if provided and needed.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    object? Convert(object? value, Locale? locale = null);
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
    /// <inheritdoc cref="IValueConverter.Convert(object?, Locale?)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    [return: MaybeNull]
    TTarget Convert([AllowNull] TSource value, Locale? locale = null);
}