namespace Yotei.ORM;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverter"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IValueConverter<TSource, TTarget> : IValueConverter
{
    /// <summary>
    /// <inheritdoc cref="IValueConverter.Convert(object?, Locale?)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    [return: MaybeNull]
    TTarget Convert([AllowNull] TSource value, Locale? locale = null);
}