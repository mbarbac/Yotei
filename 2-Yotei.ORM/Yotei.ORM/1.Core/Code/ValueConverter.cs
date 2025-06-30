namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IValueConverter{TSource, TTarget}"/>
public class ValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    /// <summary>
    /// Initializes a new instance with a default converter between the source and target
    /// types.
    /// </summary>
    public ValueConverter() { }

    /// <summary>
    /// Initializes a new instance with the given converter delegate between the source and the
    /// target types.
    /// </summary>
    /// <param name="converter"></param>
    public ValueConverter(
        Func<TSource?, Locale, TTarget?> converter) => _Converter = converter.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => $"[{SourceType.EasyName()} => {TargetType.EasyName()}]";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public Type SourceType => typeof(TSource);

    /// <inheritdoc/>
    public Type TargetType => typeof(TTarget);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> This method, by default, uses either the default converter between types
    /// or the one given at instantiation time. But it can also be overriden by derived types
    /// as needed.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    public virtual TTarget? Convert(TSource? value, Locale locale) => _Converter(value, locale);

    object? IValueConverter.Convert(
        object? value, Locale locale) => Convert((TSource?)value, locale);

    readonly Func<TSource?, Locale, TTarget?> _Converter
        = (x, _) => x is null ? default! : x.ConvertTo<TTarget>();
}