namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ExceptionExtensions
{
    /// <summary>
    /// Adds or replaces in the exception's data dictionary the entry with the given name and
    /// value, where the name is by default the name of the value variable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T WithData<T>(
        this T source,
        object? value,
        [CallerArgumentExpression(nameof(value))] string? name = null) where T : Exception
    {
        source.ThrowWhenNull();
        if (string.IsNullOrWhiteSpace(name)) name = nameof(source);

        source.Data[name] = value;
        return source;
    }

    // ----------------------------------------------------

    static readonly string SEPARATOR = new('-', 40);

    /// <summary>
    /// Returns a string representation of the exception that is suitable for display purposes,
    /// and that includes all inner exceptions.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToDisplayString(this Exception source)
    {
        source.ThrowWhenNull();

        var sb = new StringBuilder(); Generate(sb, source);
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