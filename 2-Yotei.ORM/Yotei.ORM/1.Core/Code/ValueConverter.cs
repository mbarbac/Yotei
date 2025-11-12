namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverter{TSource, TTarget}"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public class ValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    readonly Func<TSource?, ILocale?, TTarget?> Converter;

    /// <summary>
    /// Initializes a new default converter.
    /// </summary>
    public ValueConverter() => Converter = static (x, locale)
        => locale is null
        ? x.ConvertTo<TTarget?>()
        : x.ConvertTo<TTarget?>(locale.CultureInfo);

    /// <summary>
    /// Initializes a new instance with the given converter.
    /// </summary>
    /// <param name="converter"></param>
    public ValueConverter(
        Func<TSource?, ILocale?, TTarget?> converter)
        => Converter = converter.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({SourceType.EasyName()} => {TargetType.EasyName()})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Type SourceType => typeof(TSource);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Type TargetType => typeof(TTarget);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IValueConverter.Convert(object?, ILocale)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public virtual TTarget Convert(
        [AllowNull] TSource value, ILocale? locale = null) => Converter(value, locale);

    object? IValueConverter.Convert(object? value, ILocale? locale) => Convert((TSource)value!, locale);
}