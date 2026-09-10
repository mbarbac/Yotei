namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverter{TSource, TTarget}"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public sealed class ValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    /// <summary>
    /// Initializes a new instance with a default converter.
    /// </summary>
    public ValueConverter()
    {
        _Converter = static (x, locale)
            => locale is null
            ? x.ConvertTo<TTarget?>()!
            : x.ConvertTo<TTarget?>(locale.CultureInfo)!;
    }

    /// <summary>
    /// Initializes a new instance with the given converter.
    /// </summary>
    /// <param name="converter"></param>
    public ValueConverter(Func<TSource, Locale?, TTarget> converter)
    {
        _Converter = converter.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({SourceType.EasyName()} => {TargetType.EasyName()})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Type SourceType => typeof(TSource);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Type TargetType => typeof(TTarget);

    // ----------------------------------------------------

    readonly Func<TSource, Locale?, TTarget> _Converter;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public TTarget Convert(
        [AllowNull] TSource value, Locale? locale = null) => _Converter(value!, locale);

    object? IValueConverter.Convert(
        object? value, Locale? locale) => _Converter((TSource)value!, locale);
}