#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR

internal static class ArgumentOutOfRangeExceptionExtensions
{
    extension(ArgumentOutOfRangeException)
    {
        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is zero.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void ThrowIfZero(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(
                "Value is zero.")
                .WithData(value, name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(
                "Value is negative.")
                .WithData(value, name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is negative or
        /// cero.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegativeOrZero(
            int value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(
                "Value is negative or cero.")
                .WithData(value, name);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is less than the
        /// other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThan(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < other) throw new ArgumentOutOfRangeException(
                "Value is less than the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is less than or
        /// equal the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLessThanOrEqual(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value <= other) throw new ArgumentOutOfRangeException(
                "Value is less than or equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is equal the other
        /// given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfEqual(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == other) throw new ArgumentOutOfRangeException(
                "Value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is not equal the
        /// other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotEqual(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value != other) throw new ArgumentOutOfRangeException(
                "Value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is greather than
        /// the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThan(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value > other) throw new ArgumentOutOfRangeException(
                "Value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }

        // ------------------------------------------------

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if the given value is greather than
        /// or equal to the other given one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThanOrEqual(
            int value, int other,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value >= other) throw new ArgumentOutOfRangeException(
                "Value is equal to the other given one.")
                .WithData(value, name)
                .WithData(other);
        }
    }
}

#endif