namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an assignation operation on a target of a given value.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenSetter : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenSetter(IDbToken target, IDbToken value)
    {
        Target = target.ThrowWhenNull();
        Value = value.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Target} = {Value})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() =>
        Target.GetArgument() ??
        Value.GetArgument();

    /// <summary>
    /// The target of the assignation.
    /// </summary>
    public IDbToken Target { get; }

    /// <summary>
    /// The value to assign to the target.
    /// </summary>
    public IDbToken Value { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenSetter valid) return false;

        if (!Target.Equals(valid.Target)) return false;
        if (!Value.Equals(valid.Value)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenSetter);

    public static bool operator ==(DbTokenSetter? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenSetter? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Target);
        code = HashCode.Combine(code, Value);
        return code;
    }
}