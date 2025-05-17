namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a ternary expression (left ? middle : right) in a database expression.
/// </summary>
public class DbTokenTernary : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public DbTokenTernary(DbToken left, DbToken middle, DbToken right) : base()
    {
        Left = left.ThrowWhenNull();
        Middle = middle.ThrowWhenNull();
        Right = right.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} ? {Middle} : {Right})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Middle.GetArgument() ??
        Right.GetArgument();
    
    /// <summary>
    /// The left operand of the ternary operation.
    /// </summary>
    public DbToken Left { get; }

    /// <summary>
    /// The middle operand of the ternary operation.
    /// </summary>
    public DbToken Middle { get; }

    /// <summary>
    /// The right operand of the ternary operation.
    /// </summary>
    public DbToken Right { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenTernary xother)
        {
            if (Left.Equals(xother.Left) &&
                Middle.Equals(xother.Middle) &&
                Right.Equals(xother.Right)) return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Middle);
        code = HashCode.Combine(code, Right);
        return code;
    }
}