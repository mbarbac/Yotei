using Microsoft.VisualBasic;
using Entry = System.ReadOnlyMemory<char>;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a forward-only string splitter.
/// </summary>
/// <remarks>
/// This class' behavior is somehow similar to the standard 'string.sSplit(...)' one, but:
/// <br/>- Provides custom comparison capabilities.
/// <br/>- Found separators are included in the results set instead of being ignored, unless
/// <see cref="OmitSeparators"/> is requested to mimic 'string.Split(...)' behavior.
/// <br/>- And produces span-alike results.
/// </remarks>
public record class StringSplitter : IEnumerable<Entry>, IEnumerator<Entry>
{
    IEnumerator<StrItem>? Results = null;

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
    {
        Source = source.ThrowWhenNull();
        Separators = separators.ThrowWhenNull().Select(x => x.ToString()).ToImmutableArray();
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        Reset();
        GC.SuppressFinalize(this);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The source string being split.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// The collection of separators used by this instance to split the source string.
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
    ImmutableArray<string> _Separators = [];

    // ----------------------------------------------------

    /// <summary>
    /// The comparison to use to compare string values. If not set while constructing this
    /// instance, its default value is the current culture one.
    /// </summary>
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;

    /// <summary>
    /// Include in the results set the elements that are separators.
    /// </summary>
    public bool OmitSeparators { get; init; }

    /// <summary>
    /// Omit from the results set those regular entries that are empty strings.
    /// </summary>
    public bool RemoveEmptyEntries { get; init; }

    /// <summary>
    /// Trim the regular not-separator entries in the results set.
    /// If this flag is set along with the '<see cref="RemoveEmptyEntries"/>' one, then the white
    /// space only regular strings are removed from the resulting set.
    /// </summary>
    public bool TrimEntries { get; init; }

    // ----------------------------------------------------

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    public StringSplitter GetEnumerator() => this;
    IEnumerator<Entry> IEnumerable<Entry>.GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public Entry Current { get; private set; }
    object IEnumerator.Current => this;

    /// <summary>
    /// Determines if the current element is one among the given separators, or not.
    /// </summary>
    public bool IsCurrentSeparator { get; private set; }

    /// <inheritdoc/>
    public void Reset()
    {
        if (Results is not null)
        {
            Results.Dispose();
            Results = null;
        }
        Current = default;
        IsCurrentSeparator = false;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        Results ??= GetResults();

        while (Results.MoveNext())
        {
            Current = Results.Current.Value;
            IsCurrentSeparator = Results.Current.IsSeparator;
            return true;
        }
        return false;
    }

    // ----------------------------------------------------

    // Represents an individual item in the source string.
    readonly record struct StrItem(Entry Value, bool IsSeparator);

    // Enumerates the items in the souce string.
    IEnumerator<StrItem> GetResults()
    {
        int i = 0, last = 0;
        int len;
        Entry entry;

        // Looping through the source...
        LOOP:
        while (i < Source.Length)
        {
            var span = Source[i..];

            // First separator wins...
            foreach (var item in Separators)
            {
                if (!span.StartsWith(item, Comparison)) continue;

                // Pending characters...
                len = i - last;
                entry = Source.AsMemory(last, len);

                if (TrimEntries) entry = entry.Trim();
                if (!RemoveEmptyEntries || (RemoveEmptyEntries && entry.Length > 0))
                    yield return new(entry, false);

                // The found separator...
                if (!OmitSeparators) yield return new(item.AsMemory(), true);

                // Adjusting...
                i = last = (i + item.Length);
                goto LOOP;
            }

            // Otherwise, advance to the next character...
            i++;
        }

        // Remaining characters...
        len = Source.Length - last;
        entry = Source.AsMemory(last, len);

        if (TrimEntries) entry = entry.Trim();
        if (!RemoveEmptyEntries || (RemoveEmptyEntries && entry.Length > 0))
            yield return new(entry, false);
    }
}