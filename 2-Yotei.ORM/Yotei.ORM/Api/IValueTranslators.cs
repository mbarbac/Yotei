namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a collection of translators from their source types to their target ones.
/// </summary>
[Cloneable]
public partial interface IValueTranslators : IEnumerable<IValueTranslator>
{
    /// <summary>
    /// Gets the number of translators registered into this instance.
    /// </summary>
    int Count { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the translator for the given source type, or null if such is not registered into
    /// this instance. In the later case, if requested, a translator for a type from which the
    /// given one inherits from can be returned, if exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    IValueTranslator? Find(Type type, bool inherit = false);

    /// <summary>
    /// Finds the translator for the given source type, or null if such is not registered into
    /// this instance. In the later case, if requested, a translator for a type from which the
    /// given one inherits from can be returned, if exist.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="inherit"></param>
    /// <returns></returns>
    IValueTranslator? Find<TSource>(bool inherit = false);

    /// <summary>
    /// Adds the given translator to this instance, if its source type is not registered yet.
    /// Otherwise, throws an appropriate exception.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void Add<TSource, TTarget>(IValueTranslator<TSource, TTarget> translator);

    /// <summary>
    /// Adds the given translator to this instance, if its source type is not registered yet.
    /// Otherwise, throws an appropriate exception.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void Add<TSource, TTarget>(Func<TSource, Locale, TTarget> translator);

    /// <summary>
    /// Adds the given translator to this instance, if its source type is not registered yet, or
    /// replaces the existing one.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void AddOrReplace<TSource, TTarget>(IValueTranslator<TSource, TTarget> translator);

    /// <summary>
    /// Adds the given translator to this instance, if its source type is not registered yet, or
    /// replaces the existing one.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="translator"></param>
    void AddOrReplace<TSource, TTarget>(Func<TSource, Locale, TTarget> translator);

    /// <summary>
    /// Adds to this instance the translators obtained from the given range.
    /// </summary>
    /// <param name="range"></param>
    void AddRange(IEnumerable<IValueTranslator> range);

    /// <summary>
    /// Removes from this collection the translator registered for the given type, if any.
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
    /// Tries to convert the given source value to its equivalent one using a translator in this
    /// collection for its type, or one it inherits from if such is requested. If no translator
    /// can be found, returns the source value itself.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    object? Translate(object? source, Locale locale, bool inherit = false);

    /// <summary>
    /// Tries to convert the given source value to its equivalent one using a translator in this
    /// collection for its type, or one it inherits from if such is requested. If no translator
    /// can be found, returns the source value itself.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="locale"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    object? Translate<TSource>(TSource source, Locale locale, bool inherit = false);
}