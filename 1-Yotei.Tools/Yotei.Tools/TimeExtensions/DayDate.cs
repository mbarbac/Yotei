namespace Yotei.Tools.TimeExtensions;

// ========================================================
/// <summary>
/// Represents an arbitrary BC or AC date in a Gregorian calendar format, that takes into
/// consideration the XVI century conversion from the Julian to the Gregorian one.
/// </summary>
public sealed record class DayDate : IComparable<DayDate>, IEquatable<DayDate>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    public DayDate(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    public DayDate(DayDate source)
    {
        source = source.ThrowWhenNull();

        _Year = source.Year;
        _Month = source.Month;
        _Day = source.Day;
    }

    /// <summary>
    /// Initializes a new instance with the values from the date portion of the given instance.
    /// </summary>
    /// <param name="item"></param>
    public DayDate(DateTime item) : this(item.Year, item.Month, item.Day) { }

    /// <summary>
    /// The year value, which can be a negative (BC) or positive (AC) value, but not cero.
    /// </summary>
    public int Year
    {
        get => _Year;
        init
        {
            if (value == 0) throw new ArgumentException("Year cannot be cero.");
            _Year = value;
        }
    }
    int _Year = 0;

    /// <summary>
    /// The month value, which can be from 1 to 12.
    /// </summary>
    public int Month
    {
        get => _Month;
        init
        {
            if (value < 1) throw new ArgumentException("Month cannot be less than 1.").WithData(value);
            if (value > 12) throw new ArgumentException("Month cannot be bigger than 12.").WithData(value);
            _Month = value;
        }
    }
    int _Month = 0;

    /// <summary>
    /// The day value, which can be 28, 29, 30 or 31, depending upon the year and month.
    /// </summary>
    public int Day
    {
        get => _Day;
        init
        {
            if (value < 1) throw new ArgumentException("Day cannot be less than 1.").WithData(value);

            var max = DaysInMonthInternal(Year, Month);
            if (value > max) throw new ArgumentException(
                "Day exceeds the max number of days in the given year/month combination.")
                .WithData(Year)
                .WithData(Month)
                .WithData(max)
                .WithData(value);

            _Day = value;
        }
    }
    int _Day = 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string ToString() => $"{Year:0000}-{Month:00}-{Day:00}";

    /// <summary>
    /// Returns a string that represents this instance using the given culture information.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public string ToString(CultureInfo info)
    {
        info = info.ThrowWhenNull();

        var pattern = info.DateTimeFormat.ShortDatePattern;
        var separator = info.DateTimeFormat.DateSeparator;

        return pattern.StartsWith("m", StringComparison.OrdinalIgnoreCase)
           ? $"{Month:00}{separator}{Day:00}{separator}{Year:0000}"
           : $"{Day:00}{separator}{Month:00}{separator}{Year:0000}";
    }

    /// <summary>
    /// Returns a string that represents this instance using the given format specification,
    /// whose valid ones are: YYYY, YY, MM, M, DD, and D.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString(string format)
    {
        format = format.ThrowWhenNull();

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

        pos = format.IndexOf('M', comparison);
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

        pos = format.IndexOf('D', comparison);
        if (pos >= 0)
        {
            format = format.Remove(pos, 1);
            format = format.Insert(pos, Day.ToString());
        }

        return format;
    }

    // ----------------------------------------------------

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
    /// Returns a new instance based upon the values on this one.
    /// </summary>
    /// <returns></returns>
    public DateTime ToDateTime() => new(Year, Month, Day);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator DateTime(DayDate source)
    {
        source = source.ThrowWhenNull();
        return source.ToDateTime();
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator DayDate(DateTime item) => new(item);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the number of days in the given month, taking into consideration if the given
    /// year is a leap one or not.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    public static int DaysInMonth(int year, int month)
    {
        var temp = new DayDate(year, month, 1);
        month = temp.Month;

        return DaysInMonthInternal(year, month);
    }

    static int DaysInMonthInternal(int year, int month)
    {
        return month switch
        {
            1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
            4 or 6 or 9 or 11 => 30,
            _ => IsLeapYearInternal(year) ? 29 : 28,
        };
    }

    /// <summary>
    /// Returns whether the given year is a leap one or not, tacking into consideration if the
    /// year is before or after the XVI century conversion from the Julian calendar to the
    /// Gregorian one.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public static bool IsLeapYear(int year)
    {
        var temp = new DayDate(year, 1, 1);
        year = temp.Year;

        return IsLeapYearInternal(year);
    }

    static bool IsLeapYearInternal(int year)
    {
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
                return true;
            }
            return false;
        }

        return false;
    }

    /// <summary>
    /// Returns a new instance where the given number of days is added to the original one.
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    public DayDate Add(int days)
    {
        if (days == 0) return this;

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

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position in sort order as the second one.
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

    /// <inheritdoc/>
    public int CompareTo(DayDate? other) => Compare(this, other);

    public static bool operator >(DayDate? x, DayDate? y) => Compare(x, y) > 0;
    public static bool operator <(DayDate? x, DayDate? y) => Compare(x, y) < 0;
    public static bool operator >=(DayDate? x, DayDate? y) => Compare(x, y) >= 0;
    public static bool operator <=(DayDate? x, DayDate? y) => Compare(x, y) <= 0;

    /// <inheritdoc/>
    public bool Equals(DayDate? other) => Compare(this, other) == 0;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Year, Month, Day);
}