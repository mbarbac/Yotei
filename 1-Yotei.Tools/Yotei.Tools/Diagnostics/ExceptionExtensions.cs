namespace Yotei.Tools.Diagnostics;

// ========================================================
public static class ExceptionExtensions
{
    /// <summary>
    /// Returns a string representation of the given exception that is suitable for display
    /// purposes.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string ToDisplayString(this Exception exception)
    {
        exception.ThrowWhenNull();

        var sb = new StringBuilder();
        while (exception != null)
        {
            sb.AppendLine($"> Exception: {exception.GetType().Name}");
            if (exception.Message != null) sb.AppendLine($"- Message: {exception.Message}");
            if (exception.Data.Count != 0)
            {
                sb.AppendLine("- Data:");
                foreach (DictionaryEntry item in exception.Data)
                {
                    var key = item.Key.ToString();
                    var value = item.Value is null ? "NULL" : item.Value.ToString();
                    sb.AppendLine($"\t- {key} = '{value}'");
                }
            }
            if (exception.StackTrace != null)
            {
                sb.AppendLine("- Trace:");
                sb.AppendLine(exception.StackTrace);
            }

            exception = exception.InnerException!;
            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine(_Separator);
            }
        }
        return sb.ToString();
    }

    // Used to separate inner exceptions...
    static readonly string _Separator = new('-', 40);
}