namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class ExceptionExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> exception when the given value is null.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowWhenNull<T>(this T? value, string? description)
    {
        description = description.NullWhenEmpty() ?? nameof(value);

        if (value == null)
            throw new ArgumentNullException($"'{description}' is null.");

        return value;
    }

    /// <summary>
    /// Adds to the data dictionary of the given exception an entry with the given name and value.
    /// An existing entry with that name gets its value updated. Returns the given exception.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception"></param>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T WithData<T>(
        this T exception,
        object? value,
        string? name) where T : Exception
    {
        exception = exception.ThrowWhenNull(nameof(exception));

        name = name.NullWhenEmpty() ?? nameof(value);
        exception.Data[name] = value;
        return exception;
    }
}