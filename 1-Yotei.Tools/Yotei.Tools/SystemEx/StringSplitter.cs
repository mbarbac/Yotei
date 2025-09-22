using System.Net.Sockets;
using Entry = (System.ReadOnlyMemory<char> Item, bool IsSeparator);

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the extended 'string.Split' capabilities.
/// </summary>
public readonly record struct StringSplitterOptions
{
    /// <summary>
    /// Initializes a default instance.
    /// </summary>
    public StringSplitterOptions() { }

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
public static class StringSplitterExtensions
{
    /// <summary>
    /// Splits the given source string into its components, identified by their respective values
    /// and whether they are one of the given separators, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        StringSplitterOptions options,
        params IEnumerable<char> separators)
    {
        source.ThrowWhenNull();
        separators.ThrowWhenNull();

        var temps = separators.Select(x => x.ToString());
        return source.Split(options, temps);
    }

    /// <summary>
    /// Splits the given source string into its components, identified by their respective values
    /// and whether they are one of the given separators, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static IEnumerable<Entry> Split(
        this string source,
        StringSplitterOptions options,
        params IEnumerable<string> separators)
    {
        int i = 0, last = 0;
        int len;
        ReadOnlyMemory<char> item;

        // Loop through source contents...
        LOOP:
        while (i < source.Length)
        {
            // First separator wins...
            foreach (var separator in separators)
            {
                if (!source.AsSpan(i).StartsWith(separator, options.Comparison)) continue;

                // Pending characters...
                len = i - last;
                item = source.AsMemory(last, len);
                if (options.TrimEntries) item = item.Trim();
                if (!options.RemoveEmptyEntries || item.Length > 0) yield return (item, false);

                // Found separator...
                if (options.KeepSeparators) yield return (separator.AsMemory(), true);

                // Adjusting...
                i = last = (i + separator.Length);
                goto LOOP;
            }

            // Or advance to next character...
            i++;
        }

        // Remaining...
        len = source.Length - last;
        item = source.AsMemory(last, len);
        if (options.TrimEntries) item = item.Trim();
        if (!options.RemoveEmptyEntries || item.Length > 0) yield return (item, false);
    }
}