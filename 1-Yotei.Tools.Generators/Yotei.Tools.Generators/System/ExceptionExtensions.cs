namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class ExceptionExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> exception if the given value is null.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T ThrowWhenNull<T>(
        [AllowNull] this T value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        name = name.NullWhenEmpty() ?? nameof(value);

        if (value is null) throw new ArgumentNullException(name);
        return value;
    }

    /// <summary>
    /// Adds to the exception's data dictionary an entry with the given key name and value, or
    /// replaces the existing one. Returns the original exception for a fluent-syntax usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception"></param>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T WithData<T>(
        this T exception,
        object? value,
        [CallerArgumentExpression(nameof(value))] string? name = null) where T : Exception
    {
        exception.ThrowWhenNull();

        name = name.NotNullNotEmpty();
        exception.Data[name] = value;
        return exception;
    }
}