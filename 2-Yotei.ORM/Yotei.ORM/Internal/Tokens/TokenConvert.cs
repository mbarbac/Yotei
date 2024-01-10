namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a conversion or cast operation.
/// </summary>
public abstract class TokenConvert : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenConvert(Token target) => Target = target.ThrowWhenNull();

    /// <summary>
    /// The target of the conversion operation.
    /// </summary>
    public Token Target { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => Target.GetArgument();
}

// ========================================================
/// <summary>
/// Represents a conversion or cast operation to a given type.
/// </summary>
public sealed class TokenConvertToType : TokenConvert
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenConvertToType(Type type, Token target)
        : base(target)
        => Type = type.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"(({Type.EasyName()}) {Target})";

    /// <summary>
    /// The type to convert the target to.
    /// </summary>
    public Type Type { get; }
}

// ========================================================
/// <summary>
/// Represents a conversion or cast operation to a given arbitrary specification.
/// </summary>
public sealed class TokenConvertToSpecification : TokenConvert
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenConvertToSpecification(string type, Token target)
        : base(target)
        => Type = type.NotNullNotEmpty();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"(({Type}) {Target})";

    /// <summary>
    /// The specification to convert the target to.
    /// </summary>
    public string Type { get; }
}