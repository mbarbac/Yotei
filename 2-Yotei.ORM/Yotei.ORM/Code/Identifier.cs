#pragma warning disable IDE0305

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Provides methods for working with identifiers.
/// </summary>
public static class Identifier
{
    /// <summary>
    /// Returns a new identifier using the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, string? value = null)
    {
        var item = new IdentifierChain(engine, value);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0] :
            item;
    }

    /// <summary>
    /// Returns a new identifier using the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IIdentifier Create(IEngine engine, IEnumerable<string?> range)
    {
        var item = new IdentifierChain(engine, range);

        return
            item.Count == 0 ? new IdentifierPart(engine) :
            item.Count == 1 ? item[0] :
            item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given identifier matches the given specifications.
    /// <br/> Comparison is performed by comparing parts from right to left, where any empty or
    /// null one is taken as an implicit match.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static bool Match(IIdentifier item, string? specs)
    {
        item.ThrowWhenNull();

        if ((specs = specs.NullWhenEmpty()) == null) return true;

        var engine = item.Engine;
        var target = new IdentifierChain(engine, specs);
        var source = item is IIdentifierChain chain
            ? chain
            : new IdentifierChain(engine, (IIdentifierPart)item);

        for (int i = 0; ; i++)
        {
            if (i >= target.Count) break;
            if (i >= source.Count)
            {
                while (i < target.Count)
                {
                    var value = target[^(i + 1)].UnwrappedValue;
                    if (value != null) return false;
                    i++;
                }
                break;
            }

            var tvalue = target[^(i + 1)].UnwrappedValue; if (tvalue == null) continue;
            var svalue = source[^(i + 1)].UnwrappedValue;
            if (string.Compare(svalue, tvalue, !engine.CaseSensitiveNames) != 0) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the dot-separated single parts of the given value, where empty ones
    /// are transformed to nulls, and any engine terminators are removed. By default the returned
    /// list is reduced by removing the empty or null heading parts, which may lead to an empty
    /// one.
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
            var parts = GetPartsNoTerminators(value);
            for (int i = 0; i < parts.Count; i++) parts[i] = Validate(parts[i]);
            return parts;
        }

        // Single wrapped part detected...
        if (token is IStrTokenWrapped wrapped)
        {
            value = wrapped.Payload.ToString(); // No-terminators included!
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

                str = Validate(item.ToString()); // Adding the string representation...
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