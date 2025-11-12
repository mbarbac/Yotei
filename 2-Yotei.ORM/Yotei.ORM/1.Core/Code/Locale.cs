namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ILocale"/>
/// </summary>
public record Locale : ILocale
{
    /// <summary>
    /// Initializes a new instance with an invariant culture and default comparison options.
    /// </summary>
    public Locale() { }

    /// <summary>
    /// Initializes a new instance with the given culture and comparison options.
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="options"></param>
    public Locale(CultureInfo culture, CompareOptions options = CompareOptions.None)
    {
        CultureInfo = culture;
        CompareOptions = options;
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
            if(CompareOptions.HasFlag(CompareOptions.IgnoreCase)) sb.Append($", {nameof(CompareOptions.IgnoreCase)}");
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
    = CultureInfo.InvariantCulture;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CompareOptions CompareOptions { get; init; } = CompareOptions.None;
}