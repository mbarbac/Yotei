namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of translating values of a given source type to a given target one.
/// </summary>
public interface IValueTranslator
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
    /// Translates the given source value to a target one, using the given locale if such is
    /// needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    object? Translate(object? source, Locale locale);
}

// ========================================================
/// <inheritdoc cref="IValueTranslator"/>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IValueTranslator<TSource, TTarget> : IValueTranslator
{
    /// <summary>
    /// Translates the given source value to a target one, using the given locale if such is
    /// needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    TTarget Translate(TSource source, Locale locale);
}