namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a conversion or cast operation of a given token.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract partial class DbTokenConvert : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenConvert(IDbToken target) => Target = target.ThrowWhenNull();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => Target.GetArgument();

    /// <inheritdoc/>
    public abstract bool Equals(IDbToken? other);

    /// <summary>
    /// The target of the conversion or cast operation.
    /// </summary>
    public IDbToken Target { get; }

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract DbTokenConvert Clone();
    IDbToken IDbToken.Clone() => Clone();

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a given actual type.
    /// </summary>
    public partial class ToType : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToType(Type type, IDbToken target) : base(target) => Type = type.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected ToType(ToType source) : this(
            source.Type,
            source.Target.Clone())
        { }

        /// <inheritdoc/>
        public override string ToString() => $"(({Type.EasyName()}) {Target})";

        /// <inheritdoc/>
        public override ToType Clone() => new(this);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(IDbToken? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (other is not ToType valid) return false;

            if (!Target.Equals(valid.Target)) return false;
            if (Type != valid.Type) return false;
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as IDbToken);

        public static bool operator ==(ToType? host, IDbToken? other) => host?.Equals(other) ?? false;

        public static bool operator !=(ToType? host, IDbToken? other) => !(host == other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Type);
            code = HashCode.Combine(code, Target);
            return code;
        }

        // ------------------------------------------------

        /// <summary>
        /// The type to convert the target to.
        /// </summary>
        public Type Type { get; }
    }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a type specification
    /// specified by an arbitrary not-null and not-empty string.
    /// </summary>
    public partial class ToSpec : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToSpec(string type, IDbToken target) : base(target) => Type = type.NotNullNotEmpty();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected ToSpec(ToSpec source) : this(
            source.Type,
            source.Target.Clone())
        { }

        /// <inheritdoc/>
        public override string ToString() => $"(({Type}) {Target})";

        /// <inheritdoc/>
        public override ToSpec Clone() => new(this);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(IDbToken? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (other is not ToSpec valid) return false;

            if (!Target.Equals(valid.Target)) return false;
            if (Type != valid.Type) return false;
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as IDbToken);

        public static bool operator ==(ToSpec? host, IDbToken? other) => host?.Equals(other) ?? false;

        public static bool operator !=(ToSpec? host, IDbToken? other) => !(host == other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Type);
            code = HashCode.Combine(code, Target);
            return code;
        }

        // ------------------------------------------------

        /// <summary>
        /// The not-null and not-empty type specification to convert the target to.
        /// </summary>
        public string Type { get; }
    }
}