namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Obtains the dotted-separted identifier parts contained in the given value, using the
    /// separators of the given engine to protect the embedded ones, if any. An empty list is
    /// returned if that value is null, empty, or when all identified parts are null or empty
    /// themselves.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> GetParts(IEngine engine, string? value, bool reduce = true)
    {
        engine.ThrowWhenNull();

        var parts = engine.UseTerminators
            ? GetPartsWithTerminators(engine, value)
            : GetPartsNoTerminators(value);

        if (reduce)
        {
            while (parts.Count > 0)
            {
                if (parts[0] == null) parts.RemoveAt(0);
                else break;
            }
        }

        return parts;
    }

    /// <summary>
    /// Invoked when the engine uses no terminators.
    /// </summary>
    static List<string?> GetPartsNoTerminators(string? value)
    {
        throw null;
    }

    /// <summary>
    /// Invoked when the engine uses terminators.
    /// </summary>
    static List<string?> GetPartsWithTerminators(IEngine engine, string? value)
    {
        throw null;
    }
}