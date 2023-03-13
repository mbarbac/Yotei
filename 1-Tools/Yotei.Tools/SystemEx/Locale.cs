namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Helps working with locale settings.
/// </summary>
public record Locale : IComparer<string>
{
    /// <summary>
    /// A shared static default instance, that uses the invariant culture and default string
    /// comparison options.
    /// </summary>
    public static Locale Invariant { get; } = new Locale(false);
    private Locale(bool _) { }

    /// <summary>
    /// Initializes a new instance using the culture of the current thread, and default string
    /// comparison options.
    /// </summary>
    public Locale() : this(CultureInfo.CurrentCulture, CompareOptions.None) { }

    /// <summary>
    /// Initializes a new instance using the given culture and defauklt string comparison
    /// options.
    /// </summary>
    /// <param name="culture"></param>
    public Locale(CultureInfo culture) : this(culture, CompareOptions.None) { }

    /// <summary>
    /// Initializes a new instance using the culture of the current thread and the given string
    /// comparison options.
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="options"></param>
    public Locale(CompareOptions options) : this(CultureInfo.CurrentCulture, options) { }

    /// <summary>
    /// Initializes a new instance using the given culture and string comparison options.
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="options"></param>
    public Locale(CultureInfo culture, CompareOptions options)
    {
        CultureInfo = culture;
        CompareOptions = options;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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

    /// <summary>
    /// The read-only culture used by this instance.
    /// </summary>
    public CultureInfo CultureInfo
    {
        get => _CultureInfo;
        init => _CultureInfo = CultureInfo.ReadOnly(value.ThrowIfNull());
    }
    CultureInfo _CultureInfo = CultureInfo.InvariantCulture;

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
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int Compare(
        string? x, string? y)
        => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="Compare(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int Compare(
        ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="Compare(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int Compare(
        char x, char y)
        => CultureInfo.CompareInfo.Compare(x.ToString(), y.ToString(), CompareOptions);

    /// <summary>
    /// <inheritdoc cref="CompareInfo.IsPrefix(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool HasPrefix(
        string source, string prefix)
        => CultureInfo.CompareInfo.IsPrefix(source, prefix, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="HasPrefix(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool HasPrefix(
        ReadOnlySpan<char> source, ReadOnlySpan<char> prefix)
        => CultureInfo.CompareInfo.IsPrefix(source, prefix, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="CompareInfo.IsSuffix(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool HasSuffix(
        string source, string suffix)
        => CultureInfo.CompareInfo.IsSuffix(source, suffix, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="HasSuffix(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool HasSuffix(
        ReadOnlySpan<char> source, ReadOnlySpan<char> suffix)
        => CultureInfo.CompareInfo.IsSuffix(source, suffix, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="CompareInfo.IndexOf(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int IndexOf(
        string source, string value)
        => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="IndexOf(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int IndexOf(
        ReadOnlySpan<char> source, ReadOnlySpan<char> value)
        => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="CompareInfo.IndexOf(string, char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int IndexOf(
        string source, char value)
        => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="IndexOf(string, char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int IndexOf(
        ReadOnlySpan<char> source, char value)
        => CultureInfo.CompareInfo.IndexOf(source, value.ToString(), CompareOptions);

    /// <summary>
    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int LastIndexOf(
        string source, string value)
        => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="LastIndexOf(string, string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int LastIndexOf(
        ReadOnlySpan<char> source, ReadOnlySpan<char> value)
        => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int LastIndexOf(
        string source, char value)
        => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    /// <summary>
    /// <inheritdoc cref="LastIndexOf(string, char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public int LastIndexOf(
        ReadOnlySpan<char> source, char value)
       => CultureInfo.CompareInfo.LastIndexOf(source, value.ToString(), CompareOptions);

    /// <summary>
    /// <inheritdoc cref="TextInfo.ToUpper(string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public string ToUpper(string value) => CultureInfo.TextInfo.ToUpper(value);

    /// <summary>
    /// <inheritdoc cref="TextInfo.ToUpper(char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public char ToUpper(char value) => CultureInfo.TextInfo.ToUpper(value);

    /// <summary>
    /// <inheritdoc cref="TextInfo.ToLower(string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public string ToLower(string value) => CultureInfo.TextInfo.ToLower(value);

    /// <summary>
    /// <inheritdoc cref="TextInfo.ToLower(char)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public char ToLower(char value) => CultureInfo.TextInfo.ToLower(value);

    /// <summary>
    /// <inheritdoc cref="TextInfo.ToTitleCase(string)"/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public string ToTitleCase(string value) => CultureInfo.TextInfo.ToTitleCase(value);
}