namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of converting values from the given source type to the given target
/// one.
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
/// <summary>
/// <inheritdoc/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IValueConverter<TSource, TTarget> : IValueConverter
{
    /// <inheritdoc cref="IValueConverter.Convert(object?, Locale)"/>
    TTarget? Convert(TSource? value, Locale locale);
}