namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary indexed getter on a given host.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenIndexed : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    [SuppressMessage("", "IDE0290")]
    public DbTokenIndexed(IDbToken host, IEnumerable<IDbToken> indexes) : base(host)
    {
        Indexes = DbToken.ToArguments(indexes, allowEmpty: false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = $"[{string.Join(", ", Indexes)}]";
        return $"{Host}{str}";
    }

    /// <summary>
    /// The indexes of this instance.
    /// </summary>
    public ImmutableArray<IDbToken> Indexes { get; }

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
        if (other is not DbTokenIndexed valid) return false;

        if (Indexes.Length != valid.Indexes.Length) return false;
        for (int i = 0; i < Indexes.Length; i++)
        {
            var item = Indexes[i];
            var temp = valid.Indexes[i];
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
    public override bool Equals(object? obj) => Equals(obj as DbTokenIndexed);

    public static bool operator ==(DbTokenIndexed? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenIndexed? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Indexes);
        for (int i = 0; i < Indexes.Length; i++) code = HashCode.Combine(code, Indexes[i]);
        return code;
    }
}