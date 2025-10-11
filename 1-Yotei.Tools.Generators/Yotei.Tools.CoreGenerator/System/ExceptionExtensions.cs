namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class ExceptionExtensions
{
    /// <summary>
    /// Adds or replaces in the exception's data dictionary the entry with the given name and
    /// value, where the name is by default the name of the value variable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception"></param>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// Needs to be placed outside extension block for CallerArgumentExpression to work.
    public static T WithData<T>(
        this T exception,
        object? value,
        [CallerArgumentExpression(nameof(value))] string? name = null) where T : Exception
    {
        exception.ThrowWhenNull();

        name = name.NotNullNotEmpty(true);
        exception.Data[name] = value;
        return exception;
    }
}