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

    /// <inheritdoc>
    /// </inheritdoc>
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

    /// <inheritdoc>
    /// </inheritdoc>
    public int Compare(
        string? x, string? y)
        => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <inheritdoc cref="Compare(string?, string?)">
    /// </inheritdoc>
    public int Compare(
        ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <inheritdoc cref="Compare(string?, string?)">
    /// </inheritdoc>
    public int Compare(
        char x, char y)
        => CultureInfo.CompareInfo.Compare(x.ToString(), y.ToString(), CompareOptions);

    /// <inheritdoc cref="CompareInfo.IsPrefix(string, string)">
    /// </inheritdoc>
    public bool HasPrefix(
        string source, string prefix)
        => CultureInfo.CompareInfo.IsPrefix(source, prefix, CompareOptions);

    /// <inheritdoc cref="HasPrefix(string, string)">
    /// </inheritdoc>
    public bool HasPrefix(
        ReadOnlySpan<char> source, ReadOnlySpan<char> prefix)
        => CultureInfo.CompareInfo.IsPrefix(source, prefix, CompareOptions);

    /// <inheritdoc cref="CompareInfo.IsSuffix(string, string)">
    /// </inheritdoc>
    public bool HasSuffix(
        string source, string suffix)
        => CultureInfo.CompareInfo.IsSuffix(source, suffix, CompareOptions);

    /// <inheritdoc cref="HasSuffix(string, string)">
    /// </inheritdoc>
    public bool HasSuffix(
        ReadOnlySpan<char> source, ReadOnlySpan<char> suffix)
        => CultureInfo.CompareInfo.IsSuffix(source, suffix, CompareOptions);

    /// <inheritdoc cref="CompareInfo.IndexOf(string, string)">
    /// </inheritdoc>
    public int IndexOf(
        string source, string value)
        => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    /// <inheritdoc cref="IndexOf(string, string)">
    /// </inheritdoc>
    public int IndexOf(
        ReadOnlySpan<char> source, ReadOnlySpan<char> value)
        => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    /// <inheritdoc cref="CompareInfo.IndexOf(string, char)">
    /// </inheritdoc>
    public int IndexOf(
        string source, char value)
        => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    /// <inheritdoc cref="IndexOf(string, char)">
    /// </inheritdoc>
    public int IndexOf(
        ReadOnlySpan<char> source, char value)
        => CultureInfo.CompareInfo.IndexOf(source, value.ToString(), CompareOptions);

    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, string)">
    /// </inheritdoc>
    public int LastIndexOf(
        string source, string value)
        => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    /// <inheritdoc cref="LastIndexOf(string, string)">
    /// </inheritdoc>
    public int LastIndexOf(
        ReadOnlySpan<char> source, ReadOnlySpan<char> value)
        => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, char)">
    /// </inheritdoc>
    public int LastIndexOf(
        string source, char value)
        => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    /// <inheritdoc cref="LastIndexOf(string, char)">
    /// </inheritdoc>
    public int LastIndexOf(
        ReadOnlySpan<char> source, char value)
       => CultureInfo.CompareInfo.LastIndexOf(source, value.ToString(), CompareOptions);

    /// <inheritdoc cref="TextInfo.ToUpper(string)">
    /// </inheritdoc>
    public string ToUpper(string value) => CultureInfo.TextInfo.ToUpper(value);

    /// <inheritdoc cref="TextInfo.ToUpper(char)">
    /// </inheritdoc>
    public char ToUpper(char value) => CultureInfo.TextInfo.ToUpper(value);

    /// <inheritdoc cref="TextInfo.ToLower(string)">
    /// </inheritdoc>
    public string ToLower(string value) => CultureInfo.TextInfo.ToLower(value);

    /// <inheritdoc cref="TextInfo.ToLower(char)">
    /// </inheritdoc>
    public char ToLower(char value) => CultureInfo.TextInfo.ToLower(value);

    /// <inheritdoc cref="TextInfo.ToTitleCase(string)">
    /// </inheritdoc>
    public string ToTitleCase(string value) => CultureInfo.TextInfo.ToTitleCase(value);
}