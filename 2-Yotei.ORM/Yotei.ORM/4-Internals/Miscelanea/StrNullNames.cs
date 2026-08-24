namespace Yotei.ORM.Internals;

// ========================================================
public static class StrNullNames
{
    /// <summary>
    /// Returns <see langword="null"/> when the given value matches the given dynamic argument's
    /// name. Returns the original value otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(this string? value, string? darg, bool ignoreCase)
    {
        if (value is not null &&
            darg is not null &&
            string.Compare(value, darg, ignoreCase) == 0) return null;

        return value;
    }

    /// <summary>
    /// Returns <see langword="null"/> when the given value matches the given dynamic argument's
    /// name. Returns the original value otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(
        this string? value, LambdaNodeArgument? darg, bool ignoreCase)
        => value is not null && darg is not null
        ? NullWhenDynamicName(value, darg.LambdaName, ignoreCase)
        : value;

    /// <summary>
    /// Returns <see langword="null"/> when the given value matches the given dynamic argument's
    /// name. Returns the original value otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(
        this string? value, DbTokenArgument? darg, bool ignoreCase)
        => value is not null && darg is not null
        ? NullWhenDynamicName(value, darg.Name, ignoreCase)
        : value;
}