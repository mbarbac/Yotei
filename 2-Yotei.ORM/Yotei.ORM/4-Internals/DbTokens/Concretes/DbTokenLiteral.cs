namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary literal in a database expression that, by convention, will never be
/// captured as an argument.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenLiteral : IDbToken
{
    /// <summary>
    /// A static instance representing an empty literal.
    /// </summary>
    public static DbTokenLiteral Empty { get; } = new(string.Empty);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenLiteral(string value) => Value = value.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => null;

    /// <summary>
    /// The value carried by this literal.
    /// </summary>
    public string Value { get; }

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
        if (other is not DbTokenLiteral valid) return false;

        if (Value == valid.Value) return true;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenLiteral);

    public static bool operator ==(DbTokenLiteral? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenLiteral? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Value);
        return code;
    }
}