namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierPart"/>
public class IdentifierPart : IIdentifierPart
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierPart(IEngine engine) : this(engine, null) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierPart(IEngine engine, string? value)
    {
        Engine = engine.ThrowWhenNull();

        var parts = Identifier.GetParts(Engine, value);
        switch (parts.Count)
        {
            case 0: return;

            case 1:
                value = parts[0];
                UnwrappedValue = value;
                Value = Engine.UseTerminators && value is not null
                    ? $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}"
                    : null;
                return;

            default:
                throw new ArgumentException(
                "More than one part detected in the given single-part value.")
                .WithData(value);
        };
    }

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifier? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (!Engine.Equals(other.Engine)) return false;
        return string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) == 0;
    }
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);
    public static bool operator ==(IdentifierPart x, IIdentifier y) => x is not null && x.Equals(y);
    public static bool operator !=(IdentifierPart x, IIdentifier y) => !(x == y);
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        code = HashCode.Combine(code, Value);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value { get; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; }
}