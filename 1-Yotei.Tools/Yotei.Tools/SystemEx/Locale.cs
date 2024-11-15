using SpanChar = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an immutable collection of culture sensitive settings and related methods.
/// </summary>
public record class Locale
    : IComparer<string?>, IComparer<char>, IComparer<SpanChar>
    , IEqualityComparer<string?>, IEqualityComparer<char>, IEqualityComparer<SpanChar>
{
    /// <summary>
    /// Initializes a new instance using the culture of the current thread and default comparison
    /// options.
    /// </summary>
    public Locale() { }

    /// <summary>
    /// Initializes a new instance using the culture of the current thread and the given comparison
    /// options.
    /// </summary>
    /// <param name="options"></param>
    public Locale(CompareOptions options) => CompareOptions = options;

    /// <summary>
    /// Initializes a new instance using the given culture and default comparison options.
    /// </summary>
    /// <param name="culture"></param>
    public Locale(CultureInfo culture) => CultureInfo = culture;

    /// <summary>
    /// Initializes a new instance using the given culture and comparison options.
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="options"></param>
    public Locale(CultureInfo culture, CompareOptions options) : this(culture) => CompareOptions = options;

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(CultureInfo.Name.NullWhenEmpty() ?? "Invariant");
        if (CompareOptions == CompareOptions.None) sb.Append(", None");
        else
        {
            if (CompareOptions.HasFlag(CompareOptions.IgnoreCase)) sb.Append($", {nameof(CompareOptions.IgnoreCase)}");
            if (CompareOptions.HasFlag(CompareOptions.IgnoreNonSpace)) sb.Append($", {nameof(CompareOptions.IgnoreNonSpace)}");
            if (CompareOptions.HasFlag(CompareOptions.IgnoreSymbols)) sb.Append($", {nameof(CompareOptions.IgnoreSymbols)}");
            if (CompareOptions.HasFlag(CompareOptions.IgnoreKanaType)) sb.Append($", {nameof(CompareOptions.IgnoreKanaType)}");
            if (CompareOptions.HasFlag(CompareOptions.IgnoreWidth)) sb.Append($", {nameof(CompareOptions.IgnoreWidth)}");
            if (CompareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase)) sb.Append($", {nameof(CompareOptions.OrdinalIgnoreCase)}");
            if (CompareOptions.HasFlag(CompareOptions.StringSort)) sb.Append($", {nameof(CompareOptions.StringSort)}");
            if (CompareOptions.HasFlag(CompareOptions.Ordinal)) sb.Append($", {nameof(CompareOptions.Ordinal)}");
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The read-only culture this instance represents.
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

    /// <inheritdoc/>
    public int Compare(string? x, string? y) => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <inheritdoc/>
    public int Compare(char x, char y) => Compare(x.ToString(), y.ToString());

    /// <inheritdoc/>
    public int Compare(SpanChar x, SpanChar y) => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(string? x, string? y) => Compare(x, y) == 0;

    /// <inheritdoc/>
    public bool Equals(char x, char y) => Compare(x, y) == 0;

    /// <inheritdoc/>
    public bool Equals(SpanChar x, SpanChar y) => Compare(x, y) == 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] string? obj) => obj is null ? 0 : obj.GetHashCode();

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] char obj) => obj.GetHashCode();

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] SpanChar obj) => obj.ToString().GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.IsPrefix(string, string)"/>
    public bool StartsWith(SpanChar source, char value) => StartsWith(source, value.ToString());

    /// <inheritdoc cref="CompareInfo.IsPrefix(string, string)"/>
    public bool StartsWith(
        SpanChar source, SpanChar value) => CultureInfo.CompareInfo.IsPrefix(source, value, CompareOptions);

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.IsSuffix(string, string)"/>
    public bool EndsWith(SpanChar source, char value) => EndsWith(source, value.ToString());

    /// <inheritdoc cref="CompareInfo.IsSuffix(string, string)"/>
    public bool EndsWith(
        SpanChar source, SpanChar value) => CultureInfo.CompareInfo.IsSuffix(source, value, CompareOptions);

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.IndexOf(string, string)"/>
    public int IndexOf(SpanChar source, char value) => IndexOf(source, value.ToString());

    /// <inheritdoc cref="CompareInfo.IndexOf(string, string)"/>
    public int IndexOf(
        SpanChar source, SpanChar value) => CultureInfo.CompareInfo.IndexOf(source, value, CompareOptions);

    // ----------------------------------------------------

    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, string)"/>
    public int LastIndexOf(SpanChar source, char value) => LastIndexOf(source, value.ToString());

    /// <inheritdoc cref="CompareInfo.LastIndexOf(string, string)"/>
    public int LastIndexOf(
        SpanChar source, SpanChar value) => CultureInfo.CompareInfo.LastIndexOf(source, value, CompareOptions);

    // ----------------------------------------------------

    /// <inheritdoc cref="TextInfo.ToUpper(string)"/>
    public string ToUpper(string value) => CultureInfo.TextInfo.ToUpper(value);

    /// <inheritdoc cref="TextInfo.ToUpper(char)"/>
    public char ToUpper(char value) => CultureInfo.TextInfo.ToUpper(value);

    // ----------------------------------------------------

    /// <inheritdoc cref="TextInfo.ToLower(string)"/>
    public string ToLower(string value) => CultureInfo.TextInfo.ToLower(value);

    /// <inheritdoc cref="TextInfo.ToLower(char)"/>
    public char ToLower(char value) => CultureInfo.TextInfo.ToLower(value);

    // ----------------------------------------------------

    /// <inheritdoc cref="TextInfo.ToTitleCase(string)"/>
    public string ToTitleCase(string value) => CultureInfo.TextInfo.ToTitleCase(value);
}