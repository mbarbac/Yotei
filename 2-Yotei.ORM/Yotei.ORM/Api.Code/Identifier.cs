using Yotei.ORM.Internal;

namespace Yotei.ORM;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Extracts the dotted-separated parts from the given value, unwrapped from the engine's
    /// terminators, if any, and where any empty part is transformed into a null string. Finally,
    /// all all heading null parts are removed, which may produce an empty list.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<string?> GetParts(this IEngine engine, string? value)
    {
        engine.ThrowWhenNull();

        var parts = engine.UseTerminators
            ? GetPartsWithTerminators(engine, value)
            : GetPartsNoTerminators(value);

        while (parts.Count > 0) { if (parts[0] == null) parts.RemoveAt(0); else break; }
        return parts;
    }

    /// <summary>
    /// Invoked when the engine uses no terminators.
    /// </summary>
    static List<string?> GetPartsNoTerminators(string? value)
    {
        if (value == null) return [];

        string?[] items = value.Contains('.') ? value.Split('.') : [value];
        for (int i = 0; i < items.Length; i++) items[i] = Validate(items[i]);
        return items.ToList();

        // Validates each part...
        string? Validate(string? part)
        {
            if (part != null)
            {
                if (part.Length == 0) part = null;
                else
                {
                    if (part.Contains(' ')) throw new ArgumentException(
                        "Single identifier part cannot contain spaces.")
                        .WithData(value);
                }
            }
            return part;
        }
    }

    /// <summary>
    /// Invoked when the engine uses terminators.
    /// </summary>
    static List<string?> GetPartsWithTerminators(IEngine engine, string? value)
    {
        if (value == null) return [];

        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(value);

        // No wrapped parts detected...
        if (token is IStrTokenText)
        {
            return GetPartsNoTerminators(value);
        }

        // Single wrapped part detected...
        if (token is IStrTokenWrapped wrapped)
        {
            value = wrapped.Payload.ToString();
            value = Validate(value);
            return [value];
        }

        // Chain of parts...
        if (token is IStrTokenChain chain)
        {
            var dotter = new StrFixedTokenizer('.');
            token = dotter.Tokenize(chain);
            chain = token is IStrTokenChain other ? other : new StrTokenChain(token);

            string? str;
            var needed = false;
            var predot = false;
            List<string?> parts = [];

            for (int i = 0; i < chain.Count; i++)
            {
                var item = chain[i];

                if (needed) // Dot separator expected...
                {
                    if (item is not IStrTokenFixed) throw new ArgumentException(
                        "No dot separator found between parts.")
                        .WithData(value);
                }

                if (item is IStrTokenFixed) // Dotted separator found...
                {
                    if (predot || parts.Count == 0) parts.Add(null);

                    needed = false;
                    predot = true;
                    continue;
                }

                if (item is IStrTokenWrapped xwrapped) // Transform to payload...
                {
                    item = xwrapped.Payload;
                }

                str = Validate(item.ToString());
                parts.Add(str);
                needed = true;
                predot = false;
            }

            if (predot) parts.Add(null);
            return parts;
        }

        // Unknown token...
        throw new ArgumentException("Invalid token found.").WithData(value);

        // Validates each part...
        string? Validate(string? part)
        {
            if (part != null)
            {
                if (part.Length == 0) part = null;
                else
                {
                    if (part.StartsWith(' ') || part.EndsWith(' ')) throw new ArgumentException(
                        "Identifier part cannot start or end with spaces.")
                        .WithData(value);

                    if (part.StartsWith('.') || part.EndsWith('.')) throw new ArgumentException(
                        "Identifier part cannot start or end with dots.")
                        .WithData(value);

                    if (part.Contains(engine.LeftTerminator)) throw new ArgumentException(
                        "Identifier part cannot contain engine's left terminator.")
                        .WithData(value)
                        .WithData(engine.LeftTerminator);

                    if (part.Contains(engine.RightTerminator)) throw new ArgumentException(
                        "Identifier part cannot contain engine's right terminator.")
                        .WithData(value)
                        .WithData(engine.RightTerminator);
                }
            }
            return part;
        }
    }
}