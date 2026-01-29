#pragma warning disable CS8763

using System.Numerics;

namespace Yotei.Tools;

// ========================================================
public static class IndexOutOfRangeExceptionExtensions
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
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
        {
            if (value < T.Zero) throw new IndexOutOfRangeException(
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
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
        {
            if (value <= T.Zero) throw new IndexOutOfRangeException(
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
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
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
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
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
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
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
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
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
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
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
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
            where T : INumber<T>
        {
            if (value >= other) throw new IndexOutOfRangeException(
                "Index value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }
    }
}