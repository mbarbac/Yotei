namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierSinglePart"/>
public sealed class IdentifierSinglePart : IIdentifierSinglePart
{
    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static IdentifierSinglePart Empty { get; } = new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public IdentifierSinglePart() { }

    /// <summary>
    /// Initializes a new instance using the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierSinglePart(IEngine engine, string? value)
    {
        engine.ThrowWhenNull();

        if ((value = value.NullWhenEmpty()) == null) return;

        var indexes = engine.UnwrappedIndexes(value, '.');
        if (indexes.Count > 0) throw new ArgumentException(
            "Unwrapped dots not allowed in single-part identifiers.")
            .WithData(value)
            .WithData(engine);

        indexes = engine.UnwrappedIndexes(value, ' ');
        if (indexes.Count > 0) throw new ArgumentException(
            "Unwrapped spaces not allowed in single-part identifiers.")
            .WithData(value)
            .WithData(engine);

        UnwrappedValue = engine.UseTerminators
            ? value.UnWrap(engine.LeftTerminator, engine.RightTerminator).NullWhenEmpty()
            : value;

        Value = engine.UseTerminators && UnwrappedValue != null
            ? $"{engine.LeftTerminator}{UnwrappedValue}{engine.RightTerminator}"
            : UnwrappedValue;
    }

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public string? Value { get; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; }
}