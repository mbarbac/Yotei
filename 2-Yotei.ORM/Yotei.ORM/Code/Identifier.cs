namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifier"/>
public abstract class Identifier : IIdentifier
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifier? other)
    {
        if (other is null) return false;

        if (Engine.CaseSensitiveNames != other.Engine.CaseSensitiveNames) return false;
        if (Engine.UseTerminators != other.Engine.UseTerminators) return false;
        if (Engine.LeftTerminator != other.Engine.LeftTerminator) return false;
        if (Engine.RightTerminator != other.Engine.RightTerminator) return false;

        return string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) == 0;
    }
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);
    public static bool operator ==(Identifier x, IIdentifier y) => x is not null && x.Equals(y);
    public static bool operator !=(Identifier x, IIdentifier y) => !(x == y);
    public override int GetHashCode() => HashCode.Combine(Value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public abstract string? Value { get; }
}