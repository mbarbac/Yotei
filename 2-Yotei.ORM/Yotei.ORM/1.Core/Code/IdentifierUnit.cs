namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifierUnit"/>
/// </summary>
public class IdentifierUnit : IIdentifierUnit
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierUnit(IEngine engine, string? value = null)
    {
        Engine = engine.ThrowWhenNull();
        Value = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IIdentifier? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return string.Compare(Value, other.Value, !Engine.CaseSensitive) == 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);

    public static bool operator ==(IdentifierUnit? host, IIdentifier? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(IdentifierUnit? host, IIdentifier? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value
    {
        get => _Value;
        init
        {
        }
    }
    string? _Value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? RawValue
    {
        get => _RawValue;
        init => Value = value;
    }
    string? _RawValue;
}