using Yotei.ORM.Internal;

namespace Yotei.ORM.Code;

// ========================================================
public static class Identifier
{
    /// <summary>
    /// Obtains the dotted-separted identifier parts contained in the given value, using the
    /// separators of the given engine to protect the embedded ones, if any. An empty list is
    /// returned if that value is null, empty, or when all identified parts are null or empty
    /// themselves.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static List<string?> GetParts(
        IEngine engine, string? value, bool reduce = true, bool trim = true)
    {
        engine.ThrowWhenNull();

        var parts = engine.UseTerminators
            ? GetPartsWithTerminators(engine, value, trim)
            : GetPartsNoTerminators(value, trim);

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

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the engine uses no terminators.
    /// </summary>
    static List<string?> GetPartsNoTerminators(string? value, bool trim)
    {
        value = value.NullWhenEmpty(trim);
        if (value is null) return [];

        if (value.Contains('.'))
        {
            var items = value.Split('.');
            var parts = items.Select(x => ValidateNoTerminators(x, trim)).ToList();
            return parts;
        }
        else
        {
            var parts = new List<string?>(1) { ValidateNoTerminators(value, trim) };
            return parts;
        }
    }

    /// <summary>
    /// Invoked to validate a part when no terminators are used.
    /// </summary>
    static string? ValidateNoTerminators(string? value, bool trim)
    {
        value = value.NullWhenEmpty(trim);

        if (value is not null)
        {
            if (value.Contains(' ')) throw new ArgumentException(
            "Identifier parts cannot contain spaces.").WithData(value);

            if (value.Contains('.')) throw new ArgumentException(
                "Identifier parts cannot contain dots.").WithData(value);
        }
        return value;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked when the engine uses terminators.
    /// </summary>
    static List<string?> GetPartsWithTerminators(IEngine engine, string? value, bool trim)
    {
        // Trivial case...
        value = value.NullWhenEmpty(trim);
        if (value is null) return [];

        // Tokenizing...
        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(value);

        // No parts...
        if (token is IStrTokenText text)
        {
            var parts = GetPartsNoTerminators(text.Payload, trim);
            return parts;
        }

        // Single part...
        if (token is IStrTokenWrapped wrapped)
        {
            var part = wrapped.Payload.ToString();
            var parts = new List<string?>(1) { ValidateWithTerminators(engine, part, trim) };
            return parts;
        }

        // Chained parts...
        if (token is IStrTokenChain chain)
        {
            throw null;
        }

        // Unknown token...
        throw new ArgumentException("Unknown token found.").WithData(value);
    }

    /// <summary>
    /// Invoked to validate a part when terminators are used.
    /// </summary>
    static string? ValidateWithTerminators(IEngine engine, string? value, bool trim)
    {
        throw null;
    }
}