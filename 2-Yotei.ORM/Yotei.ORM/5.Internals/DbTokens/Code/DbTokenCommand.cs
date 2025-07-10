namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an embedded command in a database expression.
/// </summary>
[Cloneable]
public partial class DbTokenCommand : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="info"></param>
    public DbTokenCommand(ICommandInfo info) => CommandInfo = info.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCommand(DbTokenCommand source) : this(source.CommandInfo) { }

    /// <inheritdoc/>
    public override string ToString() => CommandInfo.IsEmpty
        ? string.Empty
        : $"({CommandInfo.Text})";

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;
    
    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommand valid) return false;

        var sensitive = CommandInfo.Engine.CaseSensitiveNames;
        if (sensitive != valid.CommandInfo.Engine.CaseSensitiveNames) return false;
        if (string.Compare(CommandInfo.Text, valid.CommandInfo.Text, !sensitive) != 0) return false;
        if (!CommandInfo.Parameters.Equals(valid.CommandInfo.Parameters)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenCommand? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCommand? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => CommandInfo.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The info carried by this instance about the embedded command  it represents.
    /// </summary>
    public ICommandInfo CommandInfo { get; }
}