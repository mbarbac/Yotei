namespace Yotei.Tools;

// ========================================================
public static class TimeOnlyExtensions
{
    extension(TimeOnly source)
    {
        /// <summary>
        /// Returns either an ISO-compliant "hh:mm" representation or a representation with an
        /// arbitrary level of precission "hh:mm:ss.milliseconds.microseconds.nanoseconds",
        /// where the separators can be modified as needed.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="dot"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        /// <param name="microseconds"></param>
        /// <param name="nanoseconds"></param>
        /// <returns></returns>
        public string ToIsoString(
            char separator = ':',
            char dot = '.',
            bool seconds = false,
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
                sb.Append(dot).Append(source.Millisecond.ToString("D3"));

            if (microseconds || nanoseconds)
                sb.Append(dot).Append(source.Microsecond.ToString("D3"));

            if (nanoseconds)
                sb.Append(dot).Append(source.Nanosecond.ToString("D3"));

            return sb.ToString();
        }
    }
}