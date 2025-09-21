namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Extensions for <see cref="Exception"/> instances.
/// TODO: Use C# 14 static extension methods to extend 'Exception' capabilities.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Adds or replaces in the given exception's data dictionary the entry with the given name
    /// and value, where the name is by default the name of the value variable.
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