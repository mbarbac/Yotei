namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Returns a new identifier.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public static IIdentifier Create(IEngine engine, string? value)
    {
        engine.ThrowWhenNull();
        value = value.NullWhenEmpty();

        var item = new IdentifierChain(engine, value);
        if (item.Count == 0) return new IdentifierPart(engine);
        if (item.Count == 1) return item[0];
        return item;
    }

    /// <summary>
    /// Returns a new identifier.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, IEnumerable<string?> values)
    {
        engine.ThrowWhenNull();
        values.ThrowWhenNull();

        var item = new IdentifierChain(engine, values);
        if (item.Count == 0) return new IdentifierPart(engine);
        if (item.Count == 1) return item[0];
        return item;
    }
}