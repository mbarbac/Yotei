namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IValueConverter{TSource, TTarget}"/>
public class ValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="func"></param>
    public ValueConverter(Func<TSource, Locale, TTarget> func)
    {
        _Converter = func.ThrowWhenNull();
    }
    readonly Func<TSource, Locale, TTarget> _Converter;

    /// <inheritdoc/>
    public override string ToString() => $"({SourceType.EasyName()} => {TargetType.EasyName()})";

    /// <inheritdoc/>
    public Type SourceType => typeof(TSource);

    /// <inheritdoc/>
    public Type TargetType => typeof(TTarget);

    /// <inheritdoc/>
    public TTarget Convert(TSource source, Locale locale) => _Converter(source!, locale);
    object? IValueConverter.Convert(object? source, Locale locale) => Convert((TSource)source!, locale);
}