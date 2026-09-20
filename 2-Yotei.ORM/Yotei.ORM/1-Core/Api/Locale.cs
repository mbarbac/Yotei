using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Provides support to culture-sensitive environments.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public sealed record Locale
    : IEqualityComparer<string>, IComparer<string?>
    , IEqualityComparer<StringSpan>, IComparer<StringSpan>
    , IEqualityComparer<char>, IComparer<char>
    , IFormatProvider
{
    /// <summary>
    /// Initializes a new instance using the current culture and no comparison options.
    /// </summary>
    public Locale() : this(CultureInfo.CurrentCulture, CompareOptions.None) { }

    /// <summary>
    /// Initializes a new instance using the given culture and no comparison options.
    /// </summary>
    /// <param name="culture"></param>
    public Locale(CultureInfo culture) : this(culture, CompareOptions.None) { }

    /// <summary>
    /// Initializes a new instance using the current culture and the given options.
    /// </summary>
    /// <param name="options"></param>
    public Locale(CompareOptions options) : this(CultureInfo.CurrentCulture, options) { }

    /// <summary>
    /// Initializes a new instance using the given culture and options (which unless explicitly
    /// given are set to none).
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="options"></param>
    public Locale(CultureInfo culture, CompareOptions options = CompareOptions.None)
    {
        CultureInfo = culture;
        CompareOptions = options;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    public Locale(Locale other)
    {
        ArgumentNullException.ThrowIfNull(other);

        CultureInfo = other.CultureInfo;
        CompareOptions = other.CompareOptions;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(CultureInfo.Name.NullWhenEmpty(false) ?? "Invariant");
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
    /// <inheritdoc/>
    /// </summary>
    public CultureInfo CultureInfo
    {
        get;
        init => field = CultureInfo.ReadOnly(value.ThrowWhenNull());
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CompareOptions CompareOptions { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(
        string? x, string? y) => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(string? x, string? y) => Compare(x, y) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(
        [DisallowNull] string obj) => CultureInfo.CompareInfo.GetHashCode(obj, CompareOptions);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(
        StringSpan x, StringSpan y) => CultureInfo.CompareInfo.Compare(x, y, CompareOptions);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(StringSpan x, StringSpan y) => Compare(x, y) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(
        [DisallowNull] StringSpan obj) => CultureInfo.CompareInfo.GetHashCode(obj, CompareOptions);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(char x, char y)
    {
        StringSpan xspan = [x];
        StringSpan yspan = [y];
        return CultureInfo.CompareInfo.Compare(xspan, yspan, CompareOptions);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(char x, char y) => Compare(x, y) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode([DisallowNull] char obj)
    {
        StringSpan objspan = [obj];
        return CultureInfo.CompareInfo.GetHashCode(objspan, CompareOptions);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="formatType"></param>
    /// <returns><inheritdoc cref="CultureInfo.GetFormat(Type?)"/></returns>
    public object? GetFormat(Type? formatType) => CultureInfo.GetFormat(formatType);
}