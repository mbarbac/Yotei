namespace Yotei.Tools.Miscelanea;

// ========================================================
/// <summary>
/// Represents an arbitrary time moment in a 24-hours clock format.
/// </summary>
public record class ClockTime
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
    /// Conversion operator.
    /// </summary>
    public static implicit operator ClockTime(DateTime item)
        => new(item.Hour, item.Minute, item.Second, item.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator ClockTime(TimeSpan item)
        => new(item.Hours, item.Minutes, item.Seconds, item.Milliseconds);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator TimeSpan(ClockTime item)
        => new(0, item.Hour, item.Minute, item.Second, item.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator ClockTime(TimeOnly item)
        => new(item.Hour, item.Minute, item.Second, item.Millisecond);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator TimeOnly(ClockTime item)
        => new(item.Hour, item.Minute, item.Second, item.Millisecond, 0);

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
    /// 'hh', 'mm', 'ss' and 'fff' case sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string? format)
        => ((TimeOnly)this).ToString(format);

    /// <summary>
    /// Returns a string representation of this instance using the given culture information.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(IFormatProvider? provider) => ((TimeOnly)this).ToString(provider);

    /// <summary>
    /// Returns a string representation of this instance using the given format and culture
    /// information. Format uses the 'hh', 'mm', 'ss' and 'fff' specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string? format,
        IFormatProvider? provider)
        => ((TimeOnly)this).ToString(format, provider);

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
    int _Hour = 0;

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
    int _Minute = 0;

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
    int _Second = 0;

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
    int _Millisecond = 0;
}