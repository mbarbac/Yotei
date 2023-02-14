namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an arbitrary date in a gregorian calendar format.
/// </summary>
public class DayDate : ICloneable, IComparable<DayDate>, IEquatable<DayDate>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    public DayDate(int year, int month, int day)
    {
        ValidateDay(year, month, day);

        Year = year;
        Month = month;
        Day = day;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="item"></param>
    public DayDate(DateTime item) : this(item.Year, item.Month, item.Day) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="item"></param>
    public DayDate(DateOnly item) : this(item.Year, item.Month, item.Day) { }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"{Year:0000}-{Month:00}-{Day:00}";

    /// <summary>
    /// Returns the string representation of this instance.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(CultureInfo info)
    {
        info = info.ThrowIfNull();

        var pattern = info.DateTimeFormat.ShortDatePattern;
        var separator = info.DateTimeFormat.DateSeparator;

        return pattern.StartsWith("m", StringComparison.OrdinalIgnoreCase)
           ? $"{Month:00}{separator}{Day:00}{separator}{Year:0000}"
           : $"{Day:00}{separator}{Month:00}{separator}{Year:0000}";
    }

    /// <summary>
    /// Returns the string representation of this instance.
    /// <para>Valid specifications are: YYYY, YY, MM, M, DD, and D.</para>
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(string format)
    {
        format = format.ThrowIfNull();

        ArgumentNullException.ThrowIfNull(format);

        var comparison = StringComparison.OrdinalIgnoreCase;
        int pos;

        pos = format.IndexOf("YYYY", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 4);
            format = format.Insert(pos, Year.ToString());
        }

        pos = format.IndexOf("YY", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, Year.ToString()[^2..]);
        }

        pos = format.IndexOf("MM", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, $"{Month:00}");
        }

        pos = format.IndexOf("M", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Month.ToString());
        }

        pos = format.IndexOf("DD", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 2);
            format = format.Insert(pos, $"{Day:00}");
        }

        pos = format.IndexOf("D", comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Day.ToString());
        }

        return format;
    }

    /// <inheritdoc cref="ICloneable.Clone">
    /// </inheritdoc>
    public DayDate Clone() => new(Year, Month, Day);

    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Returns a new instance with the original value of the 'Year' property replaced by the
    /// new one.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public DayDate WithYear(int year) => new(year, Month, Day);

    /// <summary>
    /// Returns a new instance with the original value of the 'Month' property replaced by the
    /// new one.
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public DayDate WithMonth(int month) => new(Year, month, Day);

    /// <summary>
    /// Returns a new instance with the original value of the 'Day' property replaced by the
    /// new one.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public DayDate WithDay(int day) => new(Year, Month, day);

    /// <summary>
    /// Returns a new instance based upon the values on this one.
    /// </summary>
    /// <returns></returns>
    public DateTime ToDateTime() => new(Year, Month, Day);

    /// <summary>
    /// Returns a new instance based upon the values on this one.
    /// </summary>
    /// <returns></returns>
    public DateOnly ToDateOnly() => new(Year, Month, Day);

    /// <summary>
    /// Compares this instance and returns an integer that indicates their relative position
    /// in sort order.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(DayDate? other) => Compare(this, other);

    public static bool operator ==(DayDate? x, DayDate? y) => Compare(x, y) == 0;

    public static bool operator !=(DayDate? x, DayDate? y) => Compare(x, y) != 0;

    public static bool operator >(DayDate? x, DayDate? y) => Compare(x, y) > 0;

    public static bool operator <(DayDate? x, DayDate? y) => Compare(x, y) < 0;

    public static bool operator >=(DayDate? x, DayDate? y) => Compare(x, y) >= 0;

    public static bool operator <=(DayDate? x, DayDate? y) => Compare(x, y) <= 0;

    /// <inheritdoc>
    /// </inheritdoc>
    public bool Equals(DayDate? other) => Compare(this, other) == 0;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;

        return obj is DayDate item && Compare(this, item) == 0;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override int GetHashCode() => HashCode.Combine(Year, Month, Day);

    /// <summary>
    /// Adds the given number of days to this instance returning a new one with the result.
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    public DayDate Add(int days)
    {
        int year = Year;
        int month = Month;
        int day = Day;

        while (days < 0) // Decreasing...
        {
            days++;

            if ((--day) < 1)
            {
                if ((--month) < 1)
                {
                    if ((--year) == 0) year = -1;
                    month = 12;
                }
                day = DaysInMonth(year, month);
            }
            if (year == 1582 && month == 10 && day == 14) day = 4;
        }

        while (days > 0) // Increasing...
        {
            days--;

            int max = DaysInMonth(year, month);
            if ((++day) > max)
            {
                if ((++month) > 12)
                {
                    if ((++year) == 0) year = 1;
                    month = 1;
                }
                day = 1;
            }
            if (year == 1582 && month == 10 && day == 5) day = 15;
        }

        return new(year, month, day);
    }

    /// <summary>
    /// The year value, which can be a negative (BC) or positive (AC) value, but not cero.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// The month value, which can be from 1 to 12.
    /// </summary>
    public int Month { get; }

    /// <summary>
    /// The day value, which can be 28, 29, 30 or 31, depending upon the year and month.
    /// </summary>
    public int Day { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Validates the year.
    /// </summary>
    static void ValidateYear(int year)
    {
        if (year == 0) throw new ArgumentException("Year cannot be cero.");
    }

    /// <summary>
    /// Validates the month.
    /// </summary>
    static void ValidateMonth(int month)
    {
        if (month < 1) throw new ArgumentException("Month cannot be less than cero.").WithData(month);
        if (month > 12) throw new ArgumentException("Month cannot be bigger than 12.").WithData(month);
    }

    /// <summary>
    /// Validates the day, based upon the given year and month.
    /// </summary>
    static void ValidateDay(int year, int month, int day)
    {
        ValidateYear(year);
        ValidateMonth(month);
        if (day < 1) throw new ArgumentException("Day cannot be less than cero.").WithData(day);

        var max = DaysInMonth(year, month);
        if (day > max) throw new ArgumentException(
            "Day number too big for the given month and year.")
            .WithData(year)
            .WithData(month)
            .WithData(day)
            .WithData(max);
    }

    /// <summary>
    /// Returns the number of days in the given month, taking into consideration if the given
    /// year is a leap one, or not.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    public static int DaysInMonth(int year, int month)
    {
        ValidateYear(year);
        ValidateMonth(month);

        return month switch
        {
            1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
            4 or 6 or 9 or 11 => 30,
            _ => IsLeapYear(year) ? 29 : 28,
        };
    }

    /// <summary>
    /// Returns whether the given year is a leap one, or not.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool IsLeapYear(int year)
    {
        ValidateYear(year);

        if (year >= 1582) // Gregorian calendar...
        {
            if ((year % 4) == 0) // The 4 years rule...
            {
                if ((year % 400) == 0) return true; // The 400 years rule...
                if ((year % 100) == 0) return false; // The 100 years rule...
                return true;
            }
        }

        if (year >= -45) // Julian calendar...
        {
            if (year <= -12 && (year % 3) == 0) // An error made by ancient astronomers...
                return true;

            if ((year % 4) == 0) // The 4 years rule...
            {
                if ((year % 100) == 0) return false; // The 100 years rule...
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Compares the given instances and returns an integer that indicates their relative
    /// position in sort order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(DayDate? x, DayDate? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;

        if (x.Year != y.Year) return x.Year < y.Year ? -1 : +1;
        if (x.Month != y.Month) return x.Month < y.Month ? -1 : +1;
        if (x.Day != y.Day) return x.Day < y.Day ? -1 : +1;

        return 0;
    }

    /// <summary>
    /// Returns a new instance that represents the current date, in local coordinates.
    /// </summary>
    /// <returns></returns>
    public static DayDate Now => new(DateTime.Now);

    /// <summary>
    /// Returns a new instance that represents the current date, in UTC coordinates.
    /// </summary>
    /// <returns></returns>
    public static DayDate NowUTC => new(DateTime.UtcNow);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator DateTime(DayDate source)
    {
        source = source.ThrowIfNull();
        return source.ToDateTime();
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator DayDate(DateTime item) => new(item);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator DateOnly(DayDate source)
    {
        source = source.ThrowIfNull();
        return source.ToDateOnly();
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator DayDate(DateOnly item) => new(item);
}