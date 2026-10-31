namespace Yotei.Tools.Miscelanea;

// ========================================================
/// <summary>
/// Represent an arbitrary time in a 24-hour clock format.
/// </summary>
public sealed record ClockTime : IComparable<ClockTime>
{
    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public ClockTime() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="millisecond"></param>
    public ClockTime(int hour, int minute = 0, int second = 0, int millisecond = 0)
    {
        Hour = hour;
        Minute = minute;
        Second = second;
        Millisecond = millisecond;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public ClockTime(ClockTime source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Hour = source.Hour;
        Minute = source.Minute;
        Second = source.Second;
        Millisecond = source.Millisecond;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{Hour:00}:{Minute:00}");
        if (Second != 0 || Millisecond != 0) sb.Append($":{Second:00}");
        if (Millisecond != 0) sb.Append($".{Millisecond:000}");
        return sb.ToString();
    }

    /// <summary>
    /// Returns a string representation of this instance using the given format:
    /// 'HH', 'mm', 'ss' and 'fff' case sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString([StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string format)
    {
        var item = (TimeOnly)this;
        return item.ToString(format);
    }

    /// <summary>
    /// Returns a string representation of this instance using the rules of the given culture.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(IFormatProvider provider)
    {
        var item = (TimeOnly)this;
        return item.ToString(provider);
    }

    /// <summary>
    /// Returns a string representation of this instance using the rules of the given culture
    /// and using the given format: 'HH', 'mm', 'ss' and 'fff' case sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string format,
        IFormatProvider provider)
    {
        var item = (TimeOnly)this;
        return item.ToString(format, provider);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given date part.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public DateTime ToDateTime(DateOnly value) => new(
        value.Year, value.Month, value.Day, 
        Hour, Minute, Second, Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ClockTime(DateTime value)
        => new(value.Hour, value.Minute, value.Second, value.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator TimeOnly(ClockTime value)
        => new(value.Hour, value.Minute, value.Second, value.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ClockTime(TimeOnly value)
        => new(value.Hour, value.Minute, value.Second, value.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator TimeSpan(ClockTime value)
        => new(0, value.Hour, value.Minute, value.Second, value.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ClockTime(TimeSpan value)
        => new(value.Hours, value.Minutes, value.Seconds, value.Milliseconds);

    // ----------------------------------------------------

    /// <summary>
    /// The hour, from 0 to 23.
    /// </summary>
    public int Hour
    {
        get;
        init => field = (value is >= 0 and <= 23) ? value :
            throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    }

    /// <summary>
    /// The minute, from 0 to 59.
    /// </summary>
    public int Minute
    {
        get;
        init => field = (value is >= 0 and <= 59) ? value :
            throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    }

    /// <summary>
    /// The second, from 0 to 59.
    /// </summary>
    public int Second
    {
        get;
        init => field = (value is >= 0 and <= 59) ? value :
            throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    }

    /// <summary>
    /// The milisecond, from 0 to 999.
    /// </summary>
    public int Millisecond
    {
        get;
        init => field = (value is >= 0 and <= 999) ? value :
            throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    }

    /// <summary>
    /// Gets a new instance with the current date in local coordinates.
    /// </summary>
    public static ClockTime Now => DateTime.Now;

    /// <summary>
    /// Gets a new instance with the current date in global coordinates.
    /// </summary>
    public static ClockTime UtcNow => DateTime.UtcNow;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IComparable.CompareTo(object?)"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(ClockTime? x, ClockTime? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;

        if (x.Hour != y.Hour) return x.Hour < y.Hour ? -1 : +1;
        if (x.Minute != y.Minute) return x.Minute < y.Minute ? -1 : +1;
        if (x.Second != y.Second) return x.Second < y.Second ? -1 : +1;
        if (x.Millisecond != y.Millisecond) return x.Millisecond < y.Millisecond ? -1 : +1;

        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(ClockTime? other) => Compare(this, other);

    public static bool operator >(ClockTime? x, ClockTime? y) => Compare(x, y) > 0;
    public static bool operator <(ClockTime? x, ClockTime? y) => Compare(x, y) < 0;
    public static bool operator >=(ClockTime? x, ClockTime? y) => Compare(x, y) >= 0;
    public static bool operator <=(ClockTime? x, ClockTime? y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given values have been added to (or substracted from)
    /// the current ones. This method discards any overflow (or underflow) in the number of days.
    /// </summary>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    /// <param name="seconds"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public ClockTime Add(
        int hours, int minutes = 0, int seconds = 0, int milliseconds = 0)
        => Add(out _, hours, minutes, seconds, milliseconds);

    /// <summary>
    /// Returns a new instance where the given values have been added to (or substracted from)
    /// the current ones. This method sets the out argument with the proper number of days'
    /// overflow (or underflow).
    /// </summary>
    /// <param name="days"></param>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    /// <param name="seconds"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public ClockTime Add(
         out int days,
         int hours, int minutes = 0, int seconds = 0, int milliseconds = 0)
    {
        days = 0;
        if (hours == 0 && minutes == 0 && seconds == 0 && milliseconds == 0) return this;

        milliseconds += Millisecond;
        if (milliseconds > 999) { seconds += milliseconds / 1000; milliseconds %= 1000; }
        if (milliseconds < 0) { seconds += milliseconds / 1000 - 1; milliseconds = 1000 + milliseconds % 1000; }

        seconds += Second;
        if (seconds > 59) { minutes += seconds / 60; seconds %= 60; }
        if (seconds < 0) { minutes += seconds / 60 - 1; seconds = 60 + seconds % 60; }

        minutes += Minute;
        if (minutes > 59) { hours += minutes / 60; minutes %= 60; }
        if (minutes < 0) { hours += minutes / 60 - 1; minutes = 60 + minutes % 60; }

        hours += Hour;
        if (hours > 23) { days = hours / 24; hours %= 24; }
        if (hours < 0) { days = hours / 24 - 1; hours = 24 + hours % 24; }

        return new(hours, minutes, seconds, milliseconds);
    }
}