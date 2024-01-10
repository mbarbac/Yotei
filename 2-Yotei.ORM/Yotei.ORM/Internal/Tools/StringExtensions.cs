namespace Yotei.ORM.Internal;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Validates that the given token name is a valid one for identifiers, method names, and
    /// similar elements.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateTokenName(this string name)
    {
        name = name.NotNullNotEmpty();

        if (name.ContainsAny(INVALID_CHARS)) throw new ArgumentException(
            "Name contains invalid chararcters.")
            .WithData(name);

        return name;
    }
    static readonly string INVALID_CHARS = @" .+-/*[]{}()^=?%&!\";

    /// <summary>
    /// Returns null if the given value matches the name of the given dynamic argument, if any.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(
        this string? value,
        LambdaNodeArgument? darg,
        bool caseSensitive)
    {
        if (value is not null &&
            darg is not null &&
            string.Compare(value, darg.LambdaName, !caseSensitive) == 0) return null;

        return value;
    }

    /// <summary>
    /// Returns null if the given value matches the name of the given dynamic argument, if any.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(
        this string? value,
        TokenArgument? darg,
        bool caseSensitive)
    {
        if (value is not null &&
            darg is not null &&
            string.Compare(value, darg.Name, !caseSensitive) == 0) return null;

        return value;
    }
}