namespace Yotei.ORM.Internals;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns <c>null</c> if the given value matches the given name of the dynamic argument,
    /// or the original value otherwise.
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
    /// Returns <c>null</c> if the given value matches the given name of the dynamic argument,
    /// or the original value otherwise.
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
    /// Returns <c>null</c> if the given value matches the given name of the dynamic argument,
    /// or the original value otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="darg"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string? NullWhenDynamicName(
        this string? value, DbTokenArgument? darg, bool caseSensitive)
    {
        return value is not null && darg is not null
            ? NullWhenDynamicName(value, darg.Name, caseSensitive)
            : value;
    }
}