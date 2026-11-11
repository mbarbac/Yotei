#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class ExceptionExtensions
{
    extension<T>(T source) where T : Exception
    {
        /// <summary>
        /// Adds or replaces the given [name=value] pair in the exception's dictionary.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T WithData(object? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (string.IsNullOrWhiteSpace(name)) name = nameof(value);
            source.Data[name] = value;
            return source;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a string representation of the exception suitable for display purposes. It
        /// includes both the inner exceptions and the exception's data dictionary.
        /// </summary>
        /// <returns></returns>
        public string ToDisplayString()
        {
            var sb = new StringBuilder(); Populate(sb, source);
            return sb.ToString();

            // Recursively populates the builder with the given exception.
            static void Populate(StringBuilder sb, Exception ex)
            {
                sb.AppendLine($"> Exception: {ex.GetType().Name}");
                
                if (ex.Message != null &&
                    ex.Message.Length > 0) sb.Append($"- Message: {ex.Message}");

                if (ex.Data.Count != 0)
                {
                    sb.AppendLine("- Data:"); foreach (DictionaryEntry? item in ex.Data)
                    {
                        if (!item.HasValue) continue;

                        var key = item.Value.Key.ToString();
                        var value = item.Value.Value is null ? "NULL" : item.Value.Value.ToString();
                        sb.AppendLine($"\t- {key} = '{value}'");
                    }
                }

                if (ex.StackTrace != null)
                {
                    sb.AppendLine("- Trace:");
                    sb.AppendLine(ex.StackTrace);
                }

                if (ex is AggregateException ag)
                {
                    foreach (var inner in ag.InnerExceptions)
                    {
                        sb.AppendLine();
                        sb.AppendLine(SEPARATOR);
                        Populate(sb, inner);
                    }
                }

                if (ex.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine(SEPARATOR);
                    Populate(sb, ex.InnerException);
                }
            }
        }
    }

    // ----------------------------------------------------

    static readonly string SEPARATOR = new('-', 40);
}