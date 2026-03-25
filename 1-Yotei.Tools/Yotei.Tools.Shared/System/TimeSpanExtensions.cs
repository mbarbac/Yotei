#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
static class TimeSpanExtensions
{
    extension(TimeSpan source)
    {
        /// <summary>
        /// Returns the total number of milliseconds carried by this instance, after having validated
        /// it as a timeout value: cero or greater, or -1 to indicate an infinite one.
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