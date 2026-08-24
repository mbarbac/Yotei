namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a convert or cast operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract class DbTokenConvert : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public DbTokenConvert(IDbToken target) => Target = target.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => Target.GetArgument();

    /// <summary>
    /// The target of the convert or cast operation.
    /// </summary>
    public IDbToken Target { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public abstract bool Equals(IDbToken? other);

    // ====================================================
    /// <summary>
    /// Represents a convert or cast operation to a given type.
    /// <br/> Instances of this type are intended to be immutable ones.
    /// </summary>
    public class ToType : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        [SuppressMessage("", "IDE0290")]
        public ToType(Type type, IDbToken target) : base(target) => Type = type.ThrowWhenNull();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"(({Type}) {Target})";

        /// <summary>
        /// The type to convert the given target to.
        /// </summary>
        public Type Type { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IDbToken? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (other is not ToType valid) return false;

            if (!Target.Equals(valid.Target)) return false;
            if (Type != valid.Type) return false;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as ToType);

        public static bool operator ==(ToType? host, IDbToken? item)
        {
            if (host is null && item is null) return true;
            if (host is null || item is null) return false;
            return host.Equals(item);
        }

        public static bool operator !=(ToType? host, IDbToken? item) => !(host == item);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Type);
            code = HashCode.Combine(code, Target);
            return code;
        }
    }



    // ====================================================
    /// <summary>
    /// Represents a convert or cast operation to a given type's spec.
    /// <br/> Instances of this type are intended to be immutable ones.
    /// </summary>
    public class ToSpec : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        [SuppressMessage("", "IDE0290")]
        public ToSpec(string type, IDbToken target)
            : base(target) => Type = type.NotNullNotEmpty(trim: true);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"(({Type}) {Target})";

        /// <summary>
        /// The type to convert the given target to.
        /// </summary>
        public string Type { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IDbToken? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (other is not ToSpec valid) return false;

            if (!Target.Equals(valid.Target)) return false;
            if (Type != valid.Type) return false;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as ToSpec);

        public static bool operator ==(ToSpec? host, IDbToken? item)
        {
            if (host is null && item is null) return true;
            if (host is null || item is null) return false;
            return host.Equals(item);
        }

        public static bool operator !=(ToSpec? host, IDbToken? item) => !(host == item);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Type);
            code = HashCode.Combine(code, Target);
            return code;
        }
    }
}