using Entry = (System.Range Range, bool IsSeparator);

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of split source string in its components. Instances of this type are
/// forward-only enumerators.
/// </summary>
public record struct StringSplitter : IEnumerable<Entry>, IEnumerator<Entry>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string source, params IEnumerable<string> separators)
    {
        Source = source.ThrowWhenNull();
        Separators = Validate(separators.ThrowWhenNull());

        static string[] Validate(IEnumerable<string> separators)
        {
            var items = separators.ToArray();
            foreach (var item in items)
                if (string.IsNullOrWhiteSpace(item)) throw new ArgumentException(
                    "Collection of split separators carries null or empty elements.")
                    .WithData(separators);

            return items;
        }
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string source, params IEnumerable<char> separators)
    {
        Source = source.ThrowWhenNull();
        Separators = Validate(separators.ThrowWhenNull());

        static string[] Validate(IEnumerable<char> separators)
        {
            foreach (var temp in separators)
                if (temp == 0) throw new ArgumentException(
                    "Collection of split separators carries invalid elements.")
                    .WithData(separators);

            var items = separators.Select(x => x.ToString()).ToArray();
            return items;
        }
    }

    /// <inheritdoc/>
    public void Dispose() => Reset();

    /// <inheritdoc/>
    readonly public IEnumerator<Entry> GetEnumerator() => this;
    readonly IEnumerator IEnumerable.GetEnumerator() => this;

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    readonly string Source;
    readonly string[] Separators;
    IEnumerator<Entry>? Results;

    /// <inheritdoc/>
    public Entry Current { get; private set; }
    readonly object IEnumerator.Current => Current;

    /// <inheritdoc/>
    public void Reset()
    {
        if (Results is not null)
        {
            Results.Dispose();
            Results = null!;
        }
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        Results ??= GetResults();

        if (Results.MoveNext())
        {
            Current = Results.Current;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Invoked to obtain the actual enumerable instance.
    /// </summary>
    readonly IEnumerator<Entry> GetResults()
    {
        int i = 0, last = 0;
        int len;
        Range range;

        // Loop through source contents...
        LOOP:
        while (i < Source.Length)
        {
            // First separator wins...
            foreach (var separator in Separators)
            {
                if (!Source.AsSpan(i).StartsWith(separator, Comparison)) continue;

                // Pending characters..
                len = i - last;
                range = Compute(Source, TrimEntries, last, len);
                if (!RemoveEmptyEntries || Len(range) > 0) yield return (range, false);

                // Found separator...
                if (KeepSeparators) yield return (new(i, i + separator.Length), true);

                // Adjusting...
                i = last = (i + separator.Length);
                goto LOOP;
            }

            // Or advance to next character...
            i++;
        }

        // Remaining...
        len = Source.Length - last;
        range = Compute(Source, TrimEntries, last, len);
        if (!RemoveEmptyEntries || Len(range) > 0) yield return (range, false);

        // Computes the suitable range...
        static Range Compute(string source, bool trim, int ini, int len)
        {
            var end = ini + len;
            if (!trim) return new Range(ini, end);

            while (ini < end) { if (source[ini] == ' ') ini++; else break; }
            while (end > ini) { if (source[end - 1] == ' ') end--; else break; }
            return new Range(ini, end);
        }

        // Returns the lenght of the given range...
        static int Len(Range range) => range.End.Value - range.Start.Value;
    }
}