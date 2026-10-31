#if YOTEI_TOOLS_GENERATORS
#pragma warning disable CS0436
namespace Yotei.Tools.Generators;

// ========================================================
internal static class ArgumentOutOfRangeExceptionExtensions
{
    extension(ArgumentOutOfRangeException)
    {
        /// <summary>
        /// Throws an exception if the given value is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfZero<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(default!) == 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is zero.")
                .WithData(value, name);
        }

        /// <summary>
        /// Throws an exception if the given value is negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(default!) < 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is negative.")
                .WithData(value, name);
        }

        /// <summary>
        /// Throws an exception if the given value is negative or zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(default!) <= 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is negative zero.")
                .WithData(value, name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws an exception if the given value is equal to other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IEquatable<T>
        {
            if (value.Equals(other)) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is equal to 'other'.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws an exception if the given value is not equal to other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IEquatable<T>
        {
            if (!value.Equals(other)) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is not equal to 'other'.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws an exception if the given value is greater than other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThan<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) > 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is greater than 'other'.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws an exception if the given value is equal to or greater than other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThanOrEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) >= 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is equal to or greater than 'other'.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws an exception the given if value is less than other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThan<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) < 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is less than 'other'.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws an exception the given if value is equal to or less than other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThanOrEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) <= 0) throw new ArgumentOutOfRangeException(
                name, $"'{name}' is equal to or less than 'other'.")
                .WithData(value, name)
                .WithData(other);
        }
    }
}

#endif