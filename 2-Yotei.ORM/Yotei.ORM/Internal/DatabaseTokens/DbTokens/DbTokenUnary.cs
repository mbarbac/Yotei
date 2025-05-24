namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a unary operation against a given token.
/// </summary>
public class DbTokenUnary : DbToken
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

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenUnary xother)
        {
            if (Operation == xother.Operation &&
                Target.Equals(xother.Target))
                return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Operation);
        code = HashCode.Combine(code, Target);
        return code;
    }

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