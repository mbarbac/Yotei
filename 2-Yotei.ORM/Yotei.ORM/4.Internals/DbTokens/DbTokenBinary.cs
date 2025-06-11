namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a binary operation between two given tokens.
/// </summary>
[Cloneable]
public partial class DbTokenBinary : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public DbTokenBinary(DbToken left, ExpressionType operation, DbToken right)
    {
        Left = left.ThrowWhenNull();
        Operation = ValidatedOperation(operation);
        Right = right.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenBinary(DbTokenBinary source) : this(
        source.Left.Clone(),
        source.Operation,
        source.Right.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} {Operation} {Right})";

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
        if (other is not DbTokenBinary valid) return false;

        if (!Left.Equals(valid.Left)) return false;
        if (Operation != valid.Operation) return false;
        if (!Right.Equals(valid.Right)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenBinary? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenBinary? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Operation);
        code = HashCode.Combine(code, Right);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the binary operation.
    /// </summary>
    public DbToken Left { get; }

    /// <summary>
    /// The binary operation represented by this instance.
    /// </summary>
    public ExpressionType Operation { get; }

    /// <summary>
    /// The right operand of the binary operation.
    /// </summary>
    public DbToken Right { get; }

    /// <summary>
    /// Returns a validated operation.
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static ExpressionType ValidatedOperation(ExpressionType operation)
    {
        if (!Supported.Contains(operation)) throw new ArgumentException(
            "Unsupported binary operation.")
            .WithData(operation);

        return operation;
    }

    /// <summary>
    /// The collection of supported operations by instances of this class.
    /// </summary>
    public static ImmutableArray<ExpressionType> Supported { get; } = [
        ExpressionType.Equal,
        ExpressionType.NotEqual,

        ExpressionType.Add,
        ExpressionType.Subtract,
        ExpressionType.Multiply,
        ExpressionType.Divide,
        ExpressionType.Modulo,
        ExpressionType.Power,

        ExpressionType.And,
        ExpressionType.Or,

        ExpressionType.GreaterThan,
        ExpressionType.GreaterThanOrEqual,
        ExpressionType.LessThan,
        ExpressionType.LessThanOrEqual,
    ];
}