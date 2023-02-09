namespace Yotei.Generator
{
    // ====================================================
    public static class StringExtensions
    {
        /// <summary>
        /// Returns null if the given string is null or empty, or the original value instead.
        /// By default, the given value is trimmed before validating.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Validates that the given string value is not null and not empty. By default, the
        /// given value is trimmed before validating.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="trim"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotNullNotEmpty(
            this string? value,
            string valueName,
            bool trim = true)
        {
            valueName = valueName.NullWhenEmpty(trim) ?? nameof(value);

            value = value.ThrowIfNull(nameof(value));

            if ((value = value.NullWhenEmpty(trim)) == null)
                throw new EmptyException($"'{valueName}' cannot be empty.");

            return value;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns an array with the dot-separated parts of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] ToParts(this string value)
        {
            value = value.ThrowIfNull(nameof(value)); // Just preventing null, not adjusting...
            return value.Split('.');
        }

        /// <summary>
        /// Returns the last dot-separated part of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLastPart(this string value)
        {
            value = value.ThrowIfNull(nameof(value)); // Just preventing null, not adjusting...

            if (!value.Contains('.')) return value;
            else
            {
                var parts = value.Split('.');
                return parts[parts.Length - 1];
            }
        }
    }
}