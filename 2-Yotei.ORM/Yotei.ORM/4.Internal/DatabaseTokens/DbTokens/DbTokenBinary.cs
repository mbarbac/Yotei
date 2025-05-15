namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a binary operation between two given tokens.
/// </summary>
public class DbTokenBinary : DbToken
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

    /// <inheritdoc/>
    public override string ToString() => $"({Left} {Operation} {Right})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => 
        Left.GetArgument() ??
        Right.GetArgument();

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

    // ----------------------------------------------------

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