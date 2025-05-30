﻿namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a dynamic argument in a dynamic lambda expression.
/// <br/> Instances of this type are considered translation artifacts, with no representation
/// in a database command.
/// </summary>
[Cloneable]
public partial class DbTokenArgument : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenArgument(string name) => Name = ValidateTokenName(name);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public DbTokenArgument(DbTokenArgument source) : this(source.Name) { }

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => this;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenArgument xother)
        {
            if (Name == xother.Name) return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode();
}