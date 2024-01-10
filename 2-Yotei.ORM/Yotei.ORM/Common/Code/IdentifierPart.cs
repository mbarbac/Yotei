namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifierPart"/>
/// </summary>
public sealed class IdentifierPart : IIdentifierPart
{
    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static IdentifierPart Empty { get; } = new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public IdentifierPart() { }

    /// <summary>
    /// Returns a new instance using the given value, which does not suppport embedded dots or
    /// spaces, or special characters.
    /// </summary>
    /// <param name="value"></param>
    public IdentifierPart(string? value)
    {
        value = value.NullWhenEmpty(); if (value is not null)
        {
            if (value.ContainsAny(INVALID_CHARS)) throw new ArgumentException(
                "Value contains invalid chararcters.")
                .WithData(value);
        }
        Value = value;
        UnwrappedValue = value;
    }
    static readonly string INVALID_CHARS = @" .+-/*[]{}'`´()^=?%&!\";

    /// <summary>
    /// Initializes a new instance using the given value and the terminator rules of the given
    /// engine.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierPart(IEngine engine, string? value = null)
    {
        engine.ThrowWhenNull();

        if ((value = value.NullWhenEmpty()) == null) return;

        var items = engine.UnwrappedIndexes(value, '.');
        if (items.Count > 0) throw new ArgumentException(
            "Unwrapped embedded dots not allowed for single-part identifiers.")
            .WithData(value)
            .WithData(engine);

        items = engine.UnwrappedIndexes(value, ' ');
        if (items.Count > 0) throw new ArgumentException(
            "Unwrapped embedded spaces not allowed for single-part identifiers.")
            .WithData(value)
            .WithData(engine);

        UnwrappedValue = engine.UseTerminators
            ? value.UnWrap(engine.LeftTerminator, engine.RightTerminator).NullWhenEmpty()
            : value;

        Value = engine.UseTerminators && UnwrappedValue != null
            ? $"{engine.LeftTerminator}{UnwrappedValue}{engine.RightTerminator}"
            : UnwrappedValue;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? UnwrappedValue { get; private set; }
}