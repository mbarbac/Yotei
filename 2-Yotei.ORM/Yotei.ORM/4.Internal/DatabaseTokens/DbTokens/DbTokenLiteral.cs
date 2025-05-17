namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary literal in a database expression that, by convention, will never be
/// captured as an argument. Literal values can be empty ones, but not null ones.
/// </summary>
public class DbTokenLiteral : DbToken
{
    /// <summary>
    /// Represents an empty literal.
    /// </summary>
    public static DbTokenLiteral Empty { get; } = new(string.Empty);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenLiteral(string value) => Value = value.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => null;

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public string Value { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenLiteral xother)
        {
            if (Value == xother.Value) return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();
}