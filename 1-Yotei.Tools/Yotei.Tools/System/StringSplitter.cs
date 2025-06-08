namespace Yotei.Tools;

// ========================================================
public static class StringSplitterExtensions
{
    /// <summary>
    /// Returns a new splitter for the given source string and separators.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static StringSplitter Splitter(this string source, params IEnumerable<string> separators)
    {
        return new StringSplitter(source, separators);
    }

    /// <summary>
    /// Returns a new splitter for the given source string and separators.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    public static StringSplitter Splitter(this string source, params IEnumerable<char> separators)
    {
        return new StringSplitter(source, separators);
    }
}

// ========================================================
/// <summary>
/// Represents a forward-only string splitter.
/// <br/> The class aligns with the 'string.Split()' family of methods' behavior, but accepts a
/// custom string comparison, produces span-alike results, and the separators are also produced
/// as valid results by default.
/// </summary>
public record class StringSplitter : IEnumerable<ReadOnlyMemory<char>>, IEnumerator<ReadOnlyMemory<char>>
{
    /// <summary>
    /// Initializes a new instance for the given source string and separators.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string source, params IEnumerable<string> separators)
    {
        Source = source.ThrowWhenNull();
        Separators = separators.ThrowWhenNull().ToImmutableArray();
    }

    /// <summary>
    /// Initializes a new instance for the given source string and separators.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string source, params IEnumerable<char> separators)
        : this(source, separators.ThrowWhenNull().Select(x => x.ToString())) { }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        Reset();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    public StringSplitter GetEnumerator() => this;
    IEnumerator<ReadOnlyMemory<char>> IEnumerable<ReadOnlyMemory<char>>.GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of separators to use to split the source string.
    /// </summary>
    public ImmutableArray<string> Separators
    {
        get => _Separators;
        init
        {
            _Separators = value.ThrowWhenNull();

            if (_Separators.Length == 0) throw new ArgumentException("No separators provided.");
            foreach (var item in value) item.NotNullNotEmpty(trim: false);
        }
    }
    ImmutableArray<string> _Separators = default!;

    /// <summary>
    /// The comparison to use to compare string values. If not set while constructing this
    /// instance, its default value is the current culture one.
    /// </summary>
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;

    /// <summary>
    /// Omit from the resulting set those elements that are any of the given separators.
    /// </summary>
    public bool OmitSeparators { get; init; }

    /// <summary>
    /// Omit from the resulting set those regular entries that are empty strings.
    /// </summary>
    public bool OmitEmptyEntries { get; init; }

    /// <summary>
    /// Trim the regular not-separator entries in the resulting set. If this flag is set along
    /// with the '<see cref="OmitEmptyEntries"/>' one, then white-space only regular strings are
    /// removed from the resulting set.
    /// </summary>
    public bool TrimEntries { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// The source string being split.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets the element at the current position of this enumerator.
    /// </summary>
    public ReadOnlyMemory<char> Current { get; private set; }
    object IEnumerator.Current => Current;

    /// <summary>
    /// Determines if the current element is one among the given separators, or not.
    /// </summary>
    public bool IsSeparator { get; private set; }

    // ----------------------------------------------------

    int Index = 0;
    int Last = 0;
    IEnumerator<ReadOnlyMemory<char>>? Results = null;

    /// <inheritdoc/>
    public void Reset()
    {
        Index = 0;
        Last = 0;

        Results?.Dispose();
        Results = null;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        Results ??= GetResults();

        while (Results.MoveNext()) return true;
        return false;
    }

    /// <summary>
    /// Invoked to produce the actual results of this iterator.
    /// </summary>
    IEnumerator<ReadOnlyMemory<char>> GetResults()
    {
        Current = default;
        IsSeparator = false;

        // Looping through source...
        while (Index < Source.Length)
        {
            var span = Source[Index..];

            // First separator found wins...

            foreach (var item in Separators)
            {
                if (!span.StartsWith(item, Comparison)) continue;

                // Contents before the separator...
                Current = Source.AsMemory(Last, Index - Last);

                if (TrimEntries) Current = Current.Trim();
                if (!OmitEmptyEntries || (OmitEmptyEntries && Current.Length > 0))
                {
                    yield return Current;
                }

                // Reporting the separator itself...
                if (!OmitSeparators)
                {
                    Current = Source.AsMemory(Index, item.Length);
                    IsSeparator = true;

                    yield return Current;
                }

                Index += item.Length;
                Last = Index;
                Index--;
                break;
            }

            // Advance to next character...
            Index++;
        }

        // Remaining...
        var len = Source.Length - Last;
        if (len >= 0)
        {
            Current = Source.AsMemory(Last, len);
            Last = int.MaxValue;

            if (TrimEntries) Current = Current.Trim();
            if (!OmitEmptyEntries || (OmitEmptyEntries && Current.Length > 0))
            {
                yield return Current;
            }
        }

        // Finishing...
        yield break;
    }
}