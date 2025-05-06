namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of value converters, where the source types of its elements must
/// be unique ones.
/// </summary>
public interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns the converter in this instance registered for the given source type, or null if
    /// any. If requested, converters for base types the given one inherits from, and also those
    /// with an interface that implements the given one, can also be taken into consideration.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    IValueConverter? Find(Type sourceType, bool chain = false, bool ifaces = false);

    /// <summary>
    /// Returns the converter in this instance registered for the given source type, or null if
    /// any. If requested, converters for base types the given one inherits from, and also those
    /// with an interface that implements the given one, can also be taken into consideration.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    IValueConverter? Find<TSource>(bool chain = false, bool ifaces = false);

    /// <summary>
    /// Adds to this collection the given converter, provided its source type is not already
    /// registered.
    /// </summary>
    /// <param name="converter"></param>
    void Add(IValueConverter converter);

    /// <summary>
    /// Adds to this collection the given converter, or if its source type was already registered,
    /// replaces the existing one.
    /// </summary>
    /// <param name="converter"></param>
    void Replace(IValueConverter converter);

    /// <summary>
    /// Removes from this collection the converter registered for the given source type.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <returns></returns>
    bool Remove(Type sourceType);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    void Clear();
}