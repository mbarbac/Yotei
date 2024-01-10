namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an unary operation against a given token.
/// </summary>
public sealed class TokenUnary : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenUnary(ExpressionType operation, Token target)
    {
        Operation = ValidatedOperation(operation);
        Target = target.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({Operation} {Target})";

    /// <summary>
    /// The unary operation represented by this instance.
    /// </summary>
    public ExpressionType Operation { get; }

    /// <summary>
    /// The operand of the unary operation.
    /// </summary>
    public Token Target { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => Target.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated operation.
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static ExpressionType ValidatedOperation(ExpressionType operation)
    {
        if (!Supported.Contains(operation)) throw new ArgumentException(
            "Unsupported unary operation.")
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