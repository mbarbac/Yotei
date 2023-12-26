namespace Yotei.Tools.Miscelanea;

// ========================================================
public static class ClockAndDayExtensions
{
    /// <summary>
    /// Splits the given value into its <see cref="DayDate"/> and <see cref="ClockTime"/> parts.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static (DayDate date, ClockTime time) ToDayDateClockTime(this DateTime source)
    {
        return (source.ToDayDate(), source.ToClockTime());
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new <see cref="DayDate"/> instance based upon the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static DayDate ToDayDate(this DateTime source) => new(source);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new <see cref="ClockTime"/> instance based upon the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ClockTime ToClockTime(this DateTime source) => new(source);

    /// <summary>
    /// Returns a new <see cref="ClockTime"/> instance based upon the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ClockTime ToClockTime(this TimeSpan source) => new(source);
}