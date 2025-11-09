namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Helpers for <see cref="IIdentifier"/> instances.
/// </summary>
public static class Identifier
{
    /// <summary>
    /// Returns a list with the dot-separated parts found in the given source. Engine terminators
    /// (if used) are always removed, and the parts are trimmed. Empty parts are returned as null
    /// ones. The returned list is, by default, reduced by removing its empty or null heads.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> GetParts(IEngine engine, string? value, bool reduce = true)
    {
        engine.ThrowWhenNull();

        if ((value = value.NullWhenEmpty(true)) is null) return [];

        var items = engine.UseTerminators ? GetRawParts(value, engine) : GetRawParts(value);
        if (reduce)
        {
            while (items.Count > 0)
            {
                if (items[0] is null) items.RemoveAt(0);
                else break;
            }
        }
        return items;
    }

    /// <summary>
    /// Invoked to obtain the raw dot-separated parts of the given value when the engine does
    /// not use terminators.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static List<string?> GetRawParts(string value)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to obtain the raw dot-separated parts of the given value when the engine uses
    /// terminators.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="engine"></param>
    /// <returns></returns>
    static List<string?> GetRawParts(string value, IEngine engine)
    {
        throw null;
    }
}