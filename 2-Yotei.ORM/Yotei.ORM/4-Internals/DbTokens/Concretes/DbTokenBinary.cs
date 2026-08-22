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
        Operation = Validate(operation);
        Right = right.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Left} {Operation} {Right})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Right.GetArgument();

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

    // ----------------------------------------------------

    /// <summary>
    /// The collection of supported binary operations.
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

    /// <summary>
    /// Validates and return the given operation, or throws an appropriate exception.
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static ExpressionType Validate(ExpressionType operation)
    {
        if (!Supported.Contains(operation)) throw new ArgumentException(
            "Unsupported binary operation.")
            .WithData(operation);

        return operation;
    }

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
        if (other is not DbTokenBinary valid) return false;

        if (!Left.Equals(valid.Left)) return false;
        if (Operation != valid.Operation) return false;
        if (!Right.Equals(valid.Right)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenBinary);

    public static bool operator ==(DbTokenBinary? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenBinary? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Left);
        code = HashCode.Combine(code, Operation);
        code = HashCode.Combine(code, Right);
        return code;
    }
}