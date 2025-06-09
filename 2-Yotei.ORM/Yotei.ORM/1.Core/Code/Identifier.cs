namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Returns a new identifier of the appropriate type using the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value = null)
    {
        var item = new IdentifierChain(engine, value);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0]
            : item;
    }

    /// <summary>
    /// Returns a new identifier of the appropriate type using the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, IEnumerable<string?> range)
    {
        var item = new IdentifierChain(engine, range);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0]
            : item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance matches the given specifications or not.
    /// <br/> Comparison is performed comparing the respective parts, from right to left, where
    /// an empty or null specification is taken as an implicit match.
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
                    var value = target[^(i + 1)].UnwrappedValue;
                    if (value is not null) return false;
                    i++;
                }
                break;
            }

            var tvalue = target[^(i + 1)].UnwrappedValue; if (tvalue is null) continue;
            var svalue = source[^(i + 1)].UnwrappedValue;

            if (string.Compare(svalue, tvalue, !engine.CaseSensitiveNames) != 0) return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list containing the dot-separated parts found in the given source. Empty parts
    /// are transformed into null values, and engine terminators are also removed. By default,
    /// the returned list is reduced by removing the empty or null heading parts.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> GetParts(IEngine engine, string? value, bool reduce = true)
    {
        engine.ThrowWhenNull();

        var parts = engine.UseTerminators
            ? GetPartsWithTerminator(engine, value)
            : GetPartsNoTerminator(value);

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
    /// Invoked when the engine uses NO terminators.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static List<string?> GetPartsNoTerminator(string? value)
    {
        if (value is null) return [];

        string?[] items = value.Contains('.') ? value.Split('.') : [value];

        for (int i = 0; i < items.Length; i++) items[i] = ValidatePart(items[i]);
        return items.ToList();

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? ValidatePart(string? part)
        {
            if (part is not null)
            {
                if (part.Length == 0) part = null;
                else
                {
                    if (part.Contains(' ')) throw new ArgumentException(
                    "Not terminated identifier parts cannot contain spaces.")
                    .WithData(part);

                    if (part.Contains('.')) throw new ArgumentException(
                        "Not terminated identifier parts cannot contain dots.")
                        .WithData(part);
                }
            }

            return part;
        }
    }

    /// <summary>
    /// Invoked when the engine uses terminators.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static List<string?> GetPartsWithTerminator(IEngine engine, string? value)
    {
        // Trivial cases...
        if (value is null) return [];
        if (!value.Contains('.'))
        {
            value = value.UnWrap(engine.LeftTerminator, engine.RightTerminator, trim: false);
            value = ValidatePart(engine, value, preventTerminators: true);
            
            return [value];
        }

        // Finding wrapped parts...
        var comparison = engine.CaseSensitiveNames ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped(engine.LeftTerminator, engine.RightTerminator) { Comparison = comparison };
        var token = tokenizer.Tokenize(value, reduce: true);

        // No wrapped parts detected...
        if (token is IStrTokenText)
        {
            var parts = GetPartsNoTerminator(value);
            for (int i = 0; i < parts.Count; i++)
                parts[i] = ValidatePart(engine, parts[i], preventTerminators: true);

            return parts;
        }

        // Single wrapped part...
        if (token is IStrTokenWrapped wrapped)
        {
            value = wrapped.Payload.ToString();
            value = ValidatePart(engine, value, preventTerminators: false);

            return [value];
        }

        // Chain of arbitrary part...
        if (token is IStrTokenChain chain)
        {
            var dotter = new StrTokenizerLiteral('.') { Comparison = comparison };
            token = dotter.Tokenize(chain, reduce: true);
            chain = token is IStrTokenChain other ? other : new StrTokenChain([token]);

            List<string?> parts = [];
            var needed = false;
            var predot = false;

            for (int i = 0; i < chain.Count; i++) // Processing elements in the chain...
            {
                var item = chain[i];

                if (needed) // Dot separator expected...
                {
                    if (item is not IStrTokenLiteral) throw new ArgumentException(
                        "No dot separator found between parts.")
                        .WithData(value);
                }

                if (item is IStrTokenLiteral) // Dotted separator found, take note and continue...
                {
                    if (predot || parts.Count == 0) parts.Add(null);

                    needed = false;
                    predot = true;
                    continue;
                }

                // Adding the part...
                var str = item is IStrTokenWrapped xwrapped
                    ? xwrapped.Payload.ToString()
                    : item.ToString();

                str = ValidatePart(engine, str, item is not IStrTokenWrapped);
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
            .WithData(value)
            .WithData(engine);

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? ValidatePart(IEngine engine, string? part, bool preventTerminators)
        {
            if (part is not null)
            {
                if (part.Length == 0) part = null;
                else
                {
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
                            .WithData(part)
                            .WithData(engine);

                        if (part.Contains(engine.RightTerminator)) throw new ArgumentException(
                            "Identifier part cannot contain engine's right terminator.")
                            .WithData(part)
                            .WithData(engine);
                    }
                }
            }

            return part;
        }
    }
}