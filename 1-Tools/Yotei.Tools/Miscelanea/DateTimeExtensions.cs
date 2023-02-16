namespace Yotei.Tools;

// ========================================================
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a new instance based upon the values of the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static DayDate ToDayDate(this DateTime source) => new(source);

    /// <summary>
    /// Returns a new instance based upon the values of the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static DayDate ToDayDate(this DateOnly source) => new(source);

    /// <summary>
    /// Returns a new instance based upon the values of the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ClockTime ToClockTime(this DateTime source) => new(source);

    /// <summary>
    /// Returns a new instance based upon the values of the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ClockTime ToClockTime(this TimeSpan source) => new(source);

    /// <summary>
    /// Returns a new instance based upon the values of the given one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ClockTime ToClockTime(this TimeOnly source) => new(source);

    /// <summary>
    /// Splits the given DateTime into its calendar and clock components.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static (DayDate date, ClockTime time) ToCalendarAndClock(
        this DateTime source)
        => (source.ToDayDate(), source.ToClockTime());
}