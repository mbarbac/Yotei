namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the assignation of an arbitrary token to a target one.
/// </summary>
[Cloneable]
public partial class DbTokenSetter : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public DbTokenSetter(DbToken target, DbToken value)
    {
        Target = target.ThrowWhenNull();
        Value = value.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public DbTokenSetter(DbTokenSetter source) : this(source.Target, source.Value) { }

    /// <inheritdoc/>
    public override string ToString() => $"({Target} = {Value})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() =>
        Target.GetArgument() ??
        Value.GetArgument();

    /// <summary>
    /// The target operand of this assignation operation.
    /// </summary>
    public DbToken Target { get; }

    /// <summary>
    /// The value operand of this assignation operation.
    /// </summary>
    public DbToken Value { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenSetter xother)
        {
            if (Target.Equals(xother.Target) &&
                Value.Equals(xother.Value))
                return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Target);
        code = HashCode.Combine(code, Value);
        return code;
    }
}