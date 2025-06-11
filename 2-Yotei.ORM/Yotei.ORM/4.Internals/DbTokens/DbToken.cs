namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a database expression.
/// <br/> Instances of this type might be mutable ones, caution is needed.
/// </summary>
[Cloneable]
public abstract partial class DbToken : IEquatable<DbToken>
{
    /// <summary>
    /// Returns the dynamic argument associated with this instance, or <c>null</c> if it cannot
    /// be determined.
    /// </summary>
    /// <returns></returns>
    public abstract DbTokenArgument? GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract bool Equals(DbToken? other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(DbToken? host, DbToken? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(DbToken? host, DbToken? item) => !(host == item);

    /// <inheritdoc/>
    public override abstract int GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// Returns the validated token name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateTokenName(string name)
    {
        name = name.NotNullNotEmpty();

        if (!ValidFirstChar(name[0])) throw new ArgumentException(
            "Name contains invalid first character.")
            .WithData(name);

        if (name.Any(x => !ValidOtherChar(x))) throw new ArgumentException(
            "Name contains invalid character(s).")
            .WithData(name);

        return name;

        static bool ValidFirstChar(char c) =>
            VALID_FIRST.Contains(c) ||
            (c >= '0' && c <= '9') ||
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z');

        static bool ValidOtherChar(char c) => ValidFirstChar(c);
    }

    static readonly string VALID_FIRST = "_$@#";
}