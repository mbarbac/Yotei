﻿namespace Yotei.ORM.Internal;

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
    /// The target of the conversion operation.
    /// </summary>
    public DbToken Target { get; }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a given type.
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
        public ToType(ToType source) : this(source.Type, source.Target) { }

        /// <inheritdoc/>
        public override string ToString() => $"(({Type.EasyName()}) {Target})";

        /// <summary>
        /// The type to convert the target to.
        /// </summary>
        public Type Type { get; }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(DbToken? other)
        {
            if (other is ToType xother)
            {
                if (Target.Equals(xother.Target) &&
                    Type == xother.Type)
                    return true;
            }
            return ReferenceEquals(this, other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Target);
            code = HashCode.Combine(code, Type);
            return code;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a conversion or cast operation of a given token to a given type specification.
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
        public ToSpec(ToSpec source) : this(source.Type, source.Target) { }

        /// <inheritdoc/>
        public override string ToString() => $"(({Type}) {Target})";

        /// <summary>
        /// The specification of the type to convert the target to.
        /// </summary>
        public string Type { get; }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(DbToken? other)
        {
            if (other is ToSpec xother)
            {
                if (Target.Equals(xother.Target) &&
                    Type == xother.Type)
                    return true;
            }
            return ReferenceEquals(this, other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Target);
            code = HashCode.Combine(code, Type);
            return code;
        }
    }
}