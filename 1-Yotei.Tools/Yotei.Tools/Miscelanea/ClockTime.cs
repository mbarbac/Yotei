namespace Yotei.Tools.Miscelanea;

// ========================================================
/// <summary>
/// Represents an arbitrary time moment in a 24-hours clock format.
/// </summary>
public record class ClockTime : IComparable<ClockTime>
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
    public ClockTime(int hour, int minute, int second = 0, int millisecond = 0)
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
        source = source.ThrowWhenNull();

        _Hour = source.Hour;
        _Minute = source.Minute;
        _Second = source.Second;
        _Millisecond = source.Millisecond;
    }

    /// <summary>
    /// Returns a new object using the values of this instance and the ones from the given day.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public DateTime ToDateTime(DayDate day)
    {
        day.ThrowWhenNull();
        return new(day.Year, day.Month, day.Day, Hour, Minute, Second, Millisecond);
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator ClockTime(DateTime item)
        => new(item.Hour, item.Minute, item.Second, item.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator TimeSpan(ClockTime item)
        => new(0, item.Hour, item.Minute, item.Second, item.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator ClockTime(TimeSpan item)
        => new(item.Hours, item.Minutes, item.Seconds, item.Milliseconds);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator TimeOnly(ClockTime item)
        => new(item.Hour, item.Minute, item.Second, item.Millisecond, 0);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator ClockTime(TimeOnly item)
        => new(item.Hour, item.Minute, item.Second, item.Millisecond);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string that represents this instance in a standard format for the current
    /// culture.
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
    /// Returns a string representation of this instance using the given format, that uses the
    /// 'HH', 'mm', 'ss' and 'fff' case sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string? format)
    {
        var item = (TimeOnly)this;
        return item.ToString(format);
    }

    /// <summary>
    /// Returns a string representation of this instance using the given culture information.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(IFormatProvider? provider)
    {
        var item = (TimeOnly)this;
        return item.ToString(provider);
    }

    /// <summary>
    /// Returns a string representation of this instance using the given format and culture
    /// information. Format uses the 'HH', 'mm', 'ss' and 'fff' case sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string? format,
        IFormatProvider? provider)
    {
        var item = (TimeOnly)this;
        return item.ToString(format, provider);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The hours' value, from 0 to 23.
    /// </summary>
    public int Hour
    {
        get => _Hour;
        init
        {
            if (value < 0) throw new ArgumentException("Value cannot be less than cero.").WithData(value);
            if (value > 23) throw new ArgumentException("Value cannot be bigger than 23.").WithData(value);
            _Hour = value;
        }
    }
    int _Hour;

    /// <summary>
    /// The minutes' value, from 0 to 59.
    /// </summary>
    public int Minute
    {
        get => _Minute;
        init
        {
            if (value < 0) throw new ArgumentException("Value cannot be less than cero.").WithData(value);
            if (value > 59) throw new ArgumentException("Value cannot be bigger than 59.").WithData(value);
            _Minute = value;
        }
    }
    int _Minute;

    /// <summary>
    /// The seconds' value, from 0 to 59.
    /// </summary>
    public int Second
    {
        get => _Second;
        init
        {
            if (value < 0) throw new ArgumentException("Value cannot be less than cero.").WithData(value);
            if (value > 59) throw new ArgumentException("Value cannot be bigger than 59.").WithData(value);
            _Second = value;
        }
    }
    int _Second;

    /// <summary>
    /// The milliseconds' value, from 0 to 999.
    /// </summary>
    public int Millisecond
    {
        get => _Millisecond;
        init
        {
            if (value < 0) throw new ArgumentException("Value cannot be less than cero.").WithData(value);
            if (value > 999) throw new ArgumentException("Value cannot be bigger than 999.").WithData(value);
            _Millisecond = value;
        }
    }
    int _Millisecond;

    // ----------------------------------------------------

    /// <summary>
    /// Gets a new instance with the current time in local coordinates.
    /// </summary>
    public static ClockTime Now => DateTime.Now;

    /// <summary>
    /// Gets a new instance with the current time in global coordinates.
    /// </summary>
    public static ClockTime UtcNow => DateTime.UtcNow;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given number of hours, minutes, seconds and milliseconds
    /// ave been added to (or substracted from) the current ones. In addition, this method sets
    /// the out argument to the overflow in days, if any.
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

    /// <summary>
    /// Returns a new instance where the given number of hours, minutes, seconds and milliseconds
    /// ave been added to (or substracted from) the current ones.
    /// </summary>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    /// <param name="seconds"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public ClockTime Add(
        int hours, int minutes = 0, int seconds = 0, int milliseconds = 0)
        => Add(out _, hours, minutes, seconds, milliseconds);

    // ----------------------------------------------------

    /// <inheritdoc cref="IComparable.CompareTo(object?)"/>
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

    /// <inheritdoc/>
    public int CompareTo(ClockTime? other) => Compare(this, other);

    public static bool operator >(ClockTime? x, ClockTime? y) => Compare(x, y) > 0;
    public static bool operator <(ClockTime? x, ClockTime? y) => Compare(x, y) < 0;
    public static bool operator >=(ClockTime? x, ClockTime? y) => Compare(x, y) >= 0;
    public static bool operator <=(ClockTime? x, ClockTime? y) => Compare(x, y) <= 0;
}