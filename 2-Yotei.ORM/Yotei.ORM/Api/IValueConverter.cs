namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of converting values of a given source type to a given target one.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// The source type of the values to convert.
    /// </summary>
    Type SourceType { get; }

    /// <summary>
    /// The target type to convert the values to.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Converts the given source value to a target one, using the given locale if such is
    /// needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    object? Convert(object? source, Locale locale);
}

// ========================================================
/// <inheritdoc cref="IValueConverter"/>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IValueConverter<TSource, TTarget> : IValueConverter
{
    /// <summary>
    /// Converts the given source value to a target one, using the given locale if such is
    /// needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    TTarget Convert(TSource source, Locale locale);
}