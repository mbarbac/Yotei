namespace Yotei.Tools;

// ========================================================
public static class TimeOnlyExtensions
{
    extension(TimeOnly source)
    {
        /// <summary>
        /// Returns an ISO-compliant representation.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToIsoString(char separator = ':',
            bool seconds = true,
            bool milliseconds = false,
            bool microseconds = false,
            bool nanoseconds = false)
        {
            var sb = new StringBuilder()
                .Append(source.Hour.ToString("D2"))
                .Append(separator)
                .Append(source.Minute.ToString("D2"));

            if (seconds || milliseconds || microseconds || nanoseconds)
                sb.Append(separator).Append(source.Second.ToString("D2"));

            if (milliseconds || microseconds || nanoseconds)
                sb.Append('.').Append(source.Millisecond.ToString("D3"));

            if (microseconds || nanoseconds)
                sb.Append('.').Append(source.Microsecond.ToString("D3"));

            if (nanoseconds)
                sb.Append('.').Append(source.Nanosecond.ToString("D3"));

            return sb.ToString();
        }
    }
}