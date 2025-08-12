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
    public static IIdentifier CreateFromValue(IEngine engine, string? value)
    {
        var item = new IdentifierChain(engine, value);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0]
            : item;
    }

    /// <summary>
    /// Creates a new identifier using the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IIdentifier CreateFromRange(IEngine engine, IEnumerable<string?> range)
    {
        var item = new IdentifierChain(engine, range);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0]
            : item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given identifier matches the given specifications, or not.
    /// <br/> Matching is performed by comparing the identifier parts with the given specs, from
    /// right to left, where an empty or null specification is taken as an implicit match.
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
            : new IdentifierChain(engine, [(IIdentifierPart)item]);

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

            if (string.Compare(svalue, tvalue, !engine.CaseSensitiveNames) != 0) return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of dot-separated parts found in the given source value. Empty ones
    /// are transformed into null values, and engine terminators are always removed. By default,
    /// the returned collection is reduced by removing empty heading parts.
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
                if (parts[0] is null) parts.RemoveAt(0);
                else break;
            }
        }

        return parts;
    }

    /// <summary>
    /// Invoked to obtain the collection of dot-separated parts when the engine DOES NOT use any
    /// terminators.
    /// </summary>
    static List<string?> GetPartsNoTerminators(string? value)
    {
        if (value is null) return [];

        string?[] items = value.Contains('.') ? value.Split('.') : [value];
        for (int i = 0; i < items.Length; i++) items[i] = Validate(items[i]);
        return items.ToList();

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? Validate(string? part)
        {
            if (part is null) return null;
            if (part.Length == 0) return null;

            if (part.Contains(' ')) throw new ArgumentException(
                "Not terminated identifier parts cannot contain spaces.")
                .WithData(part);

            if (part.Contains('.')) throw new ArgumentException(
                "Not terminated identifier parts cannot contain dots.")
                .WithData(part);

            return part;
        }
    }

    /// <summary>
    /// Invoked to obtain the collection of dot-separated parts when the engine USES terminators.
    /// </summary>
    static List<string?> GetPartsWithTerminators(IEngine engine, string? value)
    {
        // Trivial cases...
        if (value is null) return [];
        if (!value.Contains('.'))
        {
            value = value.UnWrap(engine.LeftTerminator, engine.RightTerminator, trim: false);
            value = Validate(value, engine, preventTerminators: true);

            return [value];
        }

        // Finding wrapped parts...
        var comparison = engine.CaseSensitiveNames ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrapTokenizer(engine.LeftTerminator, engine.RightTerminator) { Comparison = comparison };
        var token = tokenizer.Tokenize(value, reduce: true);

        // No wrapped parts detected...
        if (token is StrTokenText)
        {
            var parts = GetPartsNoTerminators(value);
            for (int i = 0; i < parts.Count; i++)
                parts[i] = Validate(parts[i], engine, preventTerminators: true);

            return parts;
        }

        // Single wrapped part...
        if (token is StrTokenWrapped wrapped)
        {
            value = wrapped.Payload.ToString();
            value = Validate(value, engine, preventTerminators: false);

            return [value];
        }

        // Chained part...
        if (token is StrTokenChain chain)
        {
            var dotter = new StrLiteralTokenizer('.') { Comparison = comparison };
            token = dotter.Tokenize(chain, reduce: true);
            chain = token is StrTokenChain other ? other : new StrTokenChain([token]);

            List<string?> parts = [];
            var needed = false;
            var predot = false;

            for (int i = 0; i < chain.Count; i++)
            {
                var item = chain[i];

                if (needed) // Dot separator expected...
                {
                    if (item is not StrTokenLiteral) throw new ArgumentException(
                        "No dot separator found between parts.")
                        .WithData(value);
                }

                if (item is StrTokenLiteral) // Dotted separator found, take note and continue...
                {
                    if (predot || parts.Count == 0) parts.Add(null);

                    needed = false;
                    predot = true;
                    continue;
                }

                // Adding the part...
                var str = item is StrTokenWrapped xwrapped
                    ? xwrapped.Payload.ToString()
                    : item.ToString();

                str = Validate(str, engine, item is not StrTokenWrapped);
                parts.Add(str);
                needed = true;
                predot = false;
            }

            if (predot) parts.Add(null); // Pending dot...
            return parts;
        }

        // Unknown token...
        throw new ArgumentException(
            "Unknown token while finding wrapped parts.")
            .WithData(value);

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? Validate(string? part, IEngine engine, bool preventTerminators)
        {
            if (part is null) return null;
            if (part.Length == 0) return null;

            if (part.StartsWith(' ') || part.EndsWith(' ')) throw new ArgumentException(
                "Identifier part cannot start or end with spaces.")
                .WithData(part);

            if (part.StartsWith('.') || part.EndsWith('.')) throw new ArgumentException(
                "Identifier part cannot start or end with dots.")
                .WithData(part);

            if (preventTerminators)
            {
                if (part.Contains(engine.LeftTerminator)) throw new ArgumentException(
                    "Identifier part cannot contain engine's left terminator.")
                    .WithData(part);

                if (part.Contains(engine.RightTerminator)) throw new ArgumentException(
                    "Identifier part cannot contain engine's right terminator.")
                    .WithData(part);
            }

            return part;
        }
    }
}