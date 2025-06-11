namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a ternary '(left ? middle : right)' expression  in a database expression.
/// </summary>
[Cloneable]
public partial class DbTokenTernary : DbToken
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

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenTernary(DbTokenTernary source) : this(
        source.Left.Clone(),
        source.Middle.Clone(),
        source.Right.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} ? {Middle} : {Right})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Middle.GetArgument() ??
        Right.GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenTernary valid) return false;

        if (!Left.Equals(valid.Left)) return false;
        if (!Middle.Equals(valid.Middle)) return false;
        if (!Right.Equals(valid.Right)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenTernary? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenTernary? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Middle);
        code = HashCode.Combine(code, Right);
        return code;
    }

    // ----------------------------------------------------

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
}