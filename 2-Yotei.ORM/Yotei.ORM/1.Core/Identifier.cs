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

    // ----------------------------------------------------

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
    /// Returns the list of the dot-separated parts found in the given value. Empty parts are
    /// returned as null ones, and engine terminators are always removed. The returned list is,
    /// by default, reduced by removing its empty or null heads.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> GetParts(Engine engine, string? value, bool reduce = true)
    {
        engine.ThrowWhenNull();

        if ((value = value.NullWhenEmpty(trim: true)) is null) return [];
        else
        {
            var items = engine.UseTerminators ? GetRawParts(engine, value) : GetRawParts(value);
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
    }

    /// <summary>
    /// Invoked to obtain the dot-separated parts of the given value when the engine does not
    /// use any terminators.
    /// </summary>
    static List<string?> GetRawParts(string value)
    {
        string?[] items = value.Contains('.') ? value.Split('.') : [value];
        for (int i = 0; i < items.Length; i++) items[i] = Validate(items[i]);
        return [.. items];

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? Validate(string? part)
        {
            if (part is null || part.Length == 0) return null;

            if (part.Contains(' ')) throw new ArgumentException(
                "Not-terminated identifier part cannot contain spaces.")
                .WithData(part);

            if (part.Contains('.')) throw new ArgumentException(
                "Not-terminated identifier part cannot contain dots.")
                .WithData(part);

            return part;
        }
    }

    /// <summary>
    /// Invoked to obtain the dot-separated parts of the given value when the engine terminators
    /// must be taken into consideration.
    /// </summary>
    static List<string?> GetRawParts(Engine engine, string value)
    {
        // No dots used...
        if (!value.Contains('.'))
        {
            value = value.Unwrap(engine.LeftTerminator, engine.RightTerminator, trim: false)!;
            value = Validate(value)!;
            return [value];
        }


        // TODO: GetRawParts when engine is used
        throw null;

        /// <summary>
        /// Invoked to validate the given part.
        /// </summary>
        static string? Validate(string? part, Engine? engine = null)
        {
            if (part is null || part.Length == 0) return null;

            if (part.StartsWith(' ') || part.EndsWith(' ')) throw new ArgumentException(
                "Identifier part cannot begin or end with spaces.")
                .WithData(part);

            if (part.StartsWith('.') || part.EndsWith('.')) throw new ArgumentException(
                "Identifier part cannot begin or end with dots.")
                .WithData(part);

            if (engine is not null)
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