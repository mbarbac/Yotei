namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a unary operation against a given token.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenUnary : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="target"></param>
    public DbTokenUnary(ExpressionType operation, IDbToken target)
    {
        Operation = ValidatedOperation(operation);
        Target = target.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenUnary(DbTokenUnary source) : this(
        source.Operation,
        source.Target.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString() => $"({Operation} {Target})";

    /// <inheritdoc cref="ICloneable.Clone"/>
    public virtual DbTokenUnary Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => Target.GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenUnary valid) return false;

        if (Operation != valid.Operation) return false;
        if (!Target.Equals(valid.Target)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenUnary? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenUnary? host, IDbToken? other) => !(host == other);

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
    /// The unary operation represented by this instance.
    /// </summary>
    public ExpressionType Operation { get; }

    /// <summary>
    /// The target operand of the unary operation.
    /// </summary>
    public IDbToken Target { get; }

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