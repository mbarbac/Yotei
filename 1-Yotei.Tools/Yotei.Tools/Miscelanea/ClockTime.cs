namespace Yotei.Tools.Miscelanea;

// ========================================================
/// <summary>
/// Represents an arbitrary moment in a day, in a 24-hours clock format.
/// </summary>
public sealed record class ClockTime : IComparable<ClockTime>, IEquatable<ClockTime>
{
    /// <summary>
    /// Initializes a new instance that refers to the initial moment in the day.
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
    /// Copy constructor
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
    /// Initializes a new instance with the values from the time portion of given instance.
    /// </summary>
    /// <param name="item"></param>
    public ClockTime(DateTime item) : this(item.Hour, item.Minute, item.Second, item.Millisecond) { }

    /// <summary>
    /// Initializes a new instance with the values from the given instance.
    /// </summary>
    /// <param name="item"></param>
    public ClockTime(TimeSpan item) : this(item.Hours, item.Minutes, item.Seconds, item.Milliseconds) { }

    /// <summary>
    /// The hours' value, from 0 to 23.
    /// </summary>
    public int Hour
    {
        get => _Hour;
        init
        {
            if (value < 0) throw new ArgumentException("Hour cannot be less than cero.").WithData(value);
            if (value > 23) throw new ArgumentException("Hour cannot be bigger than 23.").WithData(value);
            _Hour = value;
        }
    }
    int _Hour = 0;

    /// <summary>
    /// The minutes' value, which can be from 0 to 59.
    /// </summary>
    public int Minute
    {
        get => _Minute;
        init
        {
            if (value < 0) throw new ArgumentException("Minute cannot be less than cero.").WithData(value);
            if (value > 59) throw new ArgumentException("Minute cannot be bigger than 59.").WithData(value);
            _Minute = value;
        }
    }
    int _Minute = 0;

    /// <summary>
    /// The seconds' value, which can be from 0 to 59.
    /// </summary>
    public int Second
    {
        get => _Second;
        init
        {
            if (value < 0) throw new ArgumentException("Second cannot be less than cero.").WithData(value);
            if (value > 59) throw new ArgumentException("Second cannot be bigger than 59.").WithData(value);
            _Second = value;
        }
    }
    int _Second = 0;

    /// <summary>
    /// The milliseconds' value, which can be from 0 to 999.
    /// </summary>
    public int Millisecond
    {
        get => _Millisecond;
        init
        {
            if (value < 0) throw new ArgumentException("Millisecond cannot be less than cero.").WithData(value);
            if (value > 999) throw new ArgumentException("Millisecond cannot be bigger than 999.").WithData(value);
            _Millisecond = value;
        }
    }
    int _Millisecond = 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToStringSeparated(":");

    string ToStringSeparated(string separator)
    {
        var str = $"{Hour:00}{separator}{Minute:00}";

        if (Second != 0) str += $"{separator}{Second:00}";

        if (Millisecond != 0)
        {
            if (Second == 0) str += $"{separator}:00";
            str += $".{Millisecond:000}";
        }

        return str;
    }

    /// <summary>
    /// Returns a string that represents this instance using the given culture information.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public string ToString(CultureInfo info)
    {
        info = info.ThrowWhenNull();

        var separator = info.DateTimeFormat.TimeSeparator;
        return ToStringSeparated(separator);
    }

    /// <summary>
    /// Returns a string that represents this instance using the given format specification,
    /// whose valid ones are: HH, H, MM, M, SS, S, and NNN or N.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(string format)
    {
        format = format.ThrowWhenNull();

        var comparison = StringComparison.OrdinalIgnoreCase;
        int pos;

        pos = format.IndexOf("HH", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, $"{Hour:00}");
        }

        pos = format.IndexOf('H', comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Hour.ToString());
        }

        pos = format.IndexOf("MM", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, $"{Minute:00}");
        }

        pos = format.IndexOf('M', comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Minute.ToString());
        }

        pos = format.IndexOf("SS", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, $"{Second:00}");
        }

        pos = format.IndexOf('S', comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Second.ToString());
        }

        pos = format.IndexOf("NNN", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 3);
            format = format.Insert(pos, $"{Millisecond:000}");
        }

        pos = format.IndexOf('N', comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Millisecond.ToString());
        }

        return format;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that represents the current date, in local coordinates.
    /// </summary>
    /// <returns></returns>
    public static ClockTime Now => new(DateTime.Now);

    /// <summary>
    /// Returns a new instance that represents the current date, in UTC coordinates.
    /// </summary>
    /// <returns></returns>
    public static ClockTime NowUTC => new(DateTime.UtcNow);

    /// <summary>
    /// Returns a new instance based upon the values on this one.
    /// </summary>
    /// <returns></returns>
    public TimeSpan ToTimeSpan() => new(0, Hour, Minute, Second, Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator ClockTime(DateTime item) => new(item);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator ClockTime(TimeSpan item) => new(item);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator TimeSpan(ClockTime source)
    {
        source = source.ThrowWhenNull();
        return source.ToTimeSpan();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given number of hours, minutes, seconds and milliseconds to this instance
    /// returning a new one with the result, and an out value with the overflow in days.
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
    /// Adds the given number of hours, minutes, seconds and milliseconds to this instance
    /// returning a new one with the result.
    /// </summary>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    /// <param name="seconds"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public ClockTime Add(
         int hours, int minutes = 0, int seconds = 0, int milliseconds = 0)
         => Add(out var _, hours, minutes, seconds, milliseconds);

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position in sort order as the second one.
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ClockTime? other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => HashCode.Combine(Hour, Minute, Second, Millisecond);
}