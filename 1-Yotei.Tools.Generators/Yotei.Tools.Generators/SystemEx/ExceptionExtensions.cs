namespace Yotei.Tools.Generators;

// ========================================================
internal static class ExceptionExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> exception if the given value is a null one.
    /// Otherwise, returns the original value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowWhenNull<T>(this T? value, string description)
    {
        description = description.NullWhenEmpty() ?? nameof(value);

        if (value == null)
            throw new ArgumentNullException($"'{description}' cannot be null.");

        return value;
    }

    /// <summary>
    /// Adds to the data dictionary of the given exception an entry for the given name and value
    /// pair. If that entry already exists, it is replaced with the new one. Returns the original
    /// exception to support fluent syntax.
    /// <br/> If the 'name' argument was omitted, then the 'value' name is used, which may cause
    /// to overwrite previous entries.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception"></param>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T WithData<T>(
        this T exception,
        object? value,
        string name) where T : Exception
    {
        name = name.NullWhenEmpty() ?? nameof(value);
        exception.ThrowWhenNull(name);

        exception.Data[name] = value;
        return exception;
    }
}