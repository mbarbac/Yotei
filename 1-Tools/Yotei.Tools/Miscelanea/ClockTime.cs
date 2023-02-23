namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an arbitrary moment in time, in a 24-hours clock format.
/// </summary>
public class ClockTime : ICloneable, IComparable<ClockTime>, IEquatable<ClockTime>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="millisecond"></param>
    public ClockTime(int hour, int minute, int second = 0, int millisecond = 0)
    {
        ValidateHour(hour);
        ValidateMinute(minute);
        ValidateSecond(second);
        ValidateMillisecond(millisecond);

        Hour = hour;
        Minute = minute;
        Second = second;
        Millisecond = millisecond;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="item"></param>
    public ClockTime(DateTime item) : this(item.Hour, item.Minute, item.Second, item.Millisecond) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="item"></param>
    public ClockTime(TimeOnly item) : this(item.Hour, item.Minute, item.Second, item.Millisecond) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="item"></param>
    public ClockTime(TimeSpan item) : this(item.Hours, item.Minutes, item.Seconds, item.Milliseconds) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => ToStringSeparated(":");

    string ToStringSeparated(string separator)
    {
        var str = $"{Hour:00}{separator}{Minute:00}";

        if (Second != 0) str += $"{separator}{Second:00}";
        if (Millisecond != 0) str += $".{Millisecond:000}";

        return str;
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(CultureInfo info)
    {
        info = info.ThrowIfNull();

        var separator = info.DateTimeFormat.TimeSeparator;
        return ToStringSeparated(separator);
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// <para>Valid specifications are: HH, H, MM, M, SS, S, and NNN or N.</para>
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(string format)
    {
        format = format.ThrowIfNull();

        var comparison = StringComparison.OrdinalIgnoreCase;
        int pos;

        pos = format.IndexOf("HH", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, $"{Hour:00}");
        }

        pos = format.IndexOf("H", comparison);
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

        pos = format.IndexOf("M", comparison);
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

        pos = format.IndexOf("S", comparison);
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

        pos = format.IndexOf("N", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Millisecond.ToString());
        }

        return format;
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public ClockTime Clone() => new(Hour, Minute, Second, Millisecond);

    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Returns a new instance with the original value of the 'Hour' property replaced by the
    /// new one.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public ClockTime WithHour(int hour) => new(hour, Minute, Second, Millisecond);

    /// <summary>
    /// Returns a new instance with the original value of the 'Minute' property replaced by the
    /// new one.
    /// </summary>
    /// <param name="minute"></param>
    /// <returns></returns>
    public ClockTime WithMinute(int minute) => new(Hour, minute, Second, Millisecond);

    /// <summary>
    /// Returns a new instance with the original value of the 'Second' property replaced by the
    /// new one.
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public ClockTime WithSecond(int second) => new(Hour, Minute, second, Millisecond);

    /// <summary>
    /// Returns a new instance with the original value of the 'Millisecond' property replaced
    /// by the new one.
    /// </summary>
    /// <param name="millisecond"></param>
    /// <returns></returns>
    public ClockTime WithMillisecond(int millisecond) => new(Hour, Minute, Second, millisecond);

    /// <summary>
    /// Returns a new instance based upon the values on this one.
    /// </summary>
    /// <returns></returns>
    public TimeSpan ToTimeSpan() => new(0, Hour, Minute, Second, Millisecond);

    /// <summary>
    /// Returns a new instance based upon the values on this one.
    /// </summary>
    /// <returns></returns>
    public TimeOnly ToTimeOnly() => new(Hour, Minute, Second, Millisecond);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int CompareTo(ClockTime? other) => Compare(this, other);

    public static bool operator ==(ClockTime? x, ClockTime? y) => Compare(x, y) == 0;

    public static bool operator !=(ClockTime? x, ClockTime? y) => Compare(x, y) != 0;

    public static bool operator >(ClockTime? x, ClockTime? y) => Compare(x, y) > 0;

    public static bool operator <(ClockTime? x, ClockTime? y) => Compare(x, y) < 0;

    public static bool operator >=(ClockTime? x, ClockTime? y) => Compare(x, y) >= 0;

    public static bool operator <=(ClockTime? x, ClockTime? y) => Compare(x, y) <= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool Equals(ClockTime? other) => Compare(this, other) == 0;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;

        return obj is ClockTime item && Compare(this, item) == 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode() => HashCode.Combine(Hour, Minute, Second, Millisecond);

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
         int hours, int minutes, int seconds = 0, int milliseconds = 0)
    {
        days = 0;

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
         int hours, int minutes, int seconds = 0, int milliseconds = 0)
         => Add(out var _, hours, minutes, seconds, milliseconds);

    /// <summary>
    /// The hours' value, from 0 to 23.
    /// </summary>
    public int Hour { get; }

    /// <summary>
    /// The minutes' value, which can be from 0 to 59.
    /// </summary>
    public int Minute { get; }

    /// <summary>
    /// The seconds' value, which can be from 0 to 59.
    /// </summary>
    public int Second { get; }

    /// <summary>
    /// The milliseconds' value, which can be from 0 to 999.
    /// </summary>
    public int Millisecond { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Validates the hour.
    /// </summary>
    static void ValidateHour(int hour)
    {
        if (hour < 0) throw new ArgumentException("Hour cannot be less than cero.").WithData(hour);
        if (hour > 23) throw new ArgumentException("Hour cannot be bigger than 23.").WithData(hour);
    }

    /// <summary>
    /// Validates the minute.
    /// </summary>
    static void ValidateMinute(int minute)
    {
        if (minute < 0) throw new ArgumentException("Minute cannot be less than cero.").WithData(minute);
        if (minute > 59) throw new ArgumentException("Minute cannot be bigger than 59.").WithData(minute);
    }

    /// <summary>
    /// Validates the second.
    /// </summary>
    static void ValidateSecond(int second)
    {
        if (second < 0) throw new ArgumentException("Second cannot be less than cero.").WithData(second);
        if (second > 59) throw new ArgumentException("Second cannot be bigger than 59.").WithData(second);
    }

    /// <summary>
    /// Validates the second.
    /// </summary>
    static void ValidateMillisecond(int millisecond)
    {
        if (millisecond < 0) throw new ArgumentException("Millisecond cannot be less than cero.").WithData(millisecond);
        if (millisecond > 999) throw new ArgumentException("Millisecond cannot be bigger than 999.").WithData(millisecond);
    }

    /// <summary>
    /// Compares the given instances and returns an integer that indicates their relative
    /// position in sort order.
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
    /// Conversion operator.
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator TimeOnly(ClockTime source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.ToTimeOnly();
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator ClockTime(TimeOnly item) => new(item);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator TimeSpan(ClockTime source)
    {
        source = source.ThrowIfNull();
        return source.ToTimeSpan();
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator ClockTime(TimeSpan item) => new(item);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator ClockTime(DateTime item) => new(item);
}