namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierPart"/>
public sealed class IdentifierPart : IIdentifierPart
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierPart(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierPart(IEngine engine, string? value) : this(engine)
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

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value { get; private set; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; private set; }
}