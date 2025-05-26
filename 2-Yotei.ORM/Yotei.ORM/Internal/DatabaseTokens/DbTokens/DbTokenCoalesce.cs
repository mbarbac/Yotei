namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a coalesce expression (left ?? right) in a database expression.
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
    public DbTokenCoalesce(DbTokenCoalesce source) : this(source.Left, source.Right) { }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} ?? {Right})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Right.GetArgument();

    /// <summary>
    /// The left part of the coalesce operation.
    /// </summary>
    public DbToken Left { get; }

    /// <summary>
    /// The right part of the coalesce operation.
    /// </summary>
    public DbToken Right { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenCoalesce xother)
        {
            if (Left.EqualsEx(xother.Left) &&
                Right.Equals(xother.Right)) return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Right);
        return code;
    }
}