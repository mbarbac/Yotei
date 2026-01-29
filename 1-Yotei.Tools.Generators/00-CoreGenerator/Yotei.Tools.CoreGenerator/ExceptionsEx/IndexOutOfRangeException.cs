#pragma warning disable CS8763

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class IndexOutOfRangeExceptionExtensions
{
    extension(IndexOutOfRangeException)
    {
        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfNegative<T>(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 0) throw new IndexOutOfRangeException(
                "Index value is negative.")
                .WithData(value, name);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is negative or
        /// cero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfNegativeOrZero<T>(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= 0) throw new IndexOutOfRangeException(
                "Index value is negative or cero.")
                .WithData(value, name);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is less than the
        /// other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfLessThan<T>(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < other) throw new IndexOutOfRangeException(
                "Index value is less than the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is less than or
        /// equal the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfLessThanOrEqual<T>(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= other) throw new IndexOutOfRangeException(
                "Index value is less than or equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is equal the other
        /// given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfEqual<T>(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == other) throw new IndexOutOfRangeException(
                "Index value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is not equal the
        /// other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfNotEqual<T>(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value != other) throw new IndexOutOfRangeException(
                "Index value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is greather than
        /// the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfGreaterThan<T>(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value > other) throw new IndexOutOfRangeException(
                "Index value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="IndexOutOfRangeException"/> if the given value is greather than
        /// or equal to the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DoesNotReturn]
        public static void ThrowIfGreaterThanOrEqual<T>(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value >= other) throw new IndexOutOfRangeException(
                "Index value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }
    }
}