namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database identifier.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract record Identifier
{
    /// <summary>
    /// Initialises a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(Engine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// The database engine descriptor this instance is associated with.
    /// </summary>
    public Engine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    public abstract string? Value { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given instances can be considered the same.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Compare(Identifier? x, Identifier? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        var sensitive = x.Engine.CaseSensitiveNames;
        return string.Compare(x.Value, y.Value, !sensitive) == 0;
    }

    /// <inheritdoc/>
    public virtual bool Equals(Identifier? other) => Compare(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the list of the dot-separated parts found in the given source value. Empty parts,
    /// after trimming them if needed, are returned as null ones. Engine terminators are always
    /// removed. The returned list is, by default, reduced by removing its empty or null heads.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> GetParts(Engine engine, string? source, bool reduce = true)
    {
        engine.ThrowWhenNull();

        if ((source = source.NullWhenEmpty(true)) is null) return [];

        var items = engine.UseTerminators
            ? GetRawParts(engine, source)
            : GetRawParts(source);

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
    /// Invoked to obtain the dot-separated parts of the given source value when the engine does
    /// not use any terminators.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static List<string?> GetRawParts(string source)
    {
        string?[] items = source.Contains('.') ? source.Split('.') : [source];
        for (int i = 0; i < items.Length; i++) items[i] = Validate(items[i]);
        return [.. items];

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? Validate(string? part)
        {
            if ((part = part.NullWhenEmpty(trim: true)) is null) return null;

            if (part.Contains(' ')) throw new ArgumentException(
                "Not-terminated identifier part cannot contain spaces.")
                .WithData(part);

            if (part.Contains('.')) throw new ArgumentException(
                "Not-terminated identifier part cannot contain dots.")
                .WithData(part);

            return part;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the dot-separated parts of the given source value when the engine
    /// terminators must be taken into consideration.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    static List<string?> GetRawParts(Engine engine, string source)
    {
        // No dots in source value...
        if (!source.Contains('.'))
        {
            source = source.Unwrap(engine.LeftTerminator, engine.RightTerminator, trim: true)!;
            source = Validate(source)!;
            return [source];
        }

        // Other cases...
        List<string?> items = Split(source, engine);
        for (int i = 0; i < items.Count; i++) items[i] = Validate(items[i], engine);
        return [.. items];

        /// <summary>
        /// Obtains the dot-separated candidate parts, before validation.
        /// </summary>
        static List<string?> Split(string source, Engine engine)
        {
            if (!engine.UseTerminators) return [.. source.Split('.')];

            var dots = GetDots(source, engine);
            if (dots.Count == 0) return [source];

            List<string?> parts = [];
            var ini = 0;
            int end, len;
            string str;

            for (int i = 0; i < dots.Count; i++)
            {
                end = dots[i];
                len = end - ini;
                str = source.Substring(ini, len); parts.Add(str);
                ini = end + 1;
            }
            end = source.Length;
            len = end - ini;
            str = source.Substring(ini, len); parts.Add(str);

            return parts;
        }

        /// <summary>
        /// Obtains the indexes of the unprotected dots.
        /// </summary>
        static List<int> GetDots(string source, Engine engine)
        {
            return engine.LeftTerminator == engine.RightTerminator
                ? GetDotsOne(source, engine.LeftTerminator)
                : GetDotsTwo(source, engine.LeftTerminator, engine.RightTerminator);
        }

        // Invoked when the terminators are both the same...
        static List<int> GetDotsOne(string source, char ch)
        {
            List<int> dots = [];
            bool found = false;

            for (int i = 0; i < source.Length; i++)
            {
                var c = source[i];
                if (c == '.') { if (!found) dots.Add(i); continue; }
                if (c == ch) { found = !found; continue; }
            }

            return dots;
        }

        // Invoked when the terminators are distinct characters...
        static List<int> GetDotsTwo(string source, char left, char right)
        {
            List<int> lefts = [];
            List<int> dots = [];

            for (int i = 0; i < source.Length; i++)
            {
                var c = source[i];
                if (c == '.') { dots.Add(i); continue; }
                if (c == left) { lefts.Add(i); continue; }
                if (c == right)
                {
                    if (lefts.Count == 0) continue;
                    var last = lefts[^1];
                    lefts.RemoveAt(lefts.Count - 1);
                    dots = [.. dots.Where(x => x < last)];
                }
            }

            return dots;
        }

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? Validate(string? part, Engine? engine = null)
        {
            if (engine is not null && engine.UseTerminators)
                part = part.Unwrap(engine.LeftTerminator, engine.RightTerminator, trim: true);

            if ((part = part.NullWhenEmpty(trim: true)) is null) return null;

            if (part.StartsWith('.') || part.EndsWith('.')) throw new ArgumentException(
                "Identifier part cannot begin or end with dots.")
                .WithData(part);

            // What follows is deleted because of scenarios where the engine's terminators are
            // embedded inside protected parts. Just keeping it for recording purposes.
            //if (engine is not null && engine.UseTerminators)
            //{
            //    if (part.Contains(engine.LeftTerminator)) throw new ArgumentException(
            //        "Identifier part cannot contain engine's left terminator.")
            //        .WithData(part);

            //    if (part.Contains(engine.RightTerminator)) throw new ArgumentException(
            //        "Identifier part cannot contain engine's right terminator.")
            //        .WithData(part);
            //}

            return part;
        }
    }
}