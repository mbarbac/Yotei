using Entry = (System.ReadOnlyMemory<char> Value, bool IsSeparator);

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the 
/// </summary>
public readonly record struct StringSplitOptionsEx
{
    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public StringSplitOptionsEx() { }

    /// <summary>
    /// The comparison mode used to find separators in the source.
    /// </summary>
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;

    /// <summary>
    /// Trim white-space characters from each result. If <see cref="RemoveEmptyEntries"/> is also
    /// requested, then results consisting in white spaces only are not returned.
    /// </summary>
    public bool TrimEntries { get; init; }

    /// <summary>
    /// Removes from the results' set any empty elements. If <see cref="TrimEntries"/> is also
    /// requested, then results consisting in white spaces only are not returned.
    /// </summary>
    public bool RemoveEmptyEntries { get; init; }

    /// <summary>
    /// If requested, separators are not included in the results' set. The default value of this
    /// property is '<c>true</c>' to mimic the default 'string.Split(...)' behavior.
    /// </summary>
    public bool RemoveSeparators { get; init; } = true;
}

// ========================================================
public static class StringSplitExtensions
{
    /// <summary>
    /// Slits the given source using the given <see cref="StringSplitOptionsEx"/> options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        StringSplitOptionsEx options, params char[] separators)
    {
        var temps = separators.ThrowWhenNull().Select(x => x.ToString()).ToArray();
        return source.Split(options, temps);
    }

    /// <summary>
    /// Slits the given source using the given <see cref="StringSplitOptionsEx"/> options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        StringSplitOptionsEx options, params string[] separators)
    {
        source.ThrowWhenNull();
        foreach (var item in separators)
            if (string.IsNullOrWhiteSpace(item)) throw new ArgumentException(
                "Collection of separators carries null or empty elements.")
                .WithData(separators);

        // Easy cases...
        if (separators.Length == 0) { yield return (source.AsMemory(), false); yield break; }
        if (source.Length == 0) { yield return (default, false); yield break; }

        // Looping through the source...
        int i = 0, last = 0;
        int len;
        ReadOnlyMemory<char> span;

        LOOP:
        while (i < source.Length)
        {
            span = source.AsMemory(i);

            // First separator wins...
            foreach (var separator in separators)
            {
                if (!span.Span.StartsWith(separator, options.Comparison)) continue;

                // Pending characters...
                len = i - last;
                span = source.AsMemory(last, len);
                if (options.TrimEntries) span = span.Trim();
                if (!options.RemoveEmptyEntries || span.Length > 0) yield return (span, false);

                // Found separator...
                if (!options.RemoveSeparators) yield return (separator.AsMemory(), true);

                // Adjusting...
                i = last = (i + separator.Length);
                goto LOOP;
            }

            // Advance to next character...
            i++;
        }

        // Remaining...
        len = source.Length - last;
        span = source.AsMemory(last, len);
        if (options.TrimEntries) span = span.Trim();
        if (!options.RemoveEmptyEntries || span.Length > 0) yield return (span, false);
    }
}