namespace Yotei.ORM.Records.Internal;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given value matches the name of the given dynamic argument, if any.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(this string? value, string? darg, bool caseSensitive)
    {
        if (value is not null &&
            darg is not null &&
            string.Compare(value, darg, !caseSensitive) == 0) return null;

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
        this string? value, LambdaNodeArgument? darg, bool caseSensitive)
    {
        return value is not null && darg is not null
            ? NullWhenDynamicName(value, darg.LambdaName, caseSensitive)
            : value;
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
        this string? value, TokenArgument? darg, bool caseSensitive)
    {
        return value is not null && darg is not null
            ? NullWhenDynamicName(value, darg.Name, caseSensitive)
            : value;
    }
}