namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a '(left ? middle : right)' operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenTernary : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public DbTokenTernary(IDbToken left, IDbToken middle, IDbToken right)
    {
        Left = left.ThrowWhenNull();
        Middle = middle.ThrowWhenNull();
        Right = right.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Left} ? {Middle} : {Right})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Middle.GetArgument() ??
        Right.GetArgument();

    /// <summary>
    /// The left part of the ternary operation.
    /// </summary>
    public IDbToken Left { get; }

    /// <summary>
    /// The middle part of the ternary operation.
    /// </summary>
    public IDbToken Middle { get; }

    /// <summary>
    /// The right part of the ternary operation.
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
        if (other is not DbTokenTernary valid) return false;

        if (!Left.Equals(valid.Left)) return false;
        if (!Middle.Equals(valid.Middle)) return false;
        if (!Right.Equals(valid.Right)) return false;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenTernary);

    public static bool operator ==(DbTokenTernary? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenTernary? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Middle);
        code = HashCode.Combine(code, Right);
        return code;
    }
}