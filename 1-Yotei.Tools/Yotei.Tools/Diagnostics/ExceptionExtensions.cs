namespace Yotei.Tools.Diagnostics;

// ========================================================
public static class ExceptionExtensions
{
    /// <summary>
    /// Returns a string representation of the given string suitable for display purposes.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string ToDisplayString(this Exception exception)
    {
        exception = exception.ThrowWhenNull();

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
                    var value = kvp.Value == null ? "NULL" : kvp.Value.ToString();
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