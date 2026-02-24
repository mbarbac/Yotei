#pragma warning disable CS8763

namespace Yotei.Tools.CoreGenerator;

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
        [DoesNotReturn]
        public static void ThrowIfNullOrEmpty(
            [NotNull] string? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{name} is empty.").WithData(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfNullOrWhiteSpace(
            [NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} is empty or white-spaces only.").WithData(value);
        }
    }
}