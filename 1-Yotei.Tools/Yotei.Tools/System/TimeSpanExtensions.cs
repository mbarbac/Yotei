namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Extensions for <see cref="TimeSpan"/> instances.
/// TODO: Use C# 14 static extension methods to extend 'TimeSpan' capabilities.
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Validates that the given time span carries a valid timeout value, cero or greater one, or
    /// or -1 to indicate an infinite one.
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