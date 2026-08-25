namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries a command info instance.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenCommandInfo : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="info"></param>
    public DbTokenCommandInfo(ICommandInfo info) => CommandInfo = info.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => CommandInfo.IsEmpty
        ? string.Empty
        : $"({CommandInfo.Text})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => null;

    /// <summary>
    /// The command info carried by this instance.
    /// </summary>
    public ICommandInfo CommandInfo { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommandInfo valid) return false;

        if (CommandInfo.Text != valid.CommandInfo.Text) return false;
        if (!CommandInfo.Parameters.Equals(valid.CommandInfo.Parameters)) return false;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenCommandInfo);

    public static bool operator ==(DbTokenCommandInfo? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenCommandInfo? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CommandInfo);
        return code;
    }
}