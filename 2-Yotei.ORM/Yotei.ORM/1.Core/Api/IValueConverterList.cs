namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of value converters from instances of their specified source type to
/// instances of their specified target ones.
/// </summary>
[Cloneable]
public partial interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <summary>
    /// The number of converters in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Enumerates the source types registered in this collection.
    /// </summary>
    IEnumerable<Type> SourceTypes { get; }

    /// <summary>
    /// Tries to find the converter registered in this instance for the given source type. If
    /// not found, the converters registered for a base type or implemented interface can also
    /// be returned, if such is explicitly requested.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    IValueConverter? Find(Type type, bool relax = false);

    /// <summary>
    /// Tries to find the converter registered in this instance for the given source type. If
    /// not found, the converters registered for a base type or implemented interface can also
    /// be returned, if such is explicitly requested.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="relax"></param>
    /// <returns></returns>
    IValueConverter? Find<TSource>(bool relax = false);

    // ----------------------------------------------------

    /// <summary>
    /// Adds to this collection the given converter, provided that there is not converter already
    /// registered with the same source type.
    /// </summary>
    /// <param name="converter"></param>
    void Add(IValueConverter converter);

    /// <summary>
    /// Adds to this collection the given converter or, if there is a converter already registered
    /// for its source type, replaces the existing one.
    /// </summary>
    /// <param name="converter"></param>
    void AddOrReplace(IValueConverter converter);

    /// <summary>
    /// Removes from this collection the converter registered for the given source type, if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool Remove(Type type);

    /// <summary>
    /// Clears all the converters registered in this instance.
    /// </summary>
    void Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Tries to convert the given source value to an instance of the given target type using a
    /// converter registered in this collection. If such converter was not found, and relax is
    /// requested, the first converter for a base type or implemente interface, if any, is used.
    /// If finally no converter is found, returns the source value itself. 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    object? TryConvert<TSource>(TSource? value, ILocale? locale = null, bool relax = false);
}