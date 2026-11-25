#if NETSTANDARD2_0
#pragma warning disable CS8777
namespace Yotei.Tools.NStd20;

// ========================================================
public static class ArgumentExceptionExtensions
{
    extension(ArgumentException)
    {
        /// <summary>
        /// Throws an exception if the given string value is null or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrEmpty(
            [NotNull] string? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Throws an exception if the given string value is null, empty or white space
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrWhiteSpace(
            [NotNull] string? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(name);
        }
    }
}
#endif


