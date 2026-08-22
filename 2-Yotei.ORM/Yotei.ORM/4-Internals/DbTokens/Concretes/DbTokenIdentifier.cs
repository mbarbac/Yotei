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
    public DbTokenIdentifier(
        IDbToken host, IIdentifier identifier)
        : base(host)
        => Identifier = identifier.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var reduce = true;
        var useTerminators = Identifier.Engine.UseTerminators;
        var str = Identifier.ToStringEx(reduce, useTerminators);
        
        return $"{Host}.{str}";
    }

    /// <summary>
    /// The identifier carried by this token.
    /// </summary>
    public IIdentifier Identifier { get; }

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