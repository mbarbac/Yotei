namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a '(left ?? right)' coalesce expression.
/// </summary>
[Cloneable]
public partial class DbTokenCoalesce : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public DbTokenCoalesce(DbToken left, DbToken right)
    {
        Left = left.ThrowWhenNull();
        Right = right.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCoalesce(DbTokenCoalesce source) : this(
        source.Left.Clone(),
        source.Right.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} ?? {Right})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Right.GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCoalesce valid) return false;

        if (!Left.Equals(valid.Left)) return false;
        if (!Right.Equals(valid.Right)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenCoalesce? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCoalesce? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Right);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The left part of the coalesce operation.
    /// </summary>
    public DbToken Left { get; }

    /// <summary>
    /// The right part of the coalesce operation.
    /// </summary>
    public DbToken Right { get; }
}