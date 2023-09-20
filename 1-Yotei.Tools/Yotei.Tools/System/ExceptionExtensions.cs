namespace Yotei.Tools;

// ========================================================
public static class ExceptionExtensions
{
    /// <summary>
    /// Adds to the data dictionary of the given exception an entry for the given name and value
    /// pair. If that entry already exists, it is replaced with the new one. Returns the original
    /// exception to support fluent syntax.
    /// <para>If the 'name' argument was omitted, then the value name is used.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception"></param>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T WithData<T>(
        this T exception,
        object? value,
        [CallerArgumentExpression(nameof(value))] string? name = default) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(exception);        

        name = name.NullWhenEmpty() ?? nameof(value);
        exception.Data[name] = value;
        return exception;
    }
}