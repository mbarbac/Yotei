namespace Yotei.Generators
{
    // ====================================================
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> exception if the given value is null.
        /// Otherwise, returns the original value itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(
            this T value,
            string valueName)
        {
            valueName = valueName.NullWhenEmpty() ?? nameof(value);

            if (value is null)
                throw new ArgumentNullException(valueName, $"'{valueName}' cannot be null.");

            return value;
        }

        /// <summary>
        /// Adds or replaces in the exception's data dictionary the entry whose name and value
        /// are given. By default, the entry name is the caller argument expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception"></param>
        /// <param name="value"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static T WithData<T>(
            this T exception,
            object? value,
            string valueName)
            where T : Exception
        {
            exception = exception.ThrowIfNull(nameof(exception));

            valueName = valueName.NullWhenEmpty() ?? nameof(value);

            exception.Data[valueName] = value;
            return exception;
        }
    }
}