#pragma warning disable CS8763

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ArgumentNullExceptionExtensions
{
    extension(ArgumentNullException)
    {
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if the given value is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfNull<T>(
            [NotNull] T? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == null)
                throw new ArgumentNullException(name).WithData(value);
        }
    }
}