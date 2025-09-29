namespace Yotei.Tools;

// =============================================================
public static class TimeSpanExtensions
{    
    extension(TimeSpan value)
    {
        /// <summary>
        /// Validates that this instance carries a valid timeout value (cero or greater, or -1 to
        /// indicate an infinite timeout) and returns it as a long integer.
        /// </summary>
        /// <returns></returns>
        public long ValidatedTimeout
        {
            get
            {
                var ms = (long)value.TotalMilliseconds;

                if (ms is < (-1) or > uint.MaxValue) throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Invalid timeout: {ms}");

                return ms;
            }
        }
    }
}