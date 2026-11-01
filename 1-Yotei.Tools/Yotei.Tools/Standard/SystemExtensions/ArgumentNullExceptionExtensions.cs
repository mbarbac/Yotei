#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;

// ========================================================
internal static class ArgumentNullExceptionExtensions
{
#pragma warning disable IDE0079
#pragma warning disable CS0436
    extension(ArgumentNullException)
    {
        /// <summary>
        /// Throws an exception if the given value is null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull<T>(
            [NotNull] T? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value is null) throw new ArgumentNullException(name);
        }
    }
#pragma warning restore
}

#endif