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

        var sb = new StringBuilder(); OnDisplayString(sb, exception);
        return sb.ToString();
    }

    // Used to generate the display string of the given exception...
    static void OnDisplayString(StringBuilder sb, Exception ex)
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
                sb.AppendLine(_Separator);
                OnDisplayString(sb, inner);
            }
        }

        else if (ex.InnerException is not null)
        {
            sb.AppendLine();
            sb.AppendLine(_Separator);
            OnDisplayString(sb, ex.InnerException);
        }
    }

    // Used to separate inner exceptions...
    static readonly string _Separator = new('-', 40);
}