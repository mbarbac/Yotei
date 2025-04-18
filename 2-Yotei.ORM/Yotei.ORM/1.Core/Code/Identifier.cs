namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Returns a list containing the dot-separated parts found in the given source. Empty parts
    /// are transformed into null values and engine terminators removed. By default the returned
    /// list is reduced by removing the empty or null heading parts.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the engine uses NO terminators.
    /// </summary>
    static List<string?> GetPartsNoTerminators(string? value)
    {
        if (value == null) return [];
        else
        {
            string?[] items = value.Contains('.') ? value.Split('.') : [value];

            for (int i = 0; i < items.Length; i++) items[i] = ValidateNoTerminators(items[i]);
            return items.ToList();
        }
    }

    /// <summary>
    /// Invoked to validate the given part when the engine uses NO terminators.
    /// </summary>
    static string? ValidateNoTerminators(string? part)
    {
        if (part != null)
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the engine uses terminators.
    /// </summary>
    static List<string?> GetPartsWithTerminators(IEngine engine, string? value)
    {
        // Trivial cases....
        if (value == null) return [];
        if (!value.Contains('.'))
        {
            value = value.UnWrap(engine.LeftTerminator, engine.RightTerminator, trim: false);
            value = ValidateWithTerminators(engine, value, preventTerminators: true);

            return [value];
        }

        // Finding wrapped parts...
        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(value);

        token = token.Reduce(engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase);

        // No wrapped parts detected...
        if (token is StrTokenText)
        {
            var parts = GetPartsNoTerminators(value);

            for (int i = 0; i < parts.Count; i++)
                parts[i] = ValidateWithTerminators(engine, parts[i], preventTerminators: true);

            return parts;
        }

        // Single wrapped part...
        if (token is StrTokenWrapped wrapped)
        {
            value = wrapped.Payload.ToString();
            value = ValidateWithTerminators(engine, value, preventTerminators: false);

            return [value];
        }

        // Chain of arbitrary parts...
        if (token is StrTokenChain chain)
        {
            var dotter = new StrLiteralTokenizer('.');
            token = dotter.Tokenize(chain);
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

                str = ValidateWithTerminators(engine, str, item is not StrTokenWrapped);
                parts.Add(str);
                needed = true;
                predot = false;
            }

            if (predot) parts.Add(null); // We have a pending dot...
            return parts;
        }

        // Unknown token...
        throw new ArgumentException(
            "Unknown token while finding wrapped parts.")
            .WithData(value)
            .WithData(engine);
    }

    /// <summary>
    /// Invoked to validate the given part when the engine uses terminators.
    /// </summary>
    static string? ValidateWithTerminators(IEngine engine, string? part, bool preventTerminators)
    {
        if (part != null)
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