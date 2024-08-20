namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Returns a new identifier using the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value = null)
    {
        var item = new IdentifierChain(engine, value);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0] :
            item;
    }

    /// <summary>
    /// Returns a new identifier using the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, IEnumerable<string?> range)
    {
        var item = new IdentifierChain(engine, range);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0] :
            item;
    }
}