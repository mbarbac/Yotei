namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierPart"/>
public class IdentifierPart : IIdentifierPart
{
    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public string? Value { get; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; }
}