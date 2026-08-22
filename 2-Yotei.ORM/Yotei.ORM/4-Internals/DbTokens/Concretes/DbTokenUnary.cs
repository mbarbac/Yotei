namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a UNARU operation against a given target.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenUnary : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="target"></param>
    [SuppressMessage("", "IDE0290")]
    public DbTokenUnary(ExpressionType operation, IDbToken target)
    {
        Operation = Validate(operation);
        Target = target.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({Operation} {Target})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => Target.GetArgument();

    /// <summary>
    /// The binary operation represented by this instance.
    /// </summary>
    public ExpressionType Operation { get; }

    /// <summary>
    /// The right operand of the binary operation.
    /// </summary>
    public IDbToken Target { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of supported binary operations.
    /// </summary>
    public static ImmutableArray<ExpressionType> Supported { get; } = [
        ExpressionType.Not,
        ExpressionType.Negate,
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
        if (other is not DbTokenUnary valid) return false;

        if (Operation != valid.Operation) return false;
        if (!Target.Equals(valid.Target)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenUnary);

    public static bool operator ==(DbTokenUnary? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenUnary? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Operation);
        code = HashCode.Combine(code, Target);
        return code;
    }
}