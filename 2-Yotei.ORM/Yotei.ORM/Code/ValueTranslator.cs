namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IValueTranslator{TSource, TTarget}"/>
public class ValueTranslator<TSource, TTarget> : IValueTranslator<TSource, TTarget>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="translator"></param>
    public ValueTranslator(Func<TSource, Locale, TTarget> translator)
    {
        _Translator = translator.ThrowWhenNull();
    }
    readonly Func<TSource, Locale, TTarget> _Translator;

    /// <inheritdoc/>
    public override string ToString() => $"({SourceType.EasyName()} => {TargetType.EasyName()})";

    /// <inheritdoc/>
    public Type SourceType => typeof(TSource);

    /// <inheritdoc/>
    public Type TargetType => typeof(TTarget);

    /// <inheritdoc/>
    public TTarget Translate(TSource source, Locale locale) => _Translator(source!, locale);
    object? IValueTranslator.Translate(object? source, Locale locale) => Translate((TSource)source!, locale);
}