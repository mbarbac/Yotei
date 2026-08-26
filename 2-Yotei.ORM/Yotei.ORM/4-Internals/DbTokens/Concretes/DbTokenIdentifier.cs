namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries an identifier.
/// </summary>
public class DbTokenIdentifier : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    [SuppressMessage("", "IDE0290")]
    public DbTokenIdentifier(
        IDbToken host, IIdentifier identifier)
        : base(host) => Identifier = identifier.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToStringEx(false, Identifier.Engine.UseTerminators);

    /// <summary>
    /// Returns a string representation of this instance using or not its null head parts, and
    /// wrapping or not the remaining parts with the engine terminators, as requested.
    /// </summary>
    /// <param name="reduce"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public string ToStringEx(bool reduce = true, bool useTerminators = true)
    {
        var str = Identifier.ToStringEx(reduce, useTerminators);
        return $"{Host}.{str}";
    }

    /// <summary>
    /// The identifier carried by this token.
    /// </summary>
    public IIdentifier Identifier { get; }

    /// <summary>
    /// The actual value carried by the identifier of this instance, or <see langword="null"/> if
    /// it represents an empty or missed one. The empty or null head parts are removed, and then
    /// each part is wrapped with the engine's terminators, if any.
    /// <br/> The <see cref="Identifier.ToStringEx(bool, bool)"/> method can be used to customize
    /// the string representation of the value.
    /// </summary>
    public string? Value => Identifier.Value;

    /// <summary>
    /// Determines if this instance, along with its chain of hosts, represents a pure identifier
    /// or not.
    /// </summary>
    public bool IsPureIdentifier => Host switch
    {
        DbTokenArgument => true,
        DbTokenIdentifier item => item.IsPureIdentifier,
        _ => false
    };

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenIdentifier valid) return false;

        if (!Identifier.Equals(valid.Identifier)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenMethod);

    public static bool operator ==(DbTokenIdentifier? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenIdentifier? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Identifier);
        return code;
    }
}