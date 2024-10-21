#pragma warning disable IDE0305

namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Returns the list of dot-separated parts contained in the given value. Empty parts are
    /// transformed to nulls, and engine terminators are removed. By default, the returned list
    /// is reduced by removing the empty or null heading parts.
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
    /// Invoked when the engine uses NO terminators
    /// </summary>
    static List<string?> GetPartsNoTerminators(string? value)
    {
        if (value == null) return [];

        string?[] items = value.Contains('.') ? value.Split('.') : [value];

        for (int i = 0; i < items.Length; i++) items[i] = Validate(items[i]);
        return items.ToList();

        // Validates each part...
        static string? Validate(string? part)
        {
            if (part != null && part.Length == 0) part = null;
            return part;
        }
    }

    /// <summary>
    /// Invoked when the engine uses terminators
    /// </summary>
    static List<string?> GetPartsWithTerminators(IEngine engine, string? value)
    {
        throw null;

        // Validates each part...
        string? Validate(string? part)
        {
            return part;
        }
    }
}