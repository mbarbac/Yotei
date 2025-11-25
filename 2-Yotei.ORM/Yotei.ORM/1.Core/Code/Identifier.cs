namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Helpers for <see cref="IIdentifier"/> instances.
/// </summary>
public static class Identifier
{
    /// <summary>
    /// Creates a new identifier using the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value)
    {
        var chain = new IdentifierChain(engine, value);
        return
            chain.Count == 0 ? new IdentifierUnit(engine) :
            chain.Count == 1 ? chain[0] :
            chain;
    }

    /// <summary>
    /// Creates a new identifier using the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, IEnumerable<string?> range)
    {
        var chain = new IdentifierChain(engine, range);
        return
            chain.Count == 0 ? new IdentifierUnit(engine) :
            chain.Count == 1 ? chain[0] :
            chain;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IIdentifier.Match(string?)"/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static bool Match(IIdentifier item, string? specs)
    {
        item.ThrowWhenNull();

        var engine = item.Engine;
        var target = new IdentifierChain(engine, specs);
        if (target.Value is null) return true;

        var source = item is IIdentifierChain chain
            ? chain
            : new IdentifierChain(engine, [(IIdentifierUnit)item]);

        // Looping...
        for (int i = 0; ; i++)
        {
            if (i >= target.Count) break;
            if (i >= source.Count)
            {
                while (i < target.Count)
                {
                    var value = target[^(i + 1)].RawValue;
                    if (value is not null) return false;
                    i++;
                }
                break;
            }

            var tvalue = target[^(i + 1)].RawValue; if (tvalue is null) continue;
            var svalue = source[^(i + 1)].RawValue;

            if (!engine.SameNames(svalue, tvalue)) return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

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

        value = value.NullWhenEmpty(true);
        if (value is null) return reduce ? [] : [null];

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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the raw dot-separated parts of the given value when the engine does
    /// not use terminators.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static List<string?> GetRawParts(string value)
    {
        string?[] items = value.Contains('.') ? value.Split('.') : [value];

        for (int i = 0; i < items.Length; i++)
        {
            var part = items[i] = items[i].NullWhenEmpty(true);
            if (part is not null)
            {
                if (part.Contains(' ')) throw new ArgumentException(
                "Not-terminated identifier part cannot contain spaces.")
                .WithData(part);

                if (part.Contains('.')) throw new ArgumentException(
                    "Not-terminated identifier part cannot contain dots.")
                    .WithData(part);
            }
        }
        return [.. items];
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the raw dot-separated parts of the given value when the engine uses
    /// terminators.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="engine"></param>
    /// <returns></returns>
    static List<string?> GetRawParts(string value, IEngine engine)
    {
        var parts = engine.UseTerminators ? Split(value, engine) : [.. value.Split('.')];
        for (int i = 0; i < parts.Count; i++)
        {
            var part = parts[i];
            part = engine.UseTerminators ? part.Unwrap(engine.LeftTerminator, engine.RightTerminator, true) : part;
            part = parts[i] = part.NullWhenEmpty(true);

            if (part is not null)
            {
                if (part.StartsWith('.') || part.EndsWith('.')) throw new ArgumentException(
                    "Identifier part cannot begin or end with dots.")
                    .WithData(part);
            }
        }
        return parts;

        /// <summary>
        /// Splits the given value into dot-separated parts when terminators are used.
        /// </summary>
        static List<string?> Split(string value, IEngine engine)
        {
            var dots = GetDots(value, engine);
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

        /// <summary>
        /// Obtains the indexes of the unprotected dots.
        /// </summary>
        static List<int> GetDots(string value, IEngine engine)
        {
            return engine.LeftTerminator == engine.RightTerminator
                ? GetDotsOne(value, engine.LeftTerminator)
                : GetDotsTwo(value, engine.LeftTerminator, engine.RightTerminator);
        }

        /// <summary>
        /// Obtains the indexes of the unprotected dots when both terminators are the same.
        /// </summary>
        static List<int> GetDotsOne(string value, char ch)
        {
            List<int> dots = [];
            bool found = false;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == ch) { found = !found; continue; }
                if (c == '.') { if (!found) dots.Add(i); continue; }
            }
            return dots;
        }

        /// <summary>
        /// Obtains the indexes of the unprotected dots when terminators differ.
        /// </summary>
        static List<int> GetDotsTwo(string value, char left, char right)
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
}