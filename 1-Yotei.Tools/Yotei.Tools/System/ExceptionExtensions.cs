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

        name = name.NotNullNotEmpty(true);
        exception.Data[name] = value;
        return exception;
    }

    // ----------------------------------------------------

    static readonly string SEPARATOR = new('-', 40);

    /// <summary>
    /// Returns a string representation of the given exception that is suitable for dislay
    /// purposes.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string ToDisplayString(this Exception exception)
    {
        exception.ThrowWhenNull();

        var sb = new StringBuilder(); Generate(sb, exception);
        return sb.ToString();

        /// <summary>
        /// Invoked to generate the display string of the given exception.
        /// </summary>
        static void Generate(StringBuilder sb, Exception ex)
        {
            sb.AppendLine($"> Exception: {ex.GetType().Name}");
            if (ex.Message != null) sb.AppendLine($"- Message: {ex.Message}");
            if (ex.Data.Count != 0)
            {
                sb.AppendLine("- Data:");
                foreach (DictionaryEntry item in ex.Data)
                {
                    var key = item.Key.ToString();
                    var value = item.Value is null ? "NULL" : item.Value.ToString();
                    sb.AppendLine($"\t- {key} = '{value}'");
                }
            }

            if (ex.StackTrace != null)
            {
                sb.AppendLine("- Trace:");
                sb.AppendLine(ex.StackTrace);
            }

            if (ex is AggregateException agg)
            {
                foreach (var inner in agg.InnerExceptions)
                {
                    sb.AppendLine();
                    sb.AppendLine(SEPARATOR);
                    Generate(sb, inner);
                }
            }

            else if (ex.InnerException is not null)
            {
                sb.AppendLine();
                sb.AppendLine(SEPARATOR);
                Generate(sb, ex.InnerException);
            }
        }
    }
}