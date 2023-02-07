namespace Dev.Tools;

// ========================================================
public static class ExceptionExtensions
{
    /// <summary>
    /// Throws a <see cref="ArgumentNullException"/> exception if the given value is null.
    /// Otherwise, returns the original value itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T ThrowIfNull<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))] string? name = default)
    {
        name = name.NullWhenEmpty() ?? nameof(value);

        if (value is null)
            throw new ArgumentNullException(name, $"'{name}' cannot be null.");

        return value;
    }

    /// <summary>
    /// Returns a string representation of the given exception, suitable for display purposes.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string ToDisplayString(this Exception exception)
    {
        exception = exception.ThrowIfNull();

        var sb = new StringBuilder();
        sb.AppendLine();
        while (exception != null)
        {
            sb.AppendLine($"> Exception: {exception.GetType().Name}");
            if (exception.Message != null) sb.AppendLine($"- Message: {exception.Message}");
            if (exception.Data.Count != 0)
            {
                sb.AppendLine($"- Data:");
                foreach (var obj in exception.Data)
                {
                    var kvp = (DictionaryEntry)obj!;
                    var key = kvp.Key.ToString();
                    var value = kvp.Value?.ToString() ?? "-";
                    sb.AppendLine($"\t- {key} = '{value}'");
                }
            }

            if (exception.StackTrace != null)
            {
                sb.AppendLine($"- Trace:");
                sb.AppendLine(exception.StackTrace);
            }

            exception = exception.InnerException!;
            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine("----------------------------------------");
            }
        }

        return sb.ToString();
    }
}