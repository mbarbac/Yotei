using Entry = (string Value, bool IsSeparator);

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the extended 'string.Split' capabilities.
/// </summary>
public readonly record struct StringSplitter
{
    /// <summary>
    /// Initializes a default instance.
    /// </summary>
    public StringSplitter() { }

    /// <summary>
    /// The comparison mode used to find separators in the source.
    /// </summary>
    public StringComparison Comparison { get; init; }

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
    /// If requested, separators are kept in the results' set instead of being removed.
    /// </summary>
    public bool KeepSeparators { get; init; }
}

// ========================================================
// TODO: Use C# 14 static extension methods to extend 'string.Splitter' capabilities.
public static class StringSplitterExtensions
{
    /// <summary>
    /// Splits the given source into substrings using the given separator. Each substring may
    /// also be that separator, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        char separator,
        StringSplitter options)
        => source.Split([separator], options);

    /// <summary>
    /// Splits the given source into substrings using the given separators. Each substring may
    /// also be one of those separators, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        char[] separators,
        StringSplitter options)
    {
        var temps = separators.ThrowWhenNull().Select(x => x.ToString()).ToArray();
        return source.Split(temps, options);
    }

    /// <summary>
    /// Splits the given source into substrings using the given separator. Each substring may
    /// also be that separator, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        string separator,
        StringSplitter options)
        => source.Split([separator], options);

    /// <summary>
    /// Splits the given source into substrings using the given separators. Each substring may
    /// also be one of those separators, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        string[] separators,
        StringSplitter options)
    {
        source.ThrowWhenNull();
        separators.ThrowWhenNull();

        // Standard method is notably faster...
        if (!options.KeepSeparators)
        {
            var temp = StringSplitOptions.None;
            if (options.TrimEntries) temp |= StringSplitOptions.TrimEntries;
            if (options.RemoveEmptyEntries) temp |= StringSplitOptions.RemoveEmptyEntries;

            var items = source.Split(separators, temp);
            foreach (var item in items) yield return (item, false);
            yield break;
        }

        // Keeping separators requires different logic...
        // OPTIMIZE: string.Splitter
        else
        {
            int i = 0, last = 0;
            int len;
            ReadOnlySpan<char> span;

            // Loop through source contents...
            LOOP:
            while (i < source.Length)
            {
                // First separator wins...
                foreach (var separator in separators)
                {
                    span = source.AsSpan(i);
                    if (!span.StartsWith(separator, options.Comparison)) continue;

                    // Pending characters...
                    len = i - last;
                    span = source.AsSpan(last, len);
                    if (options.TrimEntries) span = span.Trim();
                    if (!options.RemoveEmptyEntries || span.Length > 0)
                        yield return (span.ToString(), false);

                    // Found separator...
                    if (options.KeepSeparators) yield return (separator, true);

                    // Adjusting...
                    i = last = (i + separator.Length);
                    goto LOOP;
                }

                // Or advance to next character...
                i++;
            }

            // Remaining...
            len = source.Length - last;
            span = source.AsSpan(last, len);
            if (options.TrimEntries) span = span.Trim();
            if (!options.RemoveEmptyEntries || span.Length > 0)
                yield return (span.ToString(), false);
        }
    }
}