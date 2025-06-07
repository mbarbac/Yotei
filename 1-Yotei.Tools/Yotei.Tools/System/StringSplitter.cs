CHANGE:
No admito Source siendo null,
Mimic what string.Split does.

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a forward-only string splitter.
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
        Results?.Dispose();
        Results = null;

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

    /// <inheritdoc/>
    void IEnumerator.Reset() { }

    // ----------------------------------------------------

    int Index = 0;
    int Last = 0;
    IEnumerator<ReadOnlyMemory<char>>? Results = null;

    IEnumerator<ReadOnlyMemory<char>> GetResults()
    {
        // Iterating through source...
        if (Source is not null)
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

                    if (Index > 0) // Separators at position 0 has not a previous element...
                    {
                        Current = Source.AsMemory(Last, Index - Last);

                        if (TrimEntries) Current = Current.Trim();
                        if (!OmitEmptyEntries || (OmitEmptyEntries && Current.Length > 0))
                        {
                            yield return Current;
                        }
                    }

                    if (!OmitSeparators) // Reporting the separator if such is allowed...
                    {
                        IsSeparator = true;
                        Current = item.AsMemory();

                        Index += item.Length; Last = Index;
                        Index--;

                        yield return Current;
                    }

                    break; // We've found a separator, we need not to try more...
                }

                // Advance to next character...
                Index++;
            }

            // Remaining...
            var len = Source.Length - Last;
            if (len > 0)
            {
                Current = Source.AsMemory(Last, len);

                if (TrimEntries) Current = Current.Trim();
                if (!OmitEmptyEntries || (OmitEmptyEntries && Current.Length > 0))
                {
                    Last = int.MaxValue;
                    yield return Current;
                }
            }
        }

        // Finishing...
        yield break;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        Results ??= GetResults();

        while (Results.MoveNext()) return true;
        return false;
    }
}