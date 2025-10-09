namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public record IdentifierUnit : Identifier
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierUnit(Engine engine, string? value = null) : base(engine) => Value = value;

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public override string? Value
    {
        get;
        init;
    }
}