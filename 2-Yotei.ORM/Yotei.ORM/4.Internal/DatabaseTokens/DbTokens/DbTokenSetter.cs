namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the assignation of an arbitrary token to a target one.
/// </summary>
public class DbTokenSetter : DbToken
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
}