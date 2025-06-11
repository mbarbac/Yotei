namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a conversion or cast operation of a given token.
/// </summary>
[Cloneable]
public abstract partial class DbTokenConvert : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenConvert(DbToken target) => Target = target.ThrowWhenNull();

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => Target.GetArgument();

    /// <summary>
    /// The target of the conversion or cast operation.
    /// </summary>
    public DbToken Target { get; }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a given actual type.
    /// </summary>
    [Cloneable]
    public partial class ToType : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToType(Type type, DbToken target) : base(target) => Type = type.ThrowWhenNull();

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

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(DbToken? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (other is not ToType valid) return false;

            if (!Target.Equals(valid.Target)) return false;
            if (Type != valid.Type) return false;
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as DbToken);

        public static bool operator ==(ToType? host, DbToken? other) => host?.Equals(other) ?? false;

        public static bool operator !=(ToType? host, DbToken? other) => !(host == other);

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
    [Cloneable]
    public partial class ToSpec : DbTokenConvert
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public ToSpec(string type, DbToken target) : base(target) => Type = type.NotNullNotEmpty();

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

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(DbToken? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            if (other is not ToSpec valid) return false;

            if (!Target.Equals(valid.Target)) return false;
            if (Type != valid.Type) return false;
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as DbToken);

        public static bool operator ==(ToSpec? host, DbToken? other) => host?.Equals(other) ?? false;

        public static bool operator !=(ToSpec? host, DbToken? other) => !(host == other);

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