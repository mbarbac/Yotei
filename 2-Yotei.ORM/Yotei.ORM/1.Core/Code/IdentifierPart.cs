namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierPart"/>
public class IdentifierPart : IIdentifierPart
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
    public IdentifierPart(IEngine engine, string? value) : this(engine) => Value = value;

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifier? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IIdentifierPart valid) return false;

        return string.Compare(Value, valid.Value) == 0;
    }

    /// <summary>
    /// Determines whether the current object is equal to the other given one, using the given
    /// comparison method.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Equals(IIdentifier? other, StringComparison comparison)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || other is not IIdentifierPart valid) return false;

        return string.Compare(Value, valid.Value, comparison) == 0;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IIdentifierPart);

    // Equality operator.
    public static bool operator ==(IdentifierPart? x, IIdentifier? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // Inequality operator.
    public static bool operator !=(IdentifierPart? x, IIdentifier? y) => !(x == y);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        if (Value is not null) code = HashCode.Combine(code, Value);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value
    {
        get => _Value;
        init
        {
            var parts = Identifier.GetParts(Engine, value);
            switch (parts.Count)
            {
                case 0:
                    _Value = null;
                    _UnwrappedValue = null;
                    break;

                case 1:
                    _UnwrappedValue = value = parts[0];
                    _Value = value is null
                        ? null
                        : Engine.UseTerminators
                            ? $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}"
                            : value;
                    break;

                default:
                    throw new ArgumentException(
                        "More than one identifier part detected")
                        .WithData(value);
            }
        }
    }
    string? _Value = null;

    /// <inheritdoc/>
    public string? UnwrappedValue
    {
        get => _UnwrappedValue;
        init => _Value = value; // 'Value' centralizes the logic...
    }
    string? _UnwrappedValue = null;
}