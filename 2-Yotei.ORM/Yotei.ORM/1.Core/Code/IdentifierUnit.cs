namespace Yotei.ORM.Code;
/*
// ========================================================
/// <inheritdoc cref="IIdentifierUnit"/>
public class IdentifierUnit : IIdentifierUnit
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierUnit(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierUnit(IEngine engine, string? value) => throw null;

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifier? other, bool caseSensitive) => throw null;

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifier? other) => Equals(other, true);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);

    public static bool operator ==(IdentifierUnit? host, IIdentifier? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(IdentifierUnit? host, IIdentifier? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value
    {
        get => throw null;
        init => throw null;
    }

    /// <inheritdoc/>
    public string? RawValue
    {
        get => throw null;
        init => throw null;
    }
}*/