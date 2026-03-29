#if YOTEI_TOOLS_GENERATORS

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ArgumentOutOfRangeExceptionExtensions
{
    extension(ArgumentOutOfRangeException)
    {
        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfZero<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(default!) == 0) throw new ArgumentOutOfRangeException(
                "Value is zero.")
                .WithData(value, name);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is negative.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfNegative<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(default!) < 0) throw new ArgumentOutOfRangeException(
                "Value is negative.")
                .WithData(value, name);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is negative or
        /// zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfNegativeOrZero<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(default!) <= 0) throw new ArgumentOutOfRangeException(
                "Value is negative or zero.")
                .WithData(value, name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is equal to
        /// the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) == 0) throw new ArgumentOutOfRangeException(
                "Value is equal to other.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is not equal to
        /// the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfNotEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) != 0) throw new ArgumentOutOfRangeException(
                "Value is not equal to other.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is greater than
        /// the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfGreaterThan<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) > 0) throw new ArgumentOutOfRangeException(
                "Value is greater than other.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is greater than
        /// or equal to the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfGreaterThanOrEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) >= 0) throw new ArgumentOutOfRangeException(
                "Value is greater than other.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is less than
        /// the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfLessThan<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) < 0) throw new ArgumentOutOfRangeException(
                "Value is less than other.")
                .WithData(value, name)
                .WithData(other);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is less than or
        /// equal to the other given one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfLessThanOrEqual<T>(
            T value, T other,
            [CallerArgumentExpression(nameof(value))] string? name = null) where T : IComparable<T>
        {
            if (value.CompareTo(other) <= 0) throw new ArgumentOutOfRangeException(
                "Value is less than other.")
                .WithData(value, name)
                .WithData(other);
        }
    }
}

#endif