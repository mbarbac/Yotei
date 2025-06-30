namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of value converters from their source types to their target ones.
/// Instances of this type are typically used to register converters between application-level
/// values and database-level ones, when there is no standard conversion between them.
/// <br/> Converters with duplicate source type are not allowed.
/// </summary>
[Cloneable]
public partial interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns the converter in this instance registered for the given source type, or null if
    /// any. If not found, then converters registered for any base type of the given one, or
    /// converters registered for interfaces the given type implements, are also be taken into
    /// consideration if such is requested.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    IValueConverter? Find(Type type, bool chain = false, bool ifaces = false);

    /// <summary>
    /// Returns the converter in this instance registered for the given source type, or null if
    /// any. If not found, then converters registered for any base type of the given one, or
    /// converters registered for interfaces the given type implements, are also be taken into
    /// consideration if such is requested.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    IValueConverter? Find<TSource>(bool chain = false, bool ifaces = false);

    // ----------------------------------------------------

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    /// <summary>
    /// Adds to this collection the given converter, provided its source type is not yet
    /// registered into this instance.
    /// </summary>
    /// <param name="converter"></param>
    void Add(IValueConverter converter);

    /// <summary>
    /// Adds to this collection the given converters, provided that their source types are not
    /// yet registered into this instance.
    /// </summary>
    /// <param name="range"></param>
    void AddRange(IEnumerable<IValueConverter> range);

    /// <summary>
    /// Replaces the converter in this collection registered for the source type of the given one.
    /// If no such converter is registered, then just adds the given one.
    /// </summary>
    /// <param name="converter"></param>
    void Replace(IValueConverter converter);

    /// <summary>
    /// Removes from this collection the converter registered for the given source type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool Remove(Type type);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    void Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value using a converter registered for its type, or
    /// returns the original value if such converter is not found. In this case, converters
    /// registered for base types of the given one, or registered for interfaces the given one
    /// implements, can also be taken into consideration, if such is requested.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    object? TryConvert<TSource>(
        TSource? value, Locale locale, bool chain = false, bool ifaces = false);
}