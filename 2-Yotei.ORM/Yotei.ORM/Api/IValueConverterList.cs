namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of value converters from their source types to their target ones.
/// </summary>
[Cloneable]
public partial interface IValueConverterList : IEnumerable<IValueConverter>
{
    /// <summary>
    /// Gets the number of converters registered into this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Finds the converter for the given source type, or null if such is not registered into this
    /// instance. In the later case, if requested, a converter for a type from which the given one
    /// inherits from can be returned, if exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    IValueConverter? Find(Type type, bool inherit = false);

    /// <summary>
    /// Finds the converter for the given source type, or null if such is not registered into this
    /// instance. In the later case, if requested, a converter for a type from which the given one
    /// inherits from can be returned, if exist.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="inherit"></param>
    /// <returns></returns>
    IValueConverter? Find<TSource>(bool inherit = false);

    /// <summary>
    /// Adds the given converter to this instance, if its source type is not registered yet.
    /// Otherwise, throws an appropriate exception.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="func"></param>
    void Add<TSource, TTarget>(IValueConverter<TSource, TTarget> func);

    /// <summary>
    /// Adds the given converter to this instance, if its source type is not registered yet.
    /// Otherwise, throws an appropriate exception.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void Add<TSource, TTarget>(Func<TSource, Locale, TTarget> func);

    /// <summary>
    /// Adds the given converter to this instance, if its source type is not registered yet,
    /// or replaces the existing one.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void AddOrReplace<TSource, TTarget>(IValueConverter<TSource, TTarget> func);

    /// <summary>
    /// Adds the given converter to this instance, if its source type is not registered yet,
    /// or replaces the existing one.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void AddOrReplace<TSource, TTarget>(Func<TSource, Locale, TTarget> func);

    /// <summary>
    /// Adds to this instance the converters obtained from the given range.
    /// </summary>
    /// <param name="range"></param>
    void AddRange(IEnumerable<IValueConverter> range);

    /// <summary>
    /// Removes from this collection the converter registered for the given type, if any.
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
    /// Tries to convert the given source value to its equivalent one using a converter in
    /// this collection for its type, or one it inherits from if such is requested. If no one
    /// can be found, returns the source value itself.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    object? TryConvert(object? source, Locale locale, bool inherit = false);

    /// <summary>
    /// Tries to convert the given source value to its equivalent one using a converter in
    /// this collection for its type, or one it inherits from if such is requested. If no one
    /// can be found, returns the source value itself.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    object? TryConvert<TSource>(TSource source, Locale locale, bool inherit = false);

    /// <summary>
    /// Tries to convert the given source value to its equivalent one using a converter in
    /// this collection for its type, or one it inherits from if such is requested. If no one
    /// can be found, returns the source value itself.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    TTarget TryConvert<TSource, TTarget>(TSource source, Locale locale, bool inherit = false);
}