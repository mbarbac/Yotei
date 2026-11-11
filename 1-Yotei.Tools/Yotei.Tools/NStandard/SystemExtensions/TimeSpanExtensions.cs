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
static class TimeSpanExtensions
{
    extension(TimeSpan source)
    {
        /// <summary>
        /// Returns the number of milliseconds in this isntance, such that it is either -1 to
        /// indicate an infinite timeout, or a value equal or greater than cero.
        /// </summary>
        public long ValidatedTimeout
        {
            get
            {
                var ms = (long)source.TotalMilliseconds;

                if (ms is < (-1) or > long.MaxValue) throw new ArgumentOutOfRangeException(
                    nameof(source),
                    $"Invalid timeout value in ms: {ms}");

                return ms;
            }
        }
    }
}