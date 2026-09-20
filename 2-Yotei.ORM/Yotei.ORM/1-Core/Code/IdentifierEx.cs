namespace Yotei.ORM.Code;

// ========================================================
public partial class Identifier
{
    /// <summary>
    /// Splits the given source string into its not-terminated dot-separated parts, using the
    /// rules of the given engine, and removing (reducing) or not the empty of null heading parts,
    /// as requested. Any empty source part is translated into a null one.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> GetParts(IEngine engine, string? value, bool reduce = false)
    {
        ArgumentNullException.ThrowIfNull(engine);

        value = value.NullWhenEmpty(trim: true);
        if (value == null) return reduce ? [] : [null];

        var items = engine.UseTerminators ? WithTerminators(value, engine) : NoTerminators(value);
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

        // ------------------------------------------------

        /// <summary>
        /// Invoked when the engine does not use terminators.
        /// </summary>
        static List<string?> NoTerminators(string value)
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

        // ------------------------------------------------

        /// <summary>
        /// Invoked when the engine uses terminators.
        /// </summary>
        static List<string?> WithTerminators(string value, IEngine engine)
        {
            var items = GetFirstLevel(value, engine);

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
        static List<string?> GetFirstLevel(string value, IEngine engine)
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
        /// Obtains the indexes of the not protected dots in the given value.
        /// </summary>
        static List<int> GetDots(string value, IEngine engine)
        {
            return engine.LeftTerminator == engine.RightTerminator
                ? GetDotsSameTerminators(value, engine.LeftTerminator)
                : GetDotsDiverseTerminators(value, engine.LeftTerminator, engine.RightTerminator);
        }

        /// <summary>
        /// Obtains the indexes of the not protected dots in the given value when the terminators
        /// are the same.
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
        /// Obtains the indexes of the not protected dots in the given value when the terminators
        /// are not the same.
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
}