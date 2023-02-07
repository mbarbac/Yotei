namespace Dev.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given string is null or empty, or the original value instead. By
    /// default, the given value is trimmed before validating.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? NullWhenEmpty(this string? value, bool trim = true)
    {
        if (value != null)
        {
            if (trim)
            {
                var span = value.AsSpan().Trim();
                if (span.Length == 0) return null;
                if (span.Length == value.Length) return value;

                value = span.ToString();
            }

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        return value;
    }

    /// <summary>
    /// Validates that the given string value is not null and not empty. By default, the given
    /// value is trimmed before validating.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="EmptyException"></exception>
    public static string NotNullNotEmpty(
        this string? value,
        bool trim = true,
        [CallerArgumentExpression(nameof(value))] string? name = default)
    {
        name = name.NullWhenEmpty(trim) ?? nameof(value);

        value = value.ThrowIfNull();

        if ((value = value.NullWhenEmpty(trim)) == null)
            throw new EmptyException($"'{name}' cannot be empty.");

        return value;
    }
}