/*
 namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverter{TSource, TTarget}"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public class ValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    /// <summary>
    /// Initializes a new default converter.
    /// </summary>
    public ValueConverter() { }

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

    Func<TSource?, IFormatProvider?, TTarget> _Converter =
        (x, provider) => x.ConvertTo<TTarget?>(provider)!;

    /// <summary>
    /// <inheritdoc cref="IValueConverter{TSource, TTarget}.Convert(TSource)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: MaybeNull]
    public virtual TTarget Convert([AllowNull] TSource value) => throw null;

    object? IValueConverter.Convert(object? value) => Convert((TSource)value!);
}*/