namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of converting values of a given source type to values of a given target
/// one. Instances of this type are typically used when there is not a standard conversion between
/// an application-level type to any database one.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// The source type to convert from.
    /// </summary>
    Type SourceType { get; }

    /// <summary>
    /// The target type to convert into.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Converts the given source type's value to an instance of the target one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    object? Convert(object? value, Locale locale);
}

// ========================================================
/// <inheritdoc cref="IValueConverter"/>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IValueConverter<TSource, TTarget> : IValueConverter
{
    /// <summary>
    /// <inheritdoc cref="IValueConverter.Convert(object?, Locale)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    TTarget? Convert(TSource? value, Locale locale);
}