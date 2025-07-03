namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary value in a database expression. Values carried by instances of this
/// type are typically intended to be captured as command arguments.
/// </summary>
[Cloneable]
public partial class DbTokenValue : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenValue(object? value) => Value = Value = ValidateValue(value);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenValue(DbTokenValue source) : this(
        source.Value.TryClone())
    { }

    /// <inheritdoc/>
    public override string ToString() => Value switch
    {
        bool item => item.ToString().ToUpper(),
        string item => $"'{item}'",
        null => "NULL",

        _ => $"'{Value.Sketch()}'"
    };

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenValue valid) return false;

        if (!Value.EqualsEx(valid.Value)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenValue? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenValue? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Invoked to return a validated value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object? ValidateValue(object? value) => value switch
    {
        LambdaNode => throw new ArgumentException("Not supported token value").WithData(value),
        Delegate => throw new ArgumentException("Not supported token value").WithData(value),
        IDbToken => throw new ArgumentException("Not supported token value").WithData(value),

        _ => value
    };
}