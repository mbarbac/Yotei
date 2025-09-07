namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverter"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public class ValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    /// <summary>
    /// Initializes a new instance with a default converter.
    /// </summary>
    public ValueConverter()
    {
        _Converter = (x, locale) => x is null ? default! : x.ConvertTo<TTarget?>(locale);
    }

    /// <summary>
    /// Initializes a new instance with the given converter.
    /// </summary>
    /// <param name="converter"></param>
    public ValueConverter(Func<TSource?, Locale, TTarget?> converter)
    {
        _Converter = converter.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"({SourceType.EasyName()} => {TargetType.EasyName()})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public Type SourceType => typeof(TSource);

    /// <inheritdoc/>
    public Type TargetType => typeof(TTarget);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual TTarget? Convert(TSource? value, Locale locale) => _Converter(value, locale);
    readonly Func<TSource?, Locale, TTarget?> _Converter = default!;

    object? IValueConverter.Convert(
        object? value, Locale locale) => Convert((TSource?)value, locale);
}