namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a direct invocation of a given host.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenInvoke : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance with empty arguments.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    public DbTokenInvoke(IDbToken host) : base(host) => Arguments = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    public DbTokenInvoke(IDbToken host, IEnumerable<IDbToken> args) : base(host)
    {
        Arguments = DbToken.ToArguments(args, allowEmpty: false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = Arguments.ToString("(", ")", ", ");
        return $"{Host}{str}";
    }

    /// <summary>
    /// The arguments of this instance, if any.
    /// </summary>
    public DbTokenChain Arguments { get; }

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
        if (other is not DbTokenInvoke valid) return false;

        if (Arguments.Count != valid.Arguments.Count) return false;
        for (int i = 0; i < Arguments.Count; i++)
        {
            var item = Arguments[i];
            var temp = valid.Arguments[i];
            var same = item.Equals(temp);
            if (!same) return false;
        }
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenInvoke);

    public static bool operator ==(DbTokenInvoke? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenInvoke? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Arguments);
        for (int i = 0; i < Arguments.Count; i++) code = HashCode.Combine(code, Arguments[i]);
        return code;
    }
}