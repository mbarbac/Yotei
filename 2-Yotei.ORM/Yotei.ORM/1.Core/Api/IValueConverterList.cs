namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of value converters that convert values from their respective source
/// types to their target ones, used when there are not standard conversions among application
/// level types to database ones, and viceversa.
/// <br/> Instances this type are not synchronized.
/// </summary>
public partial interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    IValueConverterList Clone();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Tries to find in this instance a converter registered for the given source type. If not
    /// found, then converters registered for any base type or implemented interface may also be
    /// returned if such is explicitly requested.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    IValueConverter? Find(Type type, bool relax = false);

    /// <summary>
    /// Tries to find in this instance a converter registered for the given source type. If not
    /// found, then converters registered for any base type or implemented interface may also be
    /// returned if such is explicitly requested.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="relax"></param>
    /// <returns></returns>
    IValueConverter? Find<TSource>(bool relax = false);

    /// <summary>
    /// Trims the internal structures of this instance.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this collection the given converter. If its source type is already registered in
    /// this instance, then an exception is thrown.
    /// </summary>
    /// <param name="converter"></param>
    void Add(IValueConverter converter);

    /// <summary>
    /// Adds to this collection the given collection of converters. If the source type of any of
    /// them was already in this instance, then an exception is thrown.
    /// </summary>
    /// <param name="range"></param>
    void AddRange(IEnumerable<IValueConverter> range);

    /// <summary>
    /// Replaces the existing converter registered for the source type of the given one with that
    /// given converter, or adds it to this instance if no existing one was found.
    /// </summary>
    /// <param name="converter"></param>
    void Replace(IValueConverter converter);

    /// <summary>
    /// Replaces the existing converters in this instance registered for the source types of the
    /// ones in the given collection, or adds them if their source types were not found.
    /// </summary>
    /// <param name="range"></param>
    void ReplaceRange(IEnumerable<IValueConverter> range);

    /// <summary>
    /// Removes the converter registered for the given source type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool Remove(Type type);

    /// <summary>
    /// Clears this instance.
    /// </summary>
    void Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value using a converter registered in this instance
    /// for its source type. If such converter was not found, then converters registered for any
    /// base type or implemented interface may also be used, if such is explicitly requested.
    /// <br/> If no converter was found for the source type, then the original value is returned.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    object? TryConvert<TSource>(TSource? value, Locale locale, bool relax = false);
}