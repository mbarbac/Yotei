namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ISimpleIdentifier"/>
public class SimpleIdentifier : ISimpleIdentifier
{
    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifier? other) => throw null;
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);
    public static bool operator ==(SimpleIdentifier x, IIdentifier y) => x is not null && x.Equals(y);
    public static bool operator !=(SimpleIdentifier x, IIdentifier y) => !(x == y);
    public override int GetHashCode() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value { get; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; }
}