namespace Yotei.ORM.Code;

// ========================================================
public partial class Identifier
{
    /// <summary>
    /// Splits the given source string into its NOT-terminated dot-separated parts, using the
    /// rules of the given engine, and removing (reducing) or not the empty of null heading parts,
    /// as requested.
    /// <br/> Any empty source part is translated into a null one, which may be reduced.
    /// <br/> If no reduce is requested, the list always contains at least one element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static List<string?> GetParts(IEngine engine, string? value, bool reduce = false)
    {
        ArgumentNullException.ThrowIfNull(engine);

        value = value.NullWhenEmpty(trim: true);
        if (value == null) return reduce ? [] : [null];

        var items = engine.UseTerminators
            ? GetPartsWithTerminators(value, engine)
            : GetPartNoTerminators(value);
        
        if (reduce)
        {
            while (items.Count > 0)
            {
                if (items[0] == null) items.RemoveAt(0);
                else break;
            }
        }

        items.TrimExcess();
        return items;
    }

    /// <summary>
    /// Invoked when the engine does not use terminators.
    /// </summary>
    static List<string?> GetPartNoTerminators(string value)
    {
        string?[] items = value.Contains('.') ? value.Split('.') : [value];

        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i] = items[i].NullWhenEmpty(trim: true);
            if (item != null)
            {
                if (item.Contains(' ')) throw new ArgumentException(
                    "Not terminated parts cannot contain embedded spaces.")
                    .WithData(item, "part");
            }
        }

        return [.. items];
    }

    /// <summary>
    /// Invoked when the engine uses terminators.
    /// </summary>
    static List<string?> GetPartsWithTerminators(string value, IEngine engine)
    {
        var items = GetFirstLevelParts(value, engine);

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i] = items[i]
                .Unwrap(engine.LeftTerminator, engine.RightTerminator, trim: true)
                .NullWhenEmpty(trim: true);

            if (item is not null)
            {
                if (item.StartsWith('.') || item.EndsWith('.')) throw new ArgumentException(
                    "Identifier part cannot begin or end with dots.")
                    .WithData(item);
            }
        }

        return items;
    }

    /// <summary>
    /// Invoked to split the given value into its not protected first level parts.
    /// </summary>
    static List<string?> GetFirstLevelParts(string value, IEngine engine)
    {
        var dots = GetDots(engine, value);
        List<string?> parts = [];
        int ini = 0;
        int end, len;
        string str;

        for (int i = 0; i < dots.Count; i++)
        {
            end = dots[i];
            len = end - ini;
            str = value.Substring(ini, len); parts.Add(str);
            ini = end + 1;
        }
        end = value.Length;
        len = end - ini;
        str = value.Substring(ini, len); parts.Add(str);

        return parts;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the indexes of the first-level dots in the given value. Dots wrapped between the
    /// engine's terminators, if used, are not reported.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> GetDots(IEngine engine, string? value)
    {
        ArgumentNullException.ThrowIfNull(engine);

        return
            value == null ? [] :
            !engine.UseTerminators ? GetDotsNoTerminators(value) :
            engine.LeftTerminator == engine.RightTerminator
            ? GetDotsSameTerminators(value, engine.LeftTerminator)
            : GetDotsDiverseTerminators(value, engine.LeftTerminator, engine.RightTerminator);
    }

    /// <summary>
    /// Invoked when no engine terminators are used.
    /// </summary>
    static List<int> GetDotsNoTerminators(string value)
    {
        List<int> dots = [];
        for (int i = 0; i < value.Length; i++) if (value[i] == '.') dots.Add(i);
        return dots;
    }

    /// <summary>
    /// Invoked when the engine terminators are the same.
    /// </summary>
    static List<int> GetDotsSameTerminators(string value, char terminator)
    {
        List<int> dots = [];
        bool found = false;

        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (c == terminator) { found = !found; continue; }
            if (c == '.') { if (!found) dots.Add(i); continue; }
        }
        return dots;
    }

    /// <summary>
    /// Invoked when the engine terminators are NOT the same.
    /// </summary>
    static List<int> GetDotsDiverseTerminators(string value, char left, char right)
    {
        List<int> temps = [];
        List<int> dots = [];

        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (c == left) { temps.Add(i); continue; }
            if (c == right)
            {
                if (temps.Count == 0) continue;

                var last = temps[^1];
                temps.RemoveAt(temps.Count - 1);
                dots = [.. dots.Where(x => x < last)];
            }
            if (c == '.') { dots.Add(i); continue; }
        }
        return dots;
    }
}