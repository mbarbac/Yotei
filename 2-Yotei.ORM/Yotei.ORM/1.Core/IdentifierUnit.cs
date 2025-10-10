namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a single-part database identifier.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public record IdentifierUnit : Identifier
{
    string? _Value;
    string? _RawValue;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierUnit(Engine engine, string? value = null) : base(engine) => Value = value;

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc cref="Identifier.Compare(Identifier?, Identifier?)"/>
    public static bool Compare(IdentifierUnit? x, IdentifierUnit? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return Identifier.Compare(x, y);
    }

    /// <inheritdoc/>
    public virtual bool Equals(IdentifierUnit? other) => Compare(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string? Value
    {
        get => _Value;
        init
        {
            if (value is null) { _Value = _RawValue = null; return; }

            var parts = GetParts(Engine, value); switch (parts.Count)
            {
                case 0: _Value = _RawValue = null; break;

                case 1:
                    _RawValue = value = parts[0];
                    _Value = value is null
                        ? null
                        : Engine.UseTerminators
                            ? $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}"
                            : value;
                    break;

                default:
                    throw new ArgumentException("More than one identifier part detected.")
                    .WithData(value);
            }
        }
    }

    /// <summary>
    /// The raw value (without engine terminators) carried by this identifier, or null if it
    /// represents an empty or missed one.
    /// </summary>
    public string? RawValue
    {
        get => _RawValue;
        init => Value = value;
    }
}