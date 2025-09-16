namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a binary operation between two given tokens.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial class DbTokenBinary : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public DbTokenBinary(IDbToken left, ExpressionType operation, IDbToken right)
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

    /// <inheritdoc cref="ICloneable.Clone"/>
    public virtual DbTokenBinary Clone() => new(Left, Operation, Right);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Right.GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
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
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenBinary? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenBinary? host, IDbToken? other) => !(host == other);

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
    public IDbToken Left { get; }

    /// <summary>
    /// The binary operation represented by this instance.
    /// </summary>
    public ExpressionType Operation { get; }

    /// <summary>
    /// The right operand of the binary operation.
    /// </summary>
    public IDbToken Right { get; }

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