namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a conversion or cast operation of a given token.
/// </summary>
public abstract class DbTokenConvert : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenConvert(DbToken target) => Target = target.ThrowWhenNull();

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => Target.GetArgument();

    /// <summary>
    /// The target of the conversion operation.
    /// </summary>
    public DbToken Target { get; }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a given type.
    /// </summary>
    public class ToType : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToType(Type type, DbToken target) : base(target) => Type = type.ThrowWhenNull();

        /// <inheritdoc/>
        public override string ToString() => $"(({Type.EasyName()}) {Target})";

        /// <summary>
        /// The type to convert the target to.
        /// </summary>
        public Type Type { get; }
    }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a given type specification.
    /// </summary>
    public class ToSpec : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToSpec(string type, DbToken target) : base(target) => Type = type.NotNullNotEmpty();

        /// <inheritdoc/>
        public override string ToString() => $"(({Type}) {Target})";

        /// <summary>
        /// The specification of the type to convert the target to.
        /// </summary>
        public string Type { get; }
    }
}