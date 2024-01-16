namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an immutable collection of culture sensitive settings and methods.
/// </summary>
public record class Locale : IComparer<string>, IEqualityComparer<char>
{
    /// <summary>
    /// Initializes a new instance using the culture of the current thread, and default string
    /// comparison options.
    /// </summary>
    public Locale() { }

    /// <summary>
    /// Initializes a new instance with the given culture, and default string comparison options.
    /// </summary>
    /// <param name="culture"></param>
    public Locale(CultureInfo culture) => CultureInfo = culture;

    /// <summary>
    /// Initializes a new instance with the culture of the current thread, and the given string
    /// comparison options.
    /// </summary>
    /// <param name="options"></param>
    public Locale(CompareOptions options) => CompareOptions = options;

    /// <summary>
    /// Initializes a new instance with the given culture and the given string comparison options.
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="options"></param>
    public Locale(CultureInfo culture, CompareOptions options)
    {
        CultureInfo = culture;
        CompareOptions = options;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(CultureInfo.Name.NullWhenEmpty() ?? "Invariant");
        if (CompareOptions.HasFlag(CompareOptions.StringSort)) sb.Append(", StringSort");
        if (CompareOptions.HasFlag(CompareOptions.Ordinal)) sb.Append(", Ordinal");
        if (CompareOptions.HasFlag(CompareOptions.IgnoreCase)) sb.Append(", IgnoreCase");
        if (CompareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase)) sb.Append(", OrdinalIgnoreCase");
        if (CompareOptions.HasFlag(CompareOptions.IgnoreNonSpace)) sb.Append(", IgnoreNonSpace");
        if (CompareOptions.HasFlag(CompareOptions.IgnoreSymbols)) sb.Append(", IgnoreSymbols");
        if (CompareOptions.HasFlag(CompareOptions.IgnoreKanaType)) sb.Append(", IgnoreKanaType");
        if (CompareOptions.HasFlag(CompareOptions.IgnoreWidth)) sb.Append(", IgnoreWidth");

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The read-only culture used by this instance.
    /// </summary>
    public CultureInfo CultureInfo
    {
        get => _CultureInfo;
        init => _CultureInfo = CultureInfo.ReadOnly(value.ThrowWhenNull());
    }
    CultureInfo _CultureInfo = CultureInfo.CurrentCulture;

    /// <summary>
    /// The string comparison options used by this instance.
    /// </summary>
    public CompareOptions CompareOptions
    {
        get => _CompareOptions;
        init => _CompareOptions = value;
    }
    CompareOptions _CompareOptions = CompareOptions.None;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> 
    /// Zero- if the given values are equal, Less than zero- if the left one is less than the
    /// right one, Greater than zero- if the right one is greater than the left one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(string? x, string? y)
    {
        return CultureInfo.CompareInfo.Compare(x, y, CompareOptions);
    }

    /// <summary>
    /// Compares the two chars returns a value indicating their lexical relationship:
    /// Zero- if they are equal, Less than zero- if the left one is less than the right one,
    /// Greater than zero- if the right one is greater than the left one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(char x, char y)
    {
        var xs = x.ToString();
        var ys = y.ToString();

        return Compare(xs, ys);
    }

    /// <summary>
    /// Compares the two char spans and returns a value indicating their lexical relationship:
    /// Zero- if they are equal, Less than zero- if the left one is less than the right one,
    /// Greater than zero- if the right one is greater than the left one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return CultureInfo.CompareInfo.Compare(x, y, CompareOptions);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(char x, char y) => Compare(x, y) == 0;

    /// <inheritdoc/>
    public int GetHashCode(char obj) => obj.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.IsPrefix(string, string)"/>
    public bool HasPrefix(string source, string prefix)
    {
        return CultureInfo.CompareInfo.IsPrefix(source, prefix, CompareOptions);
    }

    /// <inheritdoc cref="HasPrefix(string, string)"/>
    public bool HasPrefix(ReadOnlySpan<char> source, ReadOnlySpan<char> prefix)
    {
        return CultureInfo.CompareInfo.IsPrefix(source, prefix, CompareOptions);
    }

    /// <inheritdoc cref="CompareInfo.IsSuffix(string, string)"/>
    public bool HasSuffix(string source, string suffix)
    {
        return CultureInfo.CompareInfo.IsSuffix(source, suffix, CompareOptions);
    }

    /// <inheritdoc cref="HasSuffix(string, string)"/>
    public bool HasSuffix(ReadOnlySpan<char> source, ReadOnlySpan<char> suffix)
    {
        return CultureInfo.CompareInfo.IsSuffix(source, suffix, CompareOptions);
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.IndexOf(string, string)"/>
    public int IndexOf(string source, string value)
    {
        return CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);
    }

    /// <inheritdoc cref="CompareInfo.IndexOf(string, char)"/>
    public int IndexOf(string source, char value)
    {
        return CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);
    }

    /// <inheritdoc cref="IndexOf(string, string)"/>
    public int IndexOf(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
    {
        return CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);
    }

    /// <inheritdoc cref="IndexOf(string, char)"/>
    public int IndexOf(ReadOnlySpan<char> source, char value)
    {
        return CultureInfo.CompareInfo.IndexOf(source, value.ToString(), CompareOptions);
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, string)"/>
    public int LastIndexOf(string source, string value)
    {
        return CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);
    }

    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, char)"/>
    public int LastIndexOf(string source, char value)
    {
        return CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);
    }

    /// <inheritdoc cref="LastIndexOf(string, string)"/>
    public int LastIndexOf(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
    {
        return CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);
    }

    /// <inheritdoc cref="LastIndexOf(string, char)"/>
    public int LastIndexOf(ReadOnlySpan<char> source, char value)
    {
        return CultureInfo.CompareInfo.LastIndexOf(source, value.ToString(), CompareOptions);
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="TextInfo.ToUpper(string)"/>
    public string ToUpper(string value)
    {
        return CultureInfo.TextInfo.ToUpper(value);
    }

    /// <inheritdoc cref="TextInfo.ToUpper(char)"/>
    public char ToUpper(char value)
    {
        return CultureInfo.TextInfo.ToUpper(value);
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="TextInfo.ToLower(string)"/>
    public string ToLower(string value)
    {
        return CultureInfo.TextInfo.ToLower(value);
    }

    /// <inheritdoc cref="TextInfo.ToLower(char)"/>
    public char ToLower(char value)
    {
        return CultureInfo.TextInfo.ToLower(value);
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="TextInfo.ToTitleCase(string)"/>
    public string ToTitleCase(string value)
    {
        return CultureInfo.TextInfo.ToTitleCase(value);
    }
}