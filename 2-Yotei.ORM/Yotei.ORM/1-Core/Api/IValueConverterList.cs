namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of converting values from their source types to the target ones by the
/// converters registered in this collection.
/// </summary>
[Cloneable]
public partial interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <summary>
    /// Gets the number of converters registered into this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns the converter registered in this collection for the given source type, or null if
    /// it cannot be found.
    /// <br/> In relaxed mode, if a strict converter is not found, then the first one whose source
    /// type implements or derives from the given one, if any, is returned.
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
    /// Tries to convert the given source value using a registered converter for its source type,
    /// if any. In relaxed mode, if a strict converter is not found, then the first one whose source
    /// type implements or derives from the given one, if any, is used.
    /// <br/> If the value is <see langword="null"/>, then null is always returned.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    object? TryConvert<TSource>(
        [AllowNull] TSource value, Locale? locale = null, bool relax = false);
}