namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a unary operation against a given token.
/// </summary>
public sealed class DbTokenUnary : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="target"></param>
    public DbTokenUnary(ExpressionType operation, DbToken target)
    {
        Operation = ValidatedOperation(operation);
        Target = target.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"({Operation} {Target})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => Target.GetArgument();

    /// <summary>
    /// The unary operation represented by this instance.
    /// </summary>
    public ExpressionType Operation { get; }

    /// <summary>
    /// The target of the unary operation.
    /// </summary>
    public DbToken Target { get; }

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
        ExpressionType.Not,
        ExpressionType.Negate,
    ];
}