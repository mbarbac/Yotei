using Entry = (System.ReadOnlyMemory<char>, bool IsSeparator);
using StrMemory = System.ReadOnlyMemory<char>;
using StrSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a forward-only string splitter.
/// </summary>
public record struct StringSplitter : IEnumerable<Entry>, IEnumerator<Entry>
{
    /// <summary>
    /// Invalid constructor.
    /// </summary>
    public StringSplitter()
        => throw new InvalidOperationException("Cannot create an empty instance.");

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string source, params IEnumerable<string> separators)
    {
        Source = source.ThrowWhenNull().AsMemory();
        Separators = Validate(separators.ThrowWhenNull().Select(x => x.AsMemory()));
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(StrSpan source, params IEnumerable<StrSpan> separators)
    {
        //Source = source;
        //Separators = Validate(separators.ThrowWhenNull());
    }

    // Returns a validated array of separators.
    static StrMemory[] Validate(IEnumerable<StrMemory> separators)
    {
        separators.ThrowWhenNull();

        var items = separators.ToArray();
        foreach (var item in items)
            if (item.Length == 0) throw new EmptyException("Separator is empty.");

        return items;
    }

    /// <inheritdoc/>
    void IDisposable.Dispose() => OnReset();

    /// <inheritdoc/>
    public readonly IEnumerator<Entry> GetEnumerator() => this;
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    readonly StrMemory Source;
    readonly StrMemory[] Separators;
    IEnumerator<Entry>? Results = null;

    /// <inheritdoc/>
    public Entry Current { get; private set; }
    readonly object IEnumerator.Current => Current;

    /// <inheritdoc/>
    void IEnumerator.Reset() => OnReset();
    void OnReset()
    {
        if (Results is not null)
        {
            Results.Dispose();
            Results = null;
        }
        Current = default;
        Array.Clear(Separators);
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        Results ??= GetResults();

        while (Results.MoveNext())
        {
            Current = Results.Current;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Facilitates using 'yield' to get the desired results.
    /// </summary>
    readonly IEnumerator<Entry> GetResults()
    {
        // Looping through the source...
        int i = 0, last = 0;
        int len;
        StrMemory span;

        LOOP:
        while (i < Source.Length)
        {
            span = Source[i..];

            // First separator wins...
            foreach (var separator in Separators)
            {
                if (!span.Span.StartsWith(separator.Span, Comparison)) continue;

                // Pending characters...
                len = i - last;
                span = Source.Slice(last, len);
                if (TrimEntries) span = span.Trim();
                if (!RemoveEmptyEntries || span.Length > 0) yield return (span, false);

                // The found separator...
                if (!RemoveSeparators) yield return (separator, true);

                // Adjusting...
                i = last = (i + separator.Length);
                goto LOOP;
            }

            // Advance to next character...
            i++;
        }

        // Remaining characters...
        len = Source.Length - last;
        span = Source.Slice(last, len);
        if (TrimEntries) span = span.Trim();
        if (!RemoveEmptyEntries || span.Length > 0) yield return (span, false);
    }
}