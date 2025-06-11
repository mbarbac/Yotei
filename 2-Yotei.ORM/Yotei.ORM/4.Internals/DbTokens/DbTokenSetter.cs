namespace Yotei.ORM.Internals;

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
    protected DbTokenSetter(DbTokenSetter source) : this(
        source.Target.Clone(),
        source.Value.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString() => $"({Target} = {Value})";

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() =>
        Target.GetArgument() ??
        Value.GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenSetter valid) return false;

        if (!Target.Equals(valid.Target)) return false;
        if (!Value.Equals(valid.Value)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenSetter? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenSetter? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Target);
        code = HashCode.Combine(code, Value);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The target operand of this assignation operation.
    /// </summary>
    public DbToken Target { get; }

    /// <summary>
    /// The value operand of this assignation operation.
    /// </summary>
    public DbToken Value { get; }
}