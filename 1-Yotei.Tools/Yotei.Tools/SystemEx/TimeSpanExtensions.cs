namespace Yotei.Tools;

// ========================================================
public static class TimeSpanExtensions
{
    extension(TimeSpan source)
    {
        /// <summary>
        /// Validates that the value carried by this source instance is a valid timeout one and,
        /// if so, returns the number of milliseconds: cero or greater, or -1 to indicate an
        /// infinite one. Otherwise, throws an exception.
        /// </summary>
        public long ValidatedTimeout
        {
            get
            {
                var ms = (long)source.TotalMilliseconds;

                if (ms is < (-1) or > uint.MaxValue) throw new ArgumentOutOfRangeException(
                    nameof(source),
                    $"Invalid timeout: {ms}");

                return ms;
            }
        }
    }
}