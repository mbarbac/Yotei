namespace Yotei.Tools.Miscelanea;

// ========================================================
/// <summary>
/// Represent an arbitrary BC or AC date.
/// <br/> Instances of this type take into consideration the XVI century conversion from the
/// Julian calendar system to the Gregorian one.
/// </summary>
public sealed record DayDate : IComparable<DayDate>
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
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public DayDate(DayDate source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Year = source.Year;
        Month = source.Month;
        Day = source.Day;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Year:0000}-{Month:00}-{Day:00}";

    /// <summary>
    /// Returns a string representation of this instance using the given format:
    /// 'y|yy|yyyy', 'M|MM|MMM|MMMM' and 'd|dd|ddd|dddd' case sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string ToString([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format)
    {
        format = format.NotNullNotEmpty(trim: false);
        return InRange ? ((DateOnly)this).ToString(format) : ToStringInternal(format);
    }

    /// <summary>
    /// Returns a string representation of this instance using the rules of the given culture.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(IFormatProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        var format = DateTimeFormatInfo.GetInstance(provider).ShortDatePattern;
        return InRange ? ((DateOnly)this).ToString(format) : ToStringInternal(format);
    }

    /// <summary>
    /// Returns a string representation of this instance using the rules of the given culture
    /// and using the given format: 'y|yy|yyyy', 'M|MM|MMM|MMMM' and 'd|dd|ddd|dddd' case
    /// sensitive specifications.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        IFormatProvider provider)
    {
        format = format.NotNullNotEmpty(trim: false);
        ArgumentNullException.ThrowIfNull(provider);
        return InRange ? ((DateOnly)this).ToString(format, provider) : ToStringInternal(format);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to process the given format: 'yyyy mmmm dddd'
    /// </summary>
    string ToStringInternal(string? format)
    {
        if (format is null || format.Length == 0) return ToString();

        var fake = new DateTime(2000, Month, Day); // 2000 was a leap year!
        var sb = new StringBuilder();
        for (int i = 0; i < format.Length; i++)
        {
            var span = format.AsSpan(i);

            if (span.StartsWith("yyyy")) { sb.Append($"{Year:0000}"); i += 4; i--; continue; }
            if (span.StartsWith("yy")) { sb.Append($"{Year:00}"); i += 2; i--; continue; }
            if (span.StartsWith("y")) { sb.Append($"{Year:0}"); i += 1; i--; continue; }

            if (span.StartsWith("MMMM")) { sb.Append(fake.ToString("MMMM")); i += 4; i--; continue; }
            if (span.StartsWith("MMM")) { sb.Append(fake.ToString("MMM")); i += 3; i--; continue; }
            if (span.StartsWith("MM")) { sb.Append($"{Month:00}"); i += 2; i--; continue; }
            if (span.StartsWith("M")) { sb.Append($"{Month:0}"); i += 1; i--; continue; }

            if (span.StartsWith("dddd")) { sb.Append(fake.ToString("dddd")); i += 4; i--; continue; }
            if (span.StartsWith("ddd")) { sb.Append(fake.ToString("ddd")); i += 3; i--; continue; }
            if (span.StartsWith("dd")) { sb.Append($"{Day:00}"); i += 2; i--; continue; }
            if (span.StartsWith("d")) { sb.Append($"{Day:0}"); i += 1; i--; continue; }

            sb.Append(format[i]);
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with its time part set to cero.
    /// </summary>
    /// <returns></returns>
    public DateTime ToDateTime() => new(Year, Month, Day);

    /// <summary>
    /// Returns a new instance with the given time part.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public DateTime ToDateTime(TimeOnly value) => new(
        Year, Month, Day, 
        value.Hour, value.Minute, value.Second, value.Millisecond, value.Microsecond);

    /// <summary>
    /// Conversion operator. The time part of the returned instance is set to cero.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DateTime(DayDate value) => new(value.Year, value.Month, value.Day);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DayDate(DateTime value) => new(value.Year, value.Month, value.Day);

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DateOnly(DayDate value) => new(value.Year, value.Month, value.Day);

    // ----------------------------------------------------

    /// <summary>
    /// The year, which can either be a negative (BC) or positive (AC) value, but not cero.
    /// </summary>
    public int Year
    {
        get;
        init => field = value != 0 ? value :
            throw new ArgumentException("Year cannot be cero.").WithData(value);
    } = 1;

    /// <summary>
    /// The month, from 1 to 12.
    /// </summary>
    public int Month
    {
        get;
        init => field = (value is >= 1 and <= 12) ? value :
            throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    } = 1;

    /// <summary>
    /// The day, from 1 to [28,29,30,31], depending upong the month and year.
    /// </summary>
    public int Day
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);

            var max = DaysInMonthInternal(Year, Month);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, max);

            field = value;
        }
    }

    /// <summary>
    /// Gets a new instance with the current date in local coordinates.
    /// </summary>
    public static DayDate Now => DateTime.Now;

    /// <summary>
    /// Gets a new instance with the current date in global coordinates.
    /// </summary>
    public static DayDate UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Determines if this instance is within the range of the standard values.
    /// </summary>
    bool InRange => Year >= DateOnly.MinValue.Year && Year <= DateOnly.MaxValue.Year;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given year is a leap one.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public static bool IsLeapYear(int year)
    {
        if (year == 0) throw new ArgumentException("Year cannot be cero.");

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
    /// Returns the number of days in the given month of the given years, taking in consideration
    /// if it is a leap one or not.
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
            _ => IsLeapYear(year) ? 29 : 28,
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IComparable.CompareTo(object?)"/>
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(DayDate? other) => Compare(this, other);

    public static bool operator >(DayDate? x, DayDate? y) => Compare(x, y) > 0;
    public static bool operator <(DayDate? x, DayDate? y) => Compare(x, y) < 0;
    public static bool operator >=(DayDate? x, DayDate? y) => Compare(x, y) >= 0;
    public static bool operator <=(DayDate? x, DayDate? y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given number of days have been added to (or substracted
    /// from) the current value.
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
}