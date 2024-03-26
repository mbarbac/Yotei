namespace Yotei.ORM.Records.Internal;

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
    public TokenConvert(Token target) => Target = target.ThrowWhenNull();

    /// <summary>
    /// The target of the conversion operation.
    /// </summary>
    public Token Target { get; }

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() => Target.GetArgument();

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation to a given type.
    /// </summary>
    public sealed class ToType : TokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToType(Type type, Token target) : base(target) => Type = type.ThrowWhenNull();

        /// <inheritdoc/>
        public override string ToString() => $"(({Type.EasyName()}) {Target})";

        /// <summary>
        /// The type to convert the target to.
        /// </summary>
        public Type Type { get; }
    }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation to a given type specification.
    /// </summary>
    public sealed class ToSpec : TokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToSpec(string type, Token target)
            : base(target)
            => Type = type.NotNullNotEmpty();

        /// <inheritdoc/>
        public override string ToString() => $"(({Type}) {Target})";

        /// <summary>
        /// The specification to convert the target to.
        /// </summary>
        public string Type { get; }
    }
}