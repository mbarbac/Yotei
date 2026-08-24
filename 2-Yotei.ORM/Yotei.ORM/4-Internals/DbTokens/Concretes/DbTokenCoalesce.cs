namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a '(left ?? right)' operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenCoalesce : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public DbTokenCoalesce(IDbToken left, IDbToken right)
    {
        Left = left.ThrowWhenNull();
        Right = right.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Left} ?? {Right})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Right.GetArgument();

    /// <summary>
    /// The left part of the coalesce operation.
    /// </summary>
    public IDbToken Left { get; }

    /// <summary>
    /// The right part of the coalesce operation.
    /// </summary>
    public IDbToken Right { get; }

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
        if (other is not DbTokenCoalesce valid) return false;

        if (!Left.Equals(valid.Left)) return false;
        if (!Right.Equals(valid.Right)) return false;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenCoalesce);

    public static bool operator ==(DbTokenCoalesce? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenCoalesce? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Right);
        return code;
    }
}