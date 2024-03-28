namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary token in a database expression.
/// </summary>
public abstract class Token
{
    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if it
    /// cannot be determined.
    /// </summary>
    /// <returns></returns>
    public abstract TokenArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given token name is a valid one for identifiers, method names, and
    /// similar elements.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateTokenName(string name)
    {
        name = name.NotNullNotEmpty();

        if (name.ContainsAny(INVALID_CHARS)) throw new ArgumentException(
            "Name contains invalid chararcters.")
            .WithData(name);

        return name;
    }
    static readonly string INVALID_CHARS = @" .+-/*[]{}()^=?%&!\";
}