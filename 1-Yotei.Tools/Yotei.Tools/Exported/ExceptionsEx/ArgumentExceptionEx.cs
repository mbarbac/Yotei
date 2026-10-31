#if YOTEI_TOOLS_GENERATORS
#pragma warning disable CS0436
namespace Yotei.Tools.Generators;

// ========================================================
internal static class ArgumentExceptionExtensions
{
    extension(ArgumentException)
    {
        /// <summary>
        /// Throws an exception if value is null or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrEmpty(
            [NotNull] string? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value is null) throw new ArgumentNullException(name);
            if (value.Length == 0) throw new EmptyException($"'{name}' is empty.");
        }

        /// Throws an exception if value is null, empty, or white spaces only.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrWhiteSpace(
            [NotNull] string? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value is null) throw new ArgumentNullException(name);
            if (value.Length == 0) throw new EmptyException($"'{name}' is empty.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{name}' is whitespaces only.");
        }
    }
}

#endif