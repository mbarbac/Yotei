namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Representes a collection of value converters that tries to convert a given value using any
/// of the converters registered into it.
/// </summary>
[Cloneable]
public partial interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <summary>
    /// Gets the number of converters registered into this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Enumerates the source types of the converters registered into this instance.
    /// </summary>
    IEnumerable<Type> SourceTypes { get; }

    /// <summary>
    /// Tries to find a converter registered into this instance for the given source type, or
    /// returns null if such cannot be found.
    /// <br/> If no converter can be found in the default strict mode, and if <paramref name="relax"/>
    /// is requested, then the first converter from whose source type the given one implements or
    /// inherits from is then returned.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    IValueConverter? Find(Type sourceType, bool relax = false);

    /// <summary>
    /// <inheritdoc cref="Find(Type, bool)"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="relax"></param>
    /// <returns></returns>
    IValueConverter? Find<TSource>(bool relax = false);

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this collection the given converter. If its source type is already registered
    /// into this collection, an exception is thrown.
    /// </summary>
    /// <param name="converter"></param>
    void Add(IValueConverter converter);

    /// <summary>
    /// Adds to this collection the given converter. If its source type is already registered
    /// into this collection, then the existing converter is replaced by the newly given one.
    /// </summary>
    /// <param name="converter"></param>
    void AddOrReplace(IValueConverter converter);

    /// <summary>
    /// Removes from this collection the converter registered for the given source type, if any.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <returns></returns>
    bool Remove(Type sourceType);

    /// <summary>
    /// <inheritdoc cref="Remove(Type)"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    bool Remove<TSource>();

    /// <summary>
    /// Clears this instance.
    /// </summary>
    void Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given value using a converter registered into this instance for its
    /// source type, and the given locale, if any. If such converter is not found, then the original
    /// value is returned.
    /// <br/> If no converter can be found in the default strict mode, and if <paramref name="relax"/>
    /// is requested, then the first converter from whose source type the given one implements or
    /// inherits from is used.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    object? TryConvert<TSource>(
        [AllowNull] TSource value, Locale? locale = null, bool relax = false);
}