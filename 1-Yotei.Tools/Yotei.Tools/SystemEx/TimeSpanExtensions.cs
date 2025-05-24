namespace Yotei.Tools;

// ========================================================
public static class TimeSpanExtensions
{
    /// <summary>
    /// Validates the given <see cref="TimeSpan"/> value carries a valid timeout one, which must
    /// be cero or greater, or -1 to indicate an infinite one.
    /// <br/> Returns either -1 or the actual number of milliseconds.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static long ValidateTimeout(this TimeSpan timeout)
    {
        var ms = (long)timeout.TotalMilliseconds;

        if (ms is < (-1) or > uint.MaxValue) throw new ArgumentOutOfRangeException(
            nameof(timeout),
            $"Invalid timeout: {timeout}");

        return ms;
    }
}