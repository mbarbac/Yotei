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
    public virtual bool Equals(IIdentifier? other, bool caseSensitiveNames)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IIdentifierPart valid) return false;

        if (string.Compare(Value, valid.Value, !caseSensitiveNames) != 0) return false;
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifier? other) => Equals(other, true);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IIdentifierPart);

    public static bool operator ==(IdentifierPart? host, IIdentifier? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(IdentifierPart? host, IIdentifier? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value
    {
        get => _Value;
        init
        {
            if (value is null)
            {
                _Value = null;
                _UnwrappedValue = null;
                return;
            }

            var parts = Identifier.GetParts(Engine, value, reduce: true);
            switch (parts.Count)
            {
                case 0: break;

                case 1:
                    _UnwrappedValue = value = parts[0];
                    _Value = value is null
                        ? null
                        : Engine.UseTerminators
                            ? $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}"
                            : value;
                    break;

                default:
                    throw new ArgumentException("More than one part detected.").WithData(value);
            }
        }
    }
    string? _Value = null;

    /// <inheritdoc/>
    public string? UnwrappedValue
    {
        get => _UnwrappedValue;
        init => Value = value;
    }
    string? _UnwrappedValue = null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Match(string? specs) => Identifier.Match(this, specs);
}