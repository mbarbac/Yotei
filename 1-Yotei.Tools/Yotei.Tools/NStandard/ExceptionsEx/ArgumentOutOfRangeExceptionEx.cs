#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;

// ========================================================
public static class ArgumentOutOfRangeExceptionExtensions
{
    extension(ArgumentOutOfRangeException)
    {
        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is equal to other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfEqual<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value.EqualsEx(other)) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is not equal to other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotEqual<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (!value.EqualsEx(other)) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is greater than other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThan<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) > 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is greater than or equal to other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThanOrEqual<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) >= 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is less than other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThan<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) < 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is less than or equal to other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThanOrEqual<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) <= 0) throw new ArgumentOutOfRangeException(name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative<T>(
            short value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative<T>(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative<T>(
            long value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is negative or zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero<T>(
            short value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is negative or zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero<T>(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is negative or zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero<T>(
            long value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfZero<T>(
            short value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfZero<T>(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=nameof(value)></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfZero<T>(
            long value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(name);
        }
    }
}
#endif


